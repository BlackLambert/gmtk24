using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class BestStageDisplay : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _text;

        [SerializeField] private string _baseText = "Best Stage {0}";

        private void Start()
        {
            int stage = PlayerPrefs.HasKey("Stage") ? PlayerPrefs.GetInt("Stage") : 0;
            _text.text = string.Format(_baseText, stage.ToString());
        }
    }
}
