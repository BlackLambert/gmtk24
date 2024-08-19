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
            _body.UpdateSlots(result.Slots);
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

            int slotIndex = 0;
            int[] slotAmounts = new int[orderedSplineData.Length];
            slotAmounts[0] = Mathf.RoundToInt(startSpline.Size * _bodySettings.SlotsPerSize);
            slotAmounts[^1] = Mathf.RoundToInt(endSpline.Size * _bodySettings.SlotsPerSize);

            for (int i = 1; i < slotAmounts.Length - 1; i++)
            {
                slotAmounts[i] = Mathf.CeilToInt(orderedSplineData[i].Size / 2f * _bodySettings.SlotsPerSize) * 2;
            }
            List<BodyPartSlot> slots = new List<BodyPartSlot>();

            float angleDelta = Mathf.PI / (startVerticesAmount - 1);
            float radius = GetRadius(startSpline);
            float vertexSlotSpacing = (float)startVerticesAmount / (slotAmounts[0] + 1);
            float nextSlot = -vertexSlotSpacing;
            for (int i = 0; i < startVerticesAmount; i++)
            {
                float angle = angleDelta * i + Mathf.PI;
                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                Vector3 unitCirclePoint = new Vector3(cos, sin, 0);
                Vector3 circlePoint = unitCirclePoint * radius;
                vertices[i] = circlePoint + (Vector3)startSpline.Center;
                tangents[i] = new Vector4(unitCirclePoint.x, unitCirclePoint.y, 0, 0);
                normals[i] = new Vector3(unitCirclePoint.y, unitCirclePoint.x, 0);

                if (i > 0 && i < startVerticesAmount - 1)
                {
                    int j = (i - 1) * 3;
                    triangles[j] = i - 1;
                    triangles[j + 1] = startVerticesAmount - 1;
                    triangles[j + 2] = i;
                }

                if (nextSlot > 0)
                {
                    nextSlot -= vertexSlotSpacing;
                    float slotAngle = Vector2.SignedAngle(normals[i], Vector2.up);
                    Quaternion rotation = Quaternion.Euler(0, 0, slotAngle - 90);
                    slots.Add(new BodyPartSlot()
                        { Position = vertices[i], Rotation = rotation, VertexIndex = i });
                    slotIndex++;
                }
                
                nextSlot++;
            }

            angleDelta = Mathf.PI / (endVerticesAmount - 1);
            radius = GetRadius(endSpline);
            vertexSlotSpacing = (float)endVerticesAmount / (slotAmounts[^1] + 1);
            nextSlot = -vertexSlotSpacing;
            for (int i = 0; i < endVerticesAmount; i++)
            {
                float angle = angleDelta * i;
                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                Vector3 unitCirclePoint = new Vector3(cos, sin, 0);
                Vector3 circlePoint = unitCirclePoint * radius;
                vertices[i + startVerticesAmount] = circlePoint + (Vector3)endSpline.Center;
                tangents[i + startVerticesAmount] = new Vector4(unitCirclePoint.x, unitCirclePoint.y, 0, 0);
                normals[i + startVerticesAmount] = new Vector3(unitCirclePoint.y, unitCirclePoint.x, 0);

                if (i > 0 && i < endVerticesAmount - 1)
                {
                    int j = (startVerticesAmount + i - 3) * 3;
                    triangles[j] = i + startVerticesAmount - 1;
                    triangles[j + 1] = startVerticesAmount + endVerticesAmount - 1;
                    triangles[j + 2] = i + startVerticesAmount;
                }
                
                if (nextSlot > 0)
                {
                    nextSlot -= vertexSlotSpacing;
                    int index = startVerticesAmount + i;
                    float slotAngle = Vector2.SignedAngle(normals[index], Vector2.up);
                    Quaternion rotation = Quaternion.Euler(0, 0, slotAngle - 90);
                    slots.Add(new BodyPartSlot()
                        { Position = vertices[index], Rotation = rotation, VertexIndex = index });
                    slotIndex++;
                }
                
                nextSlot++;
            }

            // Sides
            int vertexIndex = startVerticesAmount + endVerticesAmount;
            int triangleIndex = (startVerticesAmount + endVerticesAmount - 4) * 3;
            for (int i = 0; i < sidePartsAmount; i++)
            {
                SplineData current = orderedSplineData[i];
                SplineData next = orderedSplineData[i+1];
                float currentScale = GetRadius(current);
                float nextScale = GetRadius(next);
                vertexSlotSpacing = _bodySettings.SideVertices / (slotAmounts[i] / 2f);
                nextSlot = -vertexSlotSpacing / 2;

                for (int j = 0; j < _bodySettings.SideVertices; j++)
                {
                    int v1 = vertexIndex;
                    int v2 = vertexIndex + _bodySettings.SideVertices;
                    int v3 = vertexIndex + 1;
                    int v4 = vertexIndex + _bodySettings.SideVertices + 1;

                    float portion = (float)j / (_bodySettings.SideVertices - 1);
                    float y = portion * _bodySettings.SplineSpacing + current.Center.y;
                    float x = Mathf.Lerp(currentScale, nextScale, _bodySettings.WeightCurve.Evaluate(portion));
                    
                    vertices[v1] = new Vector3(-x, y, 0);
                    normals[v1] = new Vector3(-1, 0, 0);
                    vertices[v2] = new Vector3(x, y, 0);
                    normals[v2] = new Vector3(1, 0, 0);

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
                        slots.Add(new BodyPartSlot()
                            { Position = vertices[v1], Rotation = rotation, VertexIndex = v1 });
                        slotIndex++;
                        
                        slotAngle = Vector2.Angle(normals[v2], Vector2.up);
                        factor = vertices[i].x <= 0 ? 1 : -1;
                        rotation = Quaternion.Euler(0, 0, slotAngle * factor + 180);
                        slots.Add(new BodyPartSlot()
                            { Position = vertices[v2], Rotation = rotation, VertexIndex = v2 });
                        slotIndex++;
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
                normals = normals
            };

            return new CreateResult
            {
                Mesh = mesh,
                Slots = slots.ToArray()
            };
        }

        private float GetRadius(SplineData spline)
        {
            return (spline.Size * _bodySettings.ScalePerSize + _bodySettings.MinScale) / 2;
        }

        private class CreateResult
        {
            public Mesh Mesh;
            public BodyPartSlot[] Slots;
        }
    }
}