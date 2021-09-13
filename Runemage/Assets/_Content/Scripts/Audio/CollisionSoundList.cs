using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollisonSoundList", menuName = "Audio", order = 12)]
public class CollisionSoundList : ScriptableObject
{
    public List<SoundCue_Collision> soundList;
}
