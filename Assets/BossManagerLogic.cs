using System;
using UnityEngine;
using UnityEngine.AI;

enum last_key_pressed_t {
    NONE,
    A,
    S,
};

public enum fight_state_t {
    INIT,
    PAUSED,
    PLAY,
    WON,
    LOST,
    RESET,
}

public class BossManagerLogic : MonoBehaviour
{
    public float meter; // The boss meter [0,1]
    public float mash_addition; // The amount to add on each mash.
    public float decay_rate; // The boss decay rate.
    public fight_state_t fight_state = fight_state_t.INIT;
    private last_key_pressed_t last_key_pressed = last_key_pressed_t.NONE; // The last key pressed for the mash logic.
    

    // Update is called once per frame
    void Update()
    {
        switch (fight_state)
        {
            case fight_state_t.INIT:
                // Tansition to play in INIT
                fight_state = fight_state_t.PLAY;
                break;

            case fight_state_t.PAUSED:
                // Idle in PAUSED
                // This is where dialogue triggers happen.
                break;

            case fight_state_t.PLAY:
                // Do the boss fight logic loop in PLAY.
                fight_state_update();
                break;

            case fight_state_t.WON:
                // Idle in WON
                break;

            case fight_state_t.LOST:
                // Idle in LOST
                break;

            case fight_state_t.RESET:
                // In reset, we reset the meter and set to init.
                reset_state_update();
                break;

            default:
                // If we get here, bug.
                Debug.Log("[ERROR]: Invalid boss state reached.");
                break;            
        }
    }

    void fight_state_update() {
        // Before evaluating the boss score calculations, check for loss or win.
        if (meter == 1f) {
            fight_state = fight_state_t.WON;
            return;
        }
        else if (meter == 0f){
            fight_state = fight_state_t.LOST;
            return;
        }

        // First, boss burns down the decay rate meter by a rate constant.
        meter -= decay_rate * Time.deltaTime;

        // Fight the boss every time we alternate keys.
        bool a_is_alternated_to = (last_key_pressed != last_key_pressed_t.A) && Input.GetKeyDown(KeyCode.A);
        bool s_is_alternated_to = (last_key_pressed != last_key_pressed_t.S) && Input.GetKeyDown(KeyCode.S);
        if (a_is_alternated_to){
            last_key_pressed = last_key_pressed_t.A;
            meter += mash_addition;
        }
        else if (s_is_alternated_to){
            last_key_pressed = last_key_pressed_t.S;
            meter += mash_addition;
        }

        // Meter is clamped to [0,1] for other functions to use it safely.
        meter = Mathf.Clamp(meter, 0f, 1f);
    }

    void reset_state_update() {
        // In reset, we set the meter to 0.5 again.
        meter = 0.5f;

        // Then we set the state back to init.
        fight_state = fight_state_t.INIT;
    }
}
