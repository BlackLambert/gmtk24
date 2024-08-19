using UnityEngine;

namespace Game
{
    public class SetCameraBackgroundColor : MonoBehaviour
    {
        [SerializeField] private Color _color;

        private Camera _camera;
        
        private void Awake()
        {
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        private void Start()
        {
            _camera.backgroundColor = _color;
        }
    }
}
