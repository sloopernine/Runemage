using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyOnePlayer : MonoBehaviour
{
    public static OnlyOnePlayer Instance;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
