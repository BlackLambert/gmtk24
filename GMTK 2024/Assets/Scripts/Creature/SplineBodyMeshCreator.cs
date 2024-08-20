using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class SplineBodyMeshCreator : MonoBehaviour
    {
        [SerializeField] private BodySettings _bodySettings;
        [SerializeField] private Body _body;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private PolygonCollider2D _meshCollider;

        private void Awake()
        {
            RecreateMesh();
        }

        private void Start()
        {
            _body.OnUpdateMesh += RecreateMesh;
        }

        private void OnDestroy()
        {
            _body.OnUpdateMesh -= RecreateMesh;
        }

        private void RecreateMesh()
        {
            CreateResult result = CreateMesh();
            _meshFilter.mesh = result.Mesh;
            Vector2[] colliderVertices = _meshFilter.mesh.vertices.Select(v => (Vector2)v).ToArray();
            _meshCollider.points = colliderVertices.Distinct().ToArray();
            _body.UpdateSlots(result.SlotsPerSpline);
        }

        private CreateResult CreateMesh()
        {
            if (_body.BodyData == null || _body.BodyData.Splines.Count == 0)
            {
                _body.BodyData = new BodyData() { Splines = new List<SplineData>(_bodySettings.StartSplines) };
            }

            return CreateMeshFromData(_body.BodyData);
        }

        private CreateResult CreateMeshFromData(BodyData bodyData)
        {
            SplineData[] orderedSplineData = bodyData.Splines.OrderBy(s => s.Center.y).ToArray();
            SplineData startSpline = orderedSplineData.First();
            SplineData endSpline = orderedSplineData.Last();
            int sidePartsAmount = orderedSplineData.Length - 1;

            int startVerticesAmount = _bodySettings.StartHalfCircleVertices +
                                      _bodySettings.HalfCircleVerticesPerSize * startSpline.Size;
            int endVerticesAmount = _bodySettings.StartHalfCircleVertices +
                                    _bodySettings.HalfCircleVerticesPerSize * endSpline.Size;
            int sideVerticesAmount = sidePartsAmount * _bodySettings.SideVertices * 2;

            Vector3[] vertices = new Vector3[startVerticesAmount + endVerticesAmount + sideVerticesAmount];
            int[] triangles = new int[(vertices.Length - 6) * 3];
            Vector4[] tangents = new Vector4[vertices.Length];
            Vector3[] normals = new Vector3[vertices.Length];
            Vector2[] uvs = new Vector2[vertices.Length];
            float maxLength = GetRadius(startSpline) +
                GetRadius(endSpline) + _bodySettings.SplineSpacing * (orderedSplineData.Length - 1);
            float maxWidth = _bodySettings.MaxSize * _bodySettings.ScalePerSize + _bodySettings.MinScale;

            int[] slotAmounts = new int[orderedSplineData.Length];
            slotAmounts[0] = Mathf.RoundToInt(startSpline.Size * _bodySettings.SlotsPerSize);
            slotAmounts[^1] = Mathf.RoundToInt(endSpline.Size * _bodySettings.SlotsPerSize);

            for (int i = 1; i < slotAmounts.Length - 1; i++)
            {
                slotAmounts[i] = Mathf.CeilToInt(orderedSplineData[i].Size / 2f * _bodySettings.SlotsPerSize) * 2;
            }

            Dictionary<SplineData, List<BodyPartSlot>> slots = new Dictionary<SplineData, List<BodyPartSlot>>();

            // Bottom
            List<BodyPartSlot> bottomSlots = new List<BodyPartSlot>();
            slots.Add(startSpline, bottomSlots);
            float angleDelta = Mathf.PI / (startVerticesAmount - 1);
            float radius = GetRadius(startSpline);
            for (int i = 0; i < startVerticesAmount; i++)
            {
                float angle = angleDelta * i + Mathf.PI;
                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                Vector3 unitCirclePoint = new Vector3(cos, sin, 0);
                Vector3 circlePoint = unitCirclePoint * radius;
                Vector3 vertex = circlePoint + (Vector3)startSpline.Center;
                vertices[i] = vertex;
                uvs[i] = GetUV(vertex, maxWidth, maxLength);
                tangents[i] = new Vector4(unitCirclePoint.x, unitCirclePoint.y, 0, 0);
                normals[i] = new Vector3(unitCirclePoint.y, unitCirclePoint.x, 0);

                if (i > 0 && i < startVerticesAmount - 1)
                {
                    int j = (i - 1) * 3;
                    triangles[j] = i - 1;
                    triangles[j + 1] = startVerticesAmount - 1;
                    triangles[j + 2] = i;
                }
            }
            AddCurvedSlots(ref bottomSlots, startVerticesAmount, 0, normals, vertices, slotAmounts[0], BodyPartSlotType.Back);

            
            // Front
            angleDelta = Mathf.PI / (endVerticesAmount - 1);
            radius = GetRadius(endSpline);
            List<BodyPartSlot> frontSlots = new List<BodyPartSlot>();
            slots.Add(endSpline, frontSlots);
            for (int i = 0; i < endVerticesAmount; i++)
            {
                float angle = angleDelta * i;
                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                Vector3 unitCirclePoint = new Vector3(cos, sin, 0);
                Vector3 circlePoint = unitCirclePoint * radius;
                Vector3 vertex = circlePoint + (Vector3)endSpline.Center;
                vertices[i + startVerticesAmount] = vertex;
                uvs[i + startVerticesAmount] = GetUV(vertex, maxWidth, maxLength);
                tangents[i + startVerticesAmount] = new Vector4(unitCirclePoint.x, unitCirclePoint.y, 0, 0);
                normals[i + startVerticesAmount] = new Vector3(unitCirclePoint.y, unitCirclePoint.x, 0);

                if (i > 0 && i < endVerticesAmount - 1)
                {
                    int j = (startVerticesAmount + i - 3) * 3;
                    triangles[j] = i + startVerticesAmount - 1;
                    triangles[j + 1] = startVerticesAmount + endVerticesAmount - 1;
                    triangles[j + 2] = i + startVerticesAmount;
                }
            }
            
            AddCurvedSlots(ref frontSlots, endVerticesAmount, startVerticesAmount, normals, vertices, slotAmounts[^1], BodyPartSlotType.Front);

            
            // Sides
            int vertexIndex = startVerticesAmount + endVerticesAmount;
            int triangleIndex = (startVerticesAmount + endVerticesAmount - 4) * 3;
            for (int i = 0; i < sidePartsAmount; i++)
            {
                SplineData current = orderedSplineData[i];
                SplineData next = orderedSplineData[i + 1];
                float currentScale = GetRadius(current);
                float nextScale = GetRadius(next);
                float vertexSlotSpacing = _bodySettings.SideVertices / (slotAmounts[i] / 2f) / 2;
                float nextSlot = -vertexSlotSpacing / 2;
                
                if (!slots.TryGetValue(current, out List<BodyPartSlot> sideSlots))
                {
                    sideSlots = new List<BodyPartSlot>();
                    slots.Add(current, sideSlots);
                }

                for (int j = 0; j < _bodySettings.SideVertices; j++)
                {
                    int v1 = vertexIndex;
                    int v2 = vertexIndex + _bodySettings.SideVertices;
                    int v3 = vertexIndex + 1;
                    int v4 = vertexIndex + _bodySettings.SideVertices + 1;

                    float portion = (float)j / (_bodySettings.SideVertices - 1);
                    float y = portion * _bodySettings.SplineSpacing + current.Center.y;
                    float x = Mathf.Lerp(currentScale, nextScale, _bodySettings.WeightCurve.Evaluate(portion));

                    Vector3 vertex = new Vector3(-x, y, 0);
                    normals[v1] = new Vector3(-1, 0, 0);
                    vertices[v1] = vertex;
                    uvs[v1] = GetUV(vertex, maxWidth, maxLength);
                    
                    vertex = new Vector3(x, y, 0);
                    normals[v2] = new Vector3(1, 0, 0);
                    vertices[v2] = vertex;
                    uvs[v2] = GetUV(vertex, maxWidth, maxLength);

                    if (j < _bodySettings.SideVertices - 1)
                    {
                        triangles[triangleIndex] = v1;
                        triangles[triangleIndex + 1] = v2;
                        triangles[triangleIndex + 2] = v3;
                        triangles[triangleIndex + 3] = v2;
                        triangles[triangleIndex + 4] = v4;
                        triangles[triangleIndex + 5] = v3;
                    }

                    if (nextSlot > 0)
                    {
                        nextSlot -= vertexSlotSpacing;
                        float slotAngle = Vector2.Angle(normals[v1], Vector2.up);
                        float factor = vertices[i].x <= 0 ? 1 : -1;
                        Quaternion rotation = Quaternion.Euler(0, 0, slotAngle * factor);
                        sideSlots.Add(new BodyPartSlot()
                        {
                            Position = vertices[v1], Rotation = rotation, VertexIndex = v1, Type = BodyPartSlotType.Side,
                            CounterPartIndex = sideSlots.Count + 1
                        });

                        slotAngle = Vector2.Angle(normals[v2], Vector2.up);
                        factor = vertices[i].x <= 0 ? 1 : -1;
                        rotation = Quaternion.Euler(0, 0, slotAngle * factor + 180);
                        sideSlots.Add(new BodyPartSlot()
                        {
                            Position = vertices[v2], Rotation = rotation, VertexIndex = v2,
                            CounterPartIndex = sideSlots.Count - 1, Type = BodyPartSlotType.Side
                        });
                    }

                    nextSlot++;
                    vertexIndex++;
                    triangleIndex += 6;
                }

                vertexIndex += _bodySettings.SideVertices;
            }

            Mesh mesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                tangents = tangents,
                normals = normals,
                uv = uvs
            };

            return new CreateResult
            {
                Mesh = mesh,
                SlotsPerSpline = slots
            };
        }

        private void AddCurvedSlots(ref List<BodyPartSlot> slots, int vertexAmount, int vertexOffset, Vector3[] normals,
            Vector3[] vertices, int slotAmount, BodyPartSlotType type)
        {
            bool isOdd = slotAmount % 2 == 1;
            if (!isOdd && _bodySettings.AlwaysAddTip)
            {
                isOdd = true;
                slotAmount++;
            }
            
            float vertexSlotSpacing = (float)vertexAmount / slotAmount;
            int center = vertexAmount / 2;
            
            for (int i = 0; i < slotAmount; i++)
            {
                if (isOdd && i == 0)
                {
                    slots.Add(CreateSlot(center + vertexOffset, BodyPartSlotType.Tip | type, normals, vertices));
                }
                else
                {
                    bool createSlot = isOdd && i % 2 == 0 || !isOdd && i % 2 == 1;
                    float pointAddition = i * vertexSlotSpacing / 2;
                    if (createSlot && center - (int)pointAddition >= 0)
                    {
                        slots.Add(CreateSlot(center - (int)pointAddition + vertexOffset, type, normals, vertices, slots.Count));
                        slots.Add(CreateSlot(center + (int)pointAddition + vertexOffset, type, normals, vertices, slots.Count - 2));
                    }
                }
            }
        }

        private BodyPartSlot CreateSlot(int vertexIndex, BodyPartSlotType type, Vector3[] normals,
            Vector3[] vertices, int counterPartIndex = -1)
        {
            float slotAngle = Vector2.SignedAngle(normals[vertexIndex], Vector2.up);
            Quaternion rotation = Quaternion.Euler(0, 0, slotAngle - 90);
            return new BodyPartSlot()
            {
                Position = vertices[vertexIndex], Rotation = rotation, VertexIndex = vertexIndex, Type = type,
                CounterPartIndex = counterPartIndex
            };
        }

        private float GetRadius(SplineData spline)
        {
            return (spline.Size * _bodySettings.ScalePerSize + _bodySettings.MinScale) / 2;
        }

        private Vector2 GetUV(Vector3 vertex, float maxWidth, float maxLength)
        {
            return new Vector2((vertex.x + maxWidth / 2) / maxWidth, (vertex.y + maxLength / 2) / maxLength);
        }

        private class CreateResult
        {
            public Mesh Mesh;
            public Dictionary<SplineData, List<BodyPartSlot>> SlotsPerSpline;
        }
    }
}