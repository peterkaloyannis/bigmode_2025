using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum stratagem_input_t {
    UP=0,
    RIGHT=1,
    DOWN=2,
    LEFT=3,
}

// These are the effects.
public enum effect_type_t {
    mash_rush,
    meter_break,
    meter_chunk,
    mash_block,
    cooldown_refresh,
    stratagem_block,
    boss_chunk,
}

public class StratagemManagerLogic : MonoBehaviour
{
    public BossManagerLogic boss_manager; // The boss manager.
    public List<stratagem_input_t> current_combo; // The current stratagem combo
    public List<string> stratagem_names; // List of human readable stratagem names.
    public List<float> stratagem_cooldowns; // List of stratagem cooldowns.
    public List<float> stratagem_cooldown_timers; // List of stratagem cooldown timers.
    public List<List<stratagem_input_t>> stratagem_combos; // List of the stratagem combos.
    public List<int> stratagem_matches; // List encoding how many stratagem inputs match
                                                                 // the current combo.
    public List<List<effect_type_t>> stratagem_effects; // List encoding which effects are applied by a stratagem.
    public List<List<float>> stratagem_effect_durations; // List of stratagem effect durations.
    public List<effect_type_t> active_effects; // The list of active stratagems.
    public List<float> active_effect_timers; // The activation timers of each active stratagem.
    public AudioClip invalid_combo_noise; // What to play when combo chain is invalid.
    public AudioClip valid_combo_noise; // What to play on successful combo chain, might be combo specific.

    void Start(){
        // Initiate the combo array
        current_combo = new List<stratagem_input_t>();

        // Load in the stratagems (TODO)
        load_stratagem_combos();
    }

    // Update is called once per frame
    void Update(){
        // If the fight is not happening, clear out the buffer
        // and return early.
        if (boss_manager.fight_state != fight_state_t.PLAY) {
            current_combo.Clear();
            return;
        }

        // Collect the current combo
        collect_current_combo();

        // Handle effects for stratagems before the effects
        // are emptied so 1 frame effects can be applied.
        foreach (effect_type_t effect in active_effects) {
            if (effect == effect_type_t.cooldown_refresh){
                list_fill<float>(stratagem_cooldown_timers, 0f);
            }
            if (effect == effect_type_t.stratagem_block){
                list_fill<float>(stratagem_cooldown_timers, 600f);
            }
        }

        // Loop through the active effects to check if they
        // are still valid. We loop through backwards to prevent
        // index shifting after removal.
        //
        // We also check if the lists stay synced in size.
        Debug.Assert(active_effects.Count == active_effect_timers.Count);
        for (int i=active_effects.Count - 1; i>=0; i--) {
            // Update the effect timer.
            active_effect_timers[i] -= Time.deltaTime;

            // If this effect has a negative effect timer, mark it for removal.
            if (active_effect_timers[i] < 0f){
                active_effects.RemoveAt(i);
                active_effect_timers.RemoveAt(i);
            }
        }

        // Loop through each combo, trigger if we match.
        bool matches_available = false;
        for (int i=0; i<stratagem_names.Count; i++) {
            List<stratagem_input_t> combo = stratagem_combos[i];

            // Evolve the cooldown counter of the stratagem.
            float cooldown_timer =  stratagem_cooldown_timers[i];
            cooldown_timer = Mathf.Clamp(
                cooldown_timer - Time.deltaTime,
                0,
                600
            );
            stratagem_cooldown_timers[i] = cooldown_timer;

            // If the current combo has 0 length, set the number
            // of matches to 0 and return early.
            if (current_combo.Count == 0) {
                stratagem_matches[i] = 0;
                matches_available = true;
            }

            // Loop through the combo of the stratagem and check how well
            // we match.
            for (int j=0; j<combo.Count; j++) {
                // If we are longer than the current combo or the combo
                // is on cooldown, break.
                if (j >= current_combo.Count || cooldown_timer > 0f) {
                    break;
                }

                // Compare the combo key to the current combo.
                if (current_combo[j] == combo[j]) {
                    // Place the number of stratagem matches
                    // as indices
                    // Debug.Log("SETTING 3");
                    stratagem_matches[i] = j+1;
                }
                else {
                    break;
                }
            }

            // Trigger the combo effect.
            if (stratagem_matches[i] == stratagem_combos[i].Count) {
                // Play the combo success sound.
                make_combo_noise(valid_combo_noise);
                
                // Trigger the stratagem by adding the effects.
                Debug.Log(stratagem_names[i]);
                active_effects.AddRange(stratagem_effects[i]);
                active_effect_timers.AddRange(stratagem_effect_durations[i]);

                // Place the stratagem on cooldwon.
                stratagem_cooldown_timers[i] = stratagem_cooldowns[i];

                // Clear the combo after triggering.
                current_combo.Clear();
                return;
            }

            // Check that a combo is still valid.
            // If at least one combo is valid, return true.
            // Need the count to have a +1 for the zero match case.
            bool combo_still_valid = stratagem_matches[i] == current_combo.Count;
            matches_available = matches_available || combo_still_valid;
        }

        // If no matching stratagems remain, clear the buffer.
        // If this happens, we wanna play a die sound effect.
        // The location of this trigger is probably temporary.
        // This sound is bugged for some reason... Spawns 100 of them.
        if (!matches_available) {
            make_combo_noise(invalid_combo_noise);
            current_combo.Clear();
        }
    }

    void list_fill<T>(List<T> list, T value){
        for (int i=0; i<list.Count; i++) {
            list[i] = value;
        }
    }

    void load_stratagem_combos(){
        // Init the empty list
        stratagem_combos = new List<List<stratagem_input_t>>();
        stratagem_names = new List<string>();
        stratagem_cooldowns = new List<float>();
        stratagem_cooldown_timers = new List<float>();
        stratagem_matches = new List<int>();
        stratagem_effects = new List<List<effect_type_t>>();
        stratagem_effect_durations = new List<List<float>>();

        // Grab all game objects tagged as stratagems.
        GameObject[] stratagem_objects = GameObject.FindGameObjectsWithTag("stratagem");

        // Put something in here, worry about JSON loading later.
        for (int i = 0; i < stratagem_objects.Length; i++){
            // Extract the stratagem information
            Stratagem stratagem = stratagem_objects[i].GetComponent<Stratagem>();
   
            stratagem_effects.Add(stratagem.effects);
            stratagem_effect_durations.Add(stratagem.effect_durations);
            stratagem_names.Add(stratagem.name);
            stratagem_combos.Add(stratagem.combo);
            stratagem_cooldowns.Add(stratagem.cooldown);
            stratagem_cooldown_timers.Add(0f);
            stratagem_matches.Add(0);
        }
    }

    void collect_current_combo() {
        // Add the current keystroke to the combo list.
        // We use an if statement to prevent mulitple keys
        // being pressed in a single frame.
        //
        // First, check if the rest key is pressed
        // and empty the stratagem buffer.
        if (Input.GetKeyDown(KeyCode.Space)) {
            current_combo.Clear();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            current_combo.Add(stratagem_input_t.UP);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            current_combo.Add(stratagem_input_t.RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            current_combo.Add(stratagem_input_t.DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            current_combo.Add(stratagem_input_t.LEFT);
        }
    }


    void make_combo_noise(AudioClip noise) {
        // Play the audio
        GameObject tempAudio = new GameObject("TempAudioSource");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.volume = 0.6f;
        audioSource.priority = 130;
        audioSource.clip = noise;
        audioSource.Play();

        Destroy(tempAudio, noise.length);  // Cleanup after playing
    }
}
