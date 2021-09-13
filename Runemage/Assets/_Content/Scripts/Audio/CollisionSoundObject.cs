using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundObject : MonoBehaviour
{

    [SerializeField] CollisionSoundMaterial material;
    private static Dictionary<Collider, CollisionSoundObject> SoundObjects = new Dictionary<Collider, CollisionSoundObject>();

    private Collider[] colliders;

    private void Awake()
    {

        colliders = GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            SoundObjects[colliders[i]] = this;
        }
      
    }

    private void OnDestroy()
    {       
        for (int i = 0; i < colliders.Length; i++)
        {
            SoundObjects.Remove(colliders[i]);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CollisionSoundController.Instance == null)
        {
            return;
        }

        Collider collider = collision.collider;
        if (SoundObjects.ContainsKey(collider))
        {
            CollisionSoundObject collisionSoundObject = SoundObjects[collider];

            float volume = CalculateImpactVolume(collision);
            if (volume < CollisionSoundController.Instance.MinCollisionVolume)
            {
                return;
            }
      
            Vector3 position = collision.GetContact(0).point;
            CollisionSoundController.Instance.Play(material, position, volume);
            CollisionSoundController.Instance.Play(collisionSoundObject.material, position, volume);

        }
    }

    private float CalculateImpactVolume(Collision collision)
    {
        float Volume;
        //Debug.Log("Velocity: " + Collision.relativeVelocity.magnitude.ToString());
        Volume = CubicEaseOut(collision.relativeVelocity.magnitude);
        return Volume;
    }

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="velocity">Current time in seconds.</param>
    /// <param name="startingValue">Starting value.</param>
    /// <param name="changeInValue">Change in value.</param>
    /// <param name="maxCollisionVelocity">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseOut(float velocity, float startingValue = 0, float changeInValue = 1)
    {
        return changeInValue * ((velocity = velocity / CollisionSoundController.Instance.MaxCollisionVelocity - 1) * velocity * velocity + 1) + startingValue;
    }

}
