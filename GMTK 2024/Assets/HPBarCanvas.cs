using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HPBarCanvas : MonoBehaviour
    {
        [SerializeField] Image fill_R;
        [SerializeField] Image fill_L;
        IDamageable attachedParent;

        private void Update()
        {
            if (attachedParent.Transform == null) return;
            transform.position = attachedParent.Transform.position + (Vector3.up * 2);
        }

        public void UpdateHP(float percentage)
        {
            fill_R.fillAmount = percentage;
            fill_L.fillAmount = percentage;
        }

        public void Init(IDamageable parent)
        {
            fill_R.fillAmount = 0;
            fill_L.fillAmount = 0;
            attachedParent = parent;
        }
    }
}
