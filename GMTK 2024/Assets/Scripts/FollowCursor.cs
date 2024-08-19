using UnityEngine;

namespace Game
{
    public class FollowCursor : MonoBehaviour
    {
        [SerializeField] private bool _worldSpace = true;
        
        private Camera _camera;
        
        private void Awake()
        {
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        private void Update()
        {
            Vector2 worldPoint = _worldSpace ? _camera.ScreenToWorldPoint(Input.mousePosition) : Input.mousePosition;
            transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
        }
    }
}
