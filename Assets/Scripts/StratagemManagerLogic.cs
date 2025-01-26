using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    private int max_stratagem_combo_length; // Max length of input stratagems, computed at
                                             // startup time.

    void Start(){
        // Initiate the combo array
        current_combo = new List<stratagem_input_t>();

        // Load in the stratagems (TODO)
        load_stratagem_combos();

        // Assign the max_stratagem_combo_length.
        max_stratagem_combo_length = 10;
    }

    // Update is called once per frame
    void Update(){
        // If the fight is not happening, clear out the buffer
        // and return early.
        if (boss_manager.fight_state != fight_state_t.PLAY) {
            current_combo.Clear();
            return;
        }

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

        // Compare the combo list to stratagems.
        compare_combos();

        // Trigger the first combo that matches!
        for (int i=0; i<stratagem_combos.Count; i++){

        }

        // Finally, if the stratagem combo is
        // at the max combo length, clear it out.
        if (current_combo.Count == max_stratagem_combo_length){
            current_combo.Clear();
        }
    }

    void load_stratagem_combos(){
        // Init the empty list
        stratagem_combos = new List<List<stratagem_input_t>>();
        stratagem_matches = new List<int>();

        // Put something in here, worry about JSON loading later.
        stratagem_names.Add("Classic Combo");
        stratagem_matches.Add(0);
        stratagem_combos.Add(
            new List<stratagem_input_t> 
                {stratagem_input_t.UP,
                stratagem_input_t.DOWN,
                stratagem_input_t.LEFT,
                stratagem_input_t.RIGHT}
        );
    }

    void compare_combos(){
        // Loop through each combo 
        for (int i=0; i < stratagem_combos.Count; i++) {
            List<stratagem_input_t> combo = stratagem_combos[i];
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
}
