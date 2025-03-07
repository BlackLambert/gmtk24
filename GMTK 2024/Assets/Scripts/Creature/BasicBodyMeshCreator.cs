using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class BasicBodyMeshCreator : MonoBehaviour
    {
        [SerializeField] private Body _body;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private PolygonCollider2D _meshCollider;
        [SerializeField] private float _length = 1;
        [SerializeField] private float _width = 1;
        [SerializeField] private int _numberOfRoundSideVertices = 12;

        private void Awake()
        {
            _meshFilter.mesh = CreateMesh();
            _meshCollider.points = _meshFilter.mesh.vertices.Select(v => (Vector2)v).ToArray();
        }

        private Mesh CreateMesh()
        {
            Vector3[] vertices = new Vector3[_numberOfRoundSideVertices + _numberOfRoundSideVertices];
            float halfRadius = _width / 2;
            float halfLength = _length / 2;

            Vector3 lengthAddition = new Vector3(0, halfLength, 0);
            float angleDelta = Mathf.PI / (_numberOfRoundSideVertices - 1);

            int[] triangles = new int[(vertices.Length - 2) * 3];
            Vector4[] tangents = new Vector4[vertices.Length];
            Vector3[] normals = new Vector3[vertices.Length];
            
            for (int i = 0; i < _numberOfRoundSideVertices; i++)
            {
                float angle = angleDelta * i;
                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                Vector3 unitCirclePoint = new Vector3(cos, sin, 0);
                Vector3 circlePoint = unitCirclePoint * halfRadius;
                vertices[i] = circlePoint + lengthAddition;
                tangents[i] = new Vector4(unitCirclePoint.y, unitCirclePoint.x, 0, 0);
                normals[i] = new Vector3(unitCirclePoint.x, unitCirclePoint.y, 0);

                if (i > 0 && i < _numberOfRoundSideVertices - 1)
                {
                    int j = (i - 1) * 3;
                    triangles[j] = i - 1;
                    triangles[j + 1] = _numberOfRoundSideVertices - 1;
                    triangles[j + 2] = i;
                }
            }
            
            for (int i = 0; i < _numberOfRoundSideVertices; i++)
            {
                float angle = angleDelta * i + Mathf.PI;
                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                Vector3 unitCirclePoint = new Vector3(cos, sin, 0);
                Vector3 circlePoint = unitCirclePoint * halfRadius;
                vertices[i + _numberOfRoundSideVertices] = circlePoint - lengthAddition;
                tangents[i + _numberOfRoundSideVertices] = new Vector4(unitCirclePoint.y, unitCirclePoint.x, 0, 0);
                normals[i + _numberOfRoundSideVertices] = new Vector3(unitCirclePoint.x, unitCirclePoint.y, 0);
                
                if (i > 0 && i < _numberOfRoundSideVertices - 1)
                {
                    int j = (_numberOfRoundSideVertices + i - 3) * 3;
                    triangles[j] = i + _numberOfRoundSideVertices - 1;
                    triangles[j + 1] = _numberOfRoundSideVertices + _numberOfRoundSideVertices - 1;
                    triangles[j + 2] = i + _numberOfRoundSideVertices;
                }
            }

            int tI = triangles.Length - 6;
            triangles[tI] = 0;
            triangles[tI + 1] = _numberOfRoundSideVertices + _numberOfRoundSideVertices - 1;
            triangles[tI + 2] = _numberOfRoundSideVertices;
            
            triangles[tI + 3] = 0;
            triangles[tI + 4] = _numberOfRoundSideVertices;
            triangles[tI + 5] = _numberOfRoundSideVertices - 1;

            Mesh mesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                tangents = tangents,
                normals = normals
            };

            return mesh;
        }
    }
}
