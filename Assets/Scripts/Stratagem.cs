using System;
using System.Collections.Generic;
using UnityEngine;

public class Stratagem:MonoBehaviour
{
    public List<stratagem_input_t> combo; // The button combo of the stratagem.
    public float cooldown; // The cooldown of the stratagem.
    public List<effect_type_t> effects; // Enumerator designating which effects the stratagem applies.
    public List<float> effect_durations; // Enumerator designating the duration of each effect.

    void Start(){
        // Make sure the number of effect durations is the same
        // as the number of effects.
        Debug.Assert(effect_durations.Count == effects.Count);
    }
};


