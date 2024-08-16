using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BodyPartsMenu : MonoBehaviour
    {
        [SerializeField] 
        private BodyPartsTab[] _tabs;
        
        [SerializeField] 
        private BodyPartType _startTab;

        private void Start()
        {
            Select(_startTab);
            foreach (BodyPartsTab tab in _tabs)
            {
                tab.OnClick += OnTabSelected;
            }
        }

        private void OnDestroy()
        {
            foreach (BodyPartsTab tab in _tabs)
            {
                tab.OnClick -= OnTabSelected;
            }
        }

        private void OnTabSelected(BodyPartType type)
        {
            Select(type);
        }

        private void Select(BodyPartType type)
        {
            foreach (BodyPartsTab tab in _tabs)
            {
                tab.SetInteractable(tab.Type != type);
            }
        }
    }
}
