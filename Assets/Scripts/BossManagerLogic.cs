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
    public float boss_dps; // The boss decay rate.
    public float max_pitch_integral; // The point when the pitch integral gives the max pitch.
    public float pitch_integral_decay_time; // The rate of decay of the pitch integral.
    public float base_pitch; // The base pitch of the mashing.
    public float pitch_range; // The range for values spanned by the pitch.
    public fight_state_t fight_state = fight_state_t.INIT; // The boss state.
    public AudioClip ding; // The sound effect for mashing.
    public StratagemManagerLogic stratagem_manager; // The stratagem manager
    public float mash_rush_multiplier; // Mash Rush Multiplier
    public float meter_chunk_power; // Meter chunk
    public float meter_chunk_mash_block_time; // Countdown time (s) to block mashing.
    private float mash_block_countdown; // The actual countdown for meter chunk
    private last_key_pressed_t last_key_pressed = last_key_pressed_t.NONE; // The last key pressed for the mash logic.
    private float pitch_integral = 0f; // An integral of the mashing.

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
                meter = 1f;
                break;

            case fight_state_t.LOST:
                // Idle in LOST
                meter = 0f;
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

        // Keep track of player damage and boss damage separately.
        float meter_additions = 0;
        float meter_subtractions = 0;

        // The meter chunk stratagem is applied here.
        // We check for meter_chunks and then destroy them.
        float mash_addition_modified = mash_addition;
        float boss_dps_modified = boss_dps;
        foreach (effect_type_t effect in stratagem_manager.active_effects){
            // Chunk the bar!
            if (effect == effect_type_t.meter_chunk){
                meter_additions += meter_chunk_power;
            }
            // More mash power!
            else if (effect == effect_type_t.mash_rush){
                mash_addition_modified *= mash_rush_multiplier;
            }
            // No more played damage!
            else if (effect == effect_type_t.mash_block){
                mash_addition_modified *= 0;
            }
            // A lot of boss damage! Negated by breaks.
            else if (effect == effect_type_t.boss_chunk) {
                boss_dps_modified *= 50;
            }
            // No more boss damage!
            else if (effect == effect_type_t.meter_break){
                boss_dps_modified *= 0;
            }
        }

        // Fight the boss every time we alternate keys.
        bool a_is_alternated_to = (last_key_pressed != last_key_pressed_t.A) && Input.GetKeyDown(KeyCode.A);
        bool s_is_alternated_to = (last_key_pressed != last_key_pressed_t.S) && Input.GetKeyDown(KeyCode.S);
        if (a_is_alternated_to){
            last_key_pressed = last_key_pressed_t.A;
            meter_additions += mash_addition_modified;
            if (mash_addition_modified > 0){
                mash_ding();
            }
        }
        else if (s_is_alternated_to){
            last_key_pressed = last_key_pressed_t.S;
            meter_additions += mash_addition_modified;
            if (mash_addition_modified > 0){
                mash_ding();
            }
        }

        // Compute boss damage.
        meter_subtractions = boss_dps_modified * Time.deltaTime;

        // Update the meter.
        // Meter is clamped to [0,1] for other functions to use it safely.
        meter += meter_additions - meter_subtractions;
        meter = Mathf.Clamp(meter, 0f, 1f);

        // Decay the pitch on every frame.
        exponential_pitch_decay();
    }

    void reset_state_update() {
        // In reset, we set the meter to 0.5 again.
        meter = 0.5f;

        // Then we set the state back to init.
        fight_state = fight_state_t.INIT;
    }

    void mash_ding() {
        // Adjust the ding frequency based on
        // the time since the last mash.
        //
        // Pitch is adjusted via linterp from base_pitch to base_pitch + pitch_range.
        float pitch = base_pitch + pitch_range*(pitch_integral / max_pitch_integral);
        pitch = Mathf.Clamp(pitch, base_pitch, base_pitch+pitch_range);

        // Play the audio
        GameObject tempAudio = new GameObject("TempAudioSource");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.volume = 0.6f;
        audioSource.priority = 130;
        audioSource.clip = ding;
        audioSource.pitch = pitch;
        audioSource.Play();

        Destroy(tempAudio, ding.length);  // Cleanup after playing

        // Increment the pitch integral.
        pitch_integral += 1f;
    }

    void exponential_pitch_decay(){
        // Decay as per an exponential loss.
        pitch_integral -= pitch_integral / pitch_integral_decay_time * Time.deltaTime;

        // Clamp the pitch integral to stay within [0, max_pitch_integral]
        pitch_integral = Mathf.Clamp(pitch_integral, 0f, max_pitch_integral);
    }
}
