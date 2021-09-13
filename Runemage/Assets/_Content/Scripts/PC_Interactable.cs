using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PC_Interactable : MonoBehaviour
{


    public abstract void Release(Vector3 force);

    public abstract void Grab(Transform parent);



}
