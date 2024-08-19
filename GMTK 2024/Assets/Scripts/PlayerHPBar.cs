using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PlayerHPBar : MonoBehaviour
    {
        [SerializeField] Image fill_R;
        [SerializeField] Image fill_L;
        [SerializeField] Image background_R;
        [SerializeField] Image background_L;
        float _animationDuration = 0.5f;
        bool isAnimating = false;
        float oldPercentage;
        float targetPercentage;
        float t = 0;


        public void Update()
        {
            if (!isAnimating) return;
            background_R.fillAmount = Mathf.Lerp(oldPercentage, targetPercentage, t);
            background_L.fillAmount = background_R.fillAmount;
            t += Time.deltaTime / _animationDuration;
            if (t >= 1)
            {
                background_R.fillAmount = background_R.fillAmount - 0.1f;
                background_L.fillAmount = background_R.fillAmount;
                isAnimating = false;
            }

        }
        public void UpdateHP(float percentage)
        {
            oldPercentage = fill_R.fillAmount;
            fill_R.fillAmount = percentage;
            fill_L.fillAmount = percentage;
            targetPercentage = percentage;
            isAnimating = true;
            t = 0;
        }
    }
}
