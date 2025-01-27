using System.Collections.Generic;
using UnityEngine;

public enum stratagem_input_t {
    UP=0,
    RIGHT=1,
    DOWN=2,
    LEFT=3,
}

public class StratagemManagerLogic : MonoBehaviour
{
    public BossManagerLogic boss_manager; // The boss manager.
    public List<stratagem_input_t> current_combo; // The current stratagem combo
    public List<string> stratagem_names; // List of human readable stratagem names.
    public List<List<stratagem_input_t>> stratagem_combos; // List of the stratagem combos.
                                                           // There is potential for speedup here
                                                           // using binary operations (if needed)
    public List<int> stratagem_matches; // List encoding how many stratagem inputs match
                                            // the current combo.
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

        // Compare the combo list to stratagems.
        compare_combos();

        // Trigger the combos.
        trigger_combos();
    }

    void load_stratagem_combos(){
        // Init the empty list
        stratagem_combos = new List<List<stratagem_input_t>>();
        stratagem_matches = new List<int>();

        // Grab all game objects tagged as stratagems.
        GameObject[] stratagem_objects = GameObject.FindGameObjectsWithTag("stratagem");

        // Put something in here, worry about JSON loading later.
        for (int i = 0; i < stratagem_objects.Length; i++){
            Stratagem stratagem = stratagem_objects[i].GetComponent<Stratagem>();
            stratagem_names.Add(stratagem.name);
            stratagem_combos.Add(stratagem.combo);
            
            // Add a blank to the stratagem combo
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

    void compare_combos(){
        // Loop through each combo 
        for (int i=0; i < stratagem_combos.Count; i++) {
            List<stratagem_input_t> combo = stratagem_combos[i];

            // If the current combo is 0, reset.
            if (current_combo.Count == 0) {
                stratagem_matches[i] = 0;
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
                    stratagem_matches[i] = j+1;
                }
            }
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

    void trigger_combos(){
        // Trigger the first combo that matches!
        // We do this by checking that the combo_match == combo_length.
        // We also keep track of if there are no matches left.
        bool matches_available = false;
        for (int i=0; i<stratagem_combos.Count; i++){
            if (stratagem_matches[i] == stratagem_combos[i].Count) {
                // Play the combo success sound.
                make_combo_noise(valid_combo_noise);
                
                // Trigger logic here
                Debug.Log("HI");

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
        if (!matches_available) {
            make_combo_noise(invalid_combo_noise);
            current_combo.Clear();
        }
    }
}
