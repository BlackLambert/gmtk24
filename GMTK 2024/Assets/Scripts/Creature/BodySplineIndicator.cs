using UnityEngine;

namespace Game
{
    public class BodySplineIndicator : MonoBehaviour
    {
        [SerializeField] private Transform _indicator;
        
        public void SetPosition(Vector3 position)
        {
            _indicator.position = position;
        }
    }
}
