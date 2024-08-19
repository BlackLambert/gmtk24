using System;
using UnityEngine;

namespace Game
{
    public class GameReseter : MonoBehaviour
    {
        private void Awake()
        {
            Game.Instance.Restart();
        }
    }
}
