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

public enum stratagem_types_t {
    mash_rush,
    meter_chunk,
}

public class StratagemManagerLogic : MonoBehaviour
{
    public BossManagerLogic boss_manager; // The boss manager.
    public List<stratagem_input_t> current_combo; // The current stratagem combo
    public Dictionary<stratagem_types_t, string> stratagem_names; // List of human readable stratagem names.
    public Dictionary<stratagem_types_t, List<stratagem_input_t>> stratagem_combos; // List of the stratagem combos.
                                                                                    // There is potential for speedup here
                                                                                    // using binary operations (if needed)
    public Dictionary<stratagem_types_t, int> stratagem_matches; // List encoding how many stratagem inputs match
                                                                 // the current combo.
    public List<stratagem_types_t> active_stratgems;
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

        // Loop through each combo, trigger if we match.
        bool matches_available = false;
        foreach (stratagem_types_t stratagem_type in Enum.GetValues(typeof(stratagem_types_t))){
            // If the stratagem type is not a key in the stratagem names
            // then the gameobject has not been configured. Continue.
            if (!stratagem_names.ContainsKey(stratagem_type)){
                continue;
            }
            List<stratagem_input_t> combo = stratagem_combos[stratagem_type];

            // If the current combo is 0, reset.
            if (current_combo.Count == 0) {
                stratagem_matches[stratagem_type] = 0;
                matches_available = true;
                break;
            }

            // Loop through the combo of the stratagem and check how well
            // we match.
            for (int j=0; j<combo.Count; j++) {
                // If we are longer than the current combo, break.
                if (j >= current_combo.Count) {
                    break;
                }

                // Compare the combo key to the current combo.
                if (current_combo[j] == combo[j]) {
                    // Place the number of stratagem matches
                    // as indices
                    stratagem_matches[stratagem_type] = j+1;
                }
                else {
                    break;
                }
            }

            // Trigger the combo effect.
            if (stratagem_matches[stratagem_type] == stratagem_combos[stratagem_type].Count) {
                // Play the combo success sound.
                make_combo_noise(valid_combo_noise);
                
                // Trigger logic here
                Debug.Log(stratagem_type);
                active_stratgems.Add(stratagem_type);

                // Clear the combo after triggering.
                current_combo.Clear();
                return;
            }

            // Check that a combo is still valid.
            // If at least one combo is valid, return true.
            // Need the count to have a +1 for the zero match case.
            bool combo_still_valid = stratagem_matches[stratagem_type] == current_combo.Count;
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

    void load_stratagem_combos(){
        // Init the empty list
        stratagem_combos = new Dictionary<stratagem_types_t, List<stratagem_input_t>>();
        stratagem_names = new Dictionary<stratagem_types_t, string>();
        stratagem_matches = new Dictionary<stratagem_types_t, int>();

        // Grab all game objects tagged as stratagems.
        GameObject[] stratagem_objects = GameObject.FindGameObjectsWithTag("stratagem");

        // Put something in here, worry about JSON loading later.
        for (int i = 0; i < stratagem_objects.Length; i++){
            // Extract the stratagem information
            Stratagem stratagem = stratagem_objects[i].GetComponent<Stratagem>();
            stratagem_types_t stratagem_type = stratagem.stratagem_type;

            // Check that this stratagem has not already been set.
            // Else, prep the stratagem
            if (stratagem_names.ContainsKey(stratagem_type)){
                Debug.Log("[ERROR] Duplicate stratagem type.");
                continue;
            }   
            stratagem_names[stratagem_type] = stratagem.name;
            stratagem_combos[stratagem_type] = stratagem.combo;
            stratagem_matches[stratagem_type] = 0;
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
        audioSource.clip = noise;
        audioSource.Play();

        Destroy(tempAudio, noise.length);  // Cleanup after playing
    }
}
