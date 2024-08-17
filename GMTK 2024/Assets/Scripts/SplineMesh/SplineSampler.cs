using Unity.Mathematics;
using UnityEngine.Splines;
using UnityEngine;

namespace Game
{
    public class SplineSampler : MonoBehaviour
    {
        [SerializeField]
        private SplineContainer _splineContainer;
        // Start is called before the first frame update
        [SerializeField]
        private int _splineIndex;
        [SerializeField]
        private float _bodyWidth = 1f;

        float3 position;
        float3 forward;
        float3 normal;

        public void SampleSplineVertices(float t, out Vector3 left, out Vector3 right)
        {
            _splineContainer.Evaluate(_splineIndex, t, out position, out forward, out normal);
            float3 distance = Vector3.Cross(forward, normal).normalized * _bodyWidth;
            right = position + distance;
            left = position - distance;
        }
    }
}