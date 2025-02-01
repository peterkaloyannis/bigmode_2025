using System;
using System.Collections.Generic;
using UnityEngine;

public class BossMove:MonoBehaviour
{
    public float cooldown; // The cooldown of the boss move.
    public float windup_time; // The time between triggering and activation of the boss move.
    public List<effect_type_t> effects; // Enumerator designating which effects the boss move applies.
    public List<float> effect_durations; // Enumerator designating the duration of each effect.
    public AudioClip trigger_noise; // The sound to play on triggering of this boss move.
    public AudioClip activation_noise; // The sound to play on activation of this boss move.

    void Start(){
        // Make sure the number of effect durations is the same
        // as the number of effects.
        Debug.Assert(effect_durations.Count == effects.Count);
    }
};