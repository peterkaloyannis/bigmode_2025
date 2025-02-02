using System;
using System.Collections.Generic;
using UnityEngine;

public struct Stratagem
{
    public List<stratagem_input_t> combo; // The button combo of the stratagem.
    public float cooldown; // The cooldown of the stratagem.
    public List<effect_type_t> effects; // Enumerator designating which effects the stratagem applies.
    public List<float> effect_durations; // Enumerator designating the duration of each effect.
    public AudioClip success_noise; // The sound to play on successful completion of this stratagem combo.

    public Stratagem(
        float _cooldown,
        List<stratagem_input_t> _combo,
        List<effect_type_t> _effects,
        List<float> _effect_durations,
        AudioClip _success_noise) {
            cooldown = _cooldown;
            combo = _combo;
            effects = _effects;
            effect_durations = _effect_durations;
            success_noise = _success_noise;

            // Make sure the effect duratins and effects are the same length.
            Debug.Assert(effects.Count == effect_durations.Count);
        }
};


