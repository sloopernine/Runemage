using System;
using UnityEngine;

namespace Singletons
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public bool gestureTrainingMode;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
    }
}