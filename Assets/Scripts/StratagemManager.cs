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

public class StratagemManager : MonoBehaviour
{
    public FightManager fight_manager; // The fight manager.
    public List<stratagem_input_t> current_combo; // The current stratagem combo
    public List<string> stratagem_names; // List of human readable stratagem names.
    public List<float> stratagem_cooldowns; // List of stratagem cooldowns.
    public List<float> stratagem_max_cooldown; // List of the max cooldown of each stratagem.
    public List<float> stratagem_cooldown_timers; // List of stratagem cooldown timers.
    public List<List<stratagem_input_t>> stratagem_combos; // List of the stratagem combos.
    public List<int> stratagem_matches; // List encoding how many stratagem inputs match
                                                                 // the current combo.
    public List<List<effect_type_t>> stratagem_effects; // List encoding which effects are applied by a stratagem.
    public List<List<float>> stratagem_effect_durations; // List of stratagem effect durations.
    public List<AudioClip> stratagem_success_noises;
    public AudioClip invalid_combo_noise; // What to play when combo chain is invalid.
    public AudioClip valid_combo_noise; // What to play on successful combo chain, might be combo specific.

    void Start(){
        // Initiate the combo array
        current_combo = new List<stratagem_input_t>();

        // Load in the stratagems
        // Init the empty list
        stratagem_combos = new List<List<stratagem_input_t>>();
        stratagem_names = new List<string>();
        stratagem_cooldowns = new List<float>();
        stratagem_max_cooldown = new List<float>();
        stratagem_cooldown_timers = new List<float>();
        stratagem_matches = new List<int>();
        stratagem_effects = new List<List<effect_type_t>>();
        stratagem_effect_durations = new List<List<float>>();
        stratagem_success_noises = new List<AudioClip>();

        // Grab the list of available stratagems from the singleton.
        Dictionary<string, Stratagem> available_stratagems = SceneResetter.Instance.available_stratagems;

        // Put something in here, worry about JSON loading later.
        foreach(KeyValuePair<string, Stratagem> entry in available_stratagems){
            Stratagem stratagem = entry.Value;
            string stratagem_name = entry.Key;

            stratagem_effects.Add(stratagem.effects);
            stratagem_effect_durations.Add(stratagem.effect_durations);
            stratagem_names.Add(stratagem_name);
            stratagem_combos.Add(stratagem.combo);
            stratagem_cooldowns.Add(stratagem.cooldown);
            stratagem_max_cooldown.Add(stratagem.max_cooldown);
            stratagem_success_noises.Add(stratagem.success_noise);
            stratagem_cooldown_timers.Add(0f);
            stratagem_matches.Add(0);
        }
    }

    // Update is called once per frame
    void Update(){
        // If the fight is not happening, clear out the buffer
        // and return early.
        if (fight_manager.fight_state != fight_state_t.PLAY) {
            // Clear the current combo
            current_combo.Clear();

            // Clear the stratagem cooldowns.
            set_stratagem_cooldown_timers(0f);
            
            return;
        }

        // We record the number of combo entries in the last cycle.
        // If the number of combos increased, we will play a successful
        // input sound.
        int combos_last_cycle = current_combo.Count;

        // Collect the current combo
        collect_current_combo();

        // If there are no stratagems available,
        // and the user made an input, make a failure noise,
        // clear, and return.
        if (stratagem_names.Count == 0){
            if (combos_last_cycle < current_combo.Count){
                make_combo_noise(invalid_combo_noise);
                current_combo.Clear();
            }
            return;
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
                stratagem_max_cooldown[i]
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
                // Trigger the stratagem by adding the effects.
                fight_manager.add_active_effect(
                    stratagem_effects[i], 
                    stratagem_effect_durations[i]
                );

                // Place the stratagem on cooldwon.
                stratagem_cooldown_timers[i] = stratagem_cooldowns[i];

                // Play the sound effect for completing this stratagem.
                make_combo_noise(stratagem_success_noises[i]);

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
        if (!matches_available) {
            make_combo_noise(invalid_combo_noise);
            current_combo.Clear();
        }
        // If above fails and instead the combo count incremented,
        // play a successful combo noise.
        else if (current_combo.Count > combos_last_cycle){
            make_combo_noise(
                valid_combo_noise,
                1f+(float)current_combo.Count/3
            );
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

    public void make_combo_noise(AudioClip noise, float pitch = 1f) {
        // Play the audio
        GameObject tempAudio = new GameObject("TempAudioSource");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.volume = 0.7f;
        audioSource.priority = 0;
        audioSource.clip = noise;
        audioSource.pitch = pitch;
        audioSource.Play();

        Destroy(tempAudio, noise.length);  // Cleanup after playing
    }

    public void set_stratagem_cooldown_timers(float time) {
        for (int i=0; i<stratagem_cooldown_timers.Count; i++) {
            stratagem_cooldown_timers[i] = time;
        }
    }
}
