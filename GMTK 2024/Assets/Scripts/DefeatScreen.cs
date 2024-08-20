using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DefeatScreen : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMPro.TextMeshProUGUI _callOutText;
        [SerializeField] string[] strings;
        [SerializeField] string complainString;

        [SerializeField] AudioSource defeatAudio;
        float _animationDuration = 0.5f;
        bool isAnimating = false;
        float t = 0;
        // Start is called before the first frame update
        void Start()
        {
            defeatAudio.Play();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isAnimating) return;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            t += Time.deltaTime / _animationDuration;
            if (t >= 1)
            {
                canvasGroup.alpha = 1;
                isAnimating = false;
            }
        }

        public void Die()
        {
            canvasGroup.alpha = 0;
            int i = Random.Range(0, strings.Length);
            _callOutText.text = strings[i];
            gameObject.SetActive(true);
            isAnimating = true;
            t = 0;
        }

        public void Restart()
        {

        }

        public void ReturnToMainMenu()
        {

        }

        public void Complain()
        {
            _callOutText.text = complainString; 
        }
    }
}
