using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ShowAfterSeconds : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private float _time = 10;

        private bool _shown = false;
        private float _timeToShow;

        private void Awake()
        {
            _target.SetActive(false);
        }

        private void Start()
        {
            _timeToShow = Time.time + _time;
        }

        private void Update()
        {
            if (!_shown && Time.time >= _timeToShow)
            {
                _shown = true;
                _target.SetActive(true);
            }
        }
    }
}
