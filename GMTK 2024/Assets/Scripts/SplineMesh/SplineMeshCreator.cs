using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Game
{
    public class SplineMeshCreator : MonoBehaviour
    {
        [SerializeField]
        SplineSampler _splineSampler;

        [SerializeField]
        private int _samplesPerSpline = 10;

        [SerializeField]
        MeshFilter _meshFilter;

        private List<Vector3> _verticesLeft;
        private List<Vector3> _verticesRight;
        
        // Start is called before the first frame update
        private void OnEnable()
        {
            Spline.Changed += OnSplineChanged;
            BuildMesh();
        }

        private void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }

        private void GetVertices()
        {
            _verticesLeft = new List<Vector3>();
            _verticesRight = new List<Vector3>();
            float step = 1f / (_samplesPerSpline - 1);
            for (int i = 0; i < _samplesPerSpline; i++)
            {
                float t = step * i;
                _splineSampler.SampleSplineVertices(t, out Vector3 l, out Vector3 r);
                _verticesLeft.Add(l);
                _verticesRight.Add(r);
            }
        }

        private void BuildMesh()
        {
            GetVertices();
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            int length = _verticesLeft.Count;

            for (int i = 1; i < length; i++)
            {
                Vector3 p1 = _verticesRight[i-1];
                Vector3 p2 = _verticesLeft[i-1];
                Vector3 p3 = _verticesRight[i];
                Vector3 p4 = _verticesLeft[i];

                int offset = 4 * (i-1);
                int t1 = offset;
                int t2 = offset + 2;
                int t3 = offset + 3;
                int t4 = offset + 3;
                int t5 = offset + 1;
                int t6 = offset;

                vertices.AddRange(new List<Vector3>{p1, p2, p3, p4});
                triangles.AddRange(new List<int>{t1, t2, t3, t4, t5, t6});
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            _meshFilter.mesh = mesh;
        }

        private void RebuildMesh()
        {
            BuildMesh();
        }

        private void OnSplineChanged(Spline spline, int arg2, SplineModification modification)
        {
            RebuildMesh();
        }
    }
}