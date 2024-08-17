using UnityEngine;

namespace Game
{
    public class SetMeshRendererColor : MonoBehaviour
    {
        [SerializeField] private Color _color;

        [SerializeField] private MeshRenderer _meshRenderer;

        private void Start()
        {
            Material material = _meshRenderer.material;
            material.color = new Color(_color.r, _color.g, _color.b, material.color.a);
        }
    }
}
