using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingSound : MonoBehaviour
{

    private AudioSource source;

    private Vector3 lastPos;
    private float lastVelocity;
    private float velocity;
    public float Velocity { get => velocity; }

    public float speedScaling = 0.1f;

    public bool isPlaying;
    void Start()
    {
        source = GetComponent<AudioSource>();
        lastPos = transform.position;
        source.volume = 0;
        //source.pitch = Random.Range(0.95f, 1.05f); 

    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            velocity = CalculateVelocity();
            source.volume = velocity * speedScaling;

        }
        else
        {
            source.volume = 0f;
        }

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
