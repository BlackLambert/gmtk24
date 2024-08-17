using UnityEngine;

namespace Game
{
    public class FollowCursor : MonoBehaviour
    {
        private Camera _camera;
        
        private void Awake()
        {
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        private void Update()
        {
            Vector2 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
        }
    }
}
