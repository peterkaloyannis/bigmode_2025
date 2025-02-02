using System;
using System.Collections.Generic;
using UnityEngine;

public struct BossMove
{
    public float cooldown; // The cooldown of the boss move.
    public float windup_time; // The time between triggering and activation of the boss move.
    public List<effect_type_t> effects; // Enumerator designating which effects the boss move applies.
    public List<float> effect_durations; // Enumerator designating the duration of each effect.
    public AudioClip trigger_noise; // The sound to play on triggering of this boss move.
    public AudioClip activation_noise; // The sound to play on activation of this boss move.

    public BossMove(
        float _cooldown, 
        float _windup_time, 
        List<effect_type_t> _effects,
        List<float> _effect_durations,
        AudioClip _trigger_noise,
        AudioClip _activation_noise) {
            cooldown = _cooldown;
            windup_time = _windup_time;
            effects = _effects;
            effect_durations = _effect_durations;
            trigger_noise = _trigger_noise;
            activation_noise = _activation_noise;

            // Make sure the effect duratins and effects are the same length.
            Debug.Assert(effects.Count == effect_durations.Count);
        }
};