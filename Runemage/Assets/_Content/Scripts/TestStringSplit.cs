using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStringSplit : MonoBehaviour
{
    private string spellName = "Fireball_1";
    
    void Start()
    {
        int dividerIndex = spellName.IndexOf("_", StringComparison.Ordinal);
        
        if (dividerIndex >= 0)
        {
        	spellName = spellName.Substring(0, dividerIndex);
        }
		
        Debug.Log("SplitName: " + spellName);
    }
}
