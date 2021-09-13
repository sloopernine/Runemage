using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingSound : MonoBehaviour
{

    AudioSource source;

    Vector3 lastPos;
    float lastVelocity;
    float velocity;
    public float Velocity { get => velocity; }

    void Start()
    {
        source = GetComponent<AudioSource>();
        lastPos = transform.position;
        source.volume = 0;
    }

    private void FixedUpdate()
    {
        velocity = CalculateVelocity();
        source.volume = velocity * 0.1f;

    }

    private float CalculateVelocity()
    {
        float v = ((transform.position - lastPos).magnitude) / Time.fixedDeltaTime;
        lastPos = transform.position;
        v = Mathf.SmoothStep(lastVelocity, v, 0.2f);

        lastVelocity = v;
        //print($"Speed: {v}");
        return v;
    }
}
