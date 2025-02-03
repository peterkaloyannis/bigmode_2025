using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
    WON_AFTER_DIALOGUE,
    LOST,
    LOST_AFTER_DIALOGUE,
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

public class FightManager : MonoBehaviour
{
    public float meter; // The fight meter [0,1]
    public List<effect_type_t> active_effects; // The list of active effects.
    public List<float> active_effect_timers; // The timers of each active effect.
    public float mash_addition; // The amount to add on each mash.
    public float max_pitch_integral; // The point when the pitch integral gives the max pitch.
    public float pitch_integral_decay_time; // The rate of decay of the pitch integral.
    public float base_pitch; // The base pitch of the mashing.
    public float pitch_range; // The range for values spanned by the pitch.
    public fight_state_t fight_state = fight_state_t.INIT; // The boss state.
    public AudioClip ding; // The sound effect for mashing.
    public StratagemManager stratagem_manager; // The stratagem manager
    public float mash_rush_multiplier; // Mash Rush Multiplier
    public float meter_chunk_power; // Meter chunk
    public GameObject dialogue_prefab;
    public Transform UI_transform;
    private last_key_pressed_t last_key_pressed = last_key_pressed_t.NONE; // The last key pressed for the mash logic.
    private float pitch_integral = 0f; // An integral of the mashing.
    private GameObject dialogue_object;
    public AudioClip start_play_audio;
    private bool is_dialogue_running = false;
    private bool is_inner_dialogue_running = false;
    private bool is_inner_dialogue_done = false;
    private AsyncOperation operation;

    // Update is called once per frame
    void Update()
    {
        switch (fight_state)
        {
            case fight_state_t.INIT:
            {
                if (!is_inner_dialogue_done){
                    is_inner_dialogue_done = play_inner_dialogue(SceneResetter.Instance.fname_prefight_monologue);
                } else {
                    is_inner_dialogue_done = true;
                }
                if (is_inner_dialogue_done){
                    if (!SceneResetter.Instance.fadeOut){
                        SceneResetter.Instance.fadeOut = true;
                    }
                    // Tansition to play in INIT
                    meter= 0.5f;
                    clear_active_effects();

                    bool is_dialogue_done = play_dialogue(SceneResetter.Instance.fname_pre_fight_dialogue);

                    if (is_dialogue_done){
                        is_inner_dialogue_done = false;
                        stratagem_manager.make_combo_noise(start_play_audio);
                        fight_state = fight_state_t.PLAY;
                    }
                }
                break;
            }
                
            case fight_state_t.PAUSED:
                // Idle in PAUSED
                // This is where dialogue triggers happen.
                break;

            case fight_state_t.PLAY:
                // Do the boss fight logic loop in PLAY.
                fight_state_update();
                break;

            case fight_state_t.WON:
            {
                // Idle in WON
                clear_active_effects();
                meter = 1f;

                // Play win dialogue
                bool is_dialogue_done = play_dialogue(SceneResetter.Instance.fname_fight_win_dialogue);
                if (is_dialogue_done){
                    fight_state = fight_state_t.WON_AFTER_DIALOGUE;
                }

                break;
            }  
            case fight_state_t.WON_AFTER_DIALOGUE:
                SceneResetter.Instance.fadeOut = false;
                if (!is_inner_dialogue_done){
                    is_inner_dialogue_done = play_inner_dialogue(SceneResetter.Instance.fname_postfight_monologue_W);
                    if (is_inner_dialogue_done){
                        loadNextFight(true);
                    }
                } else {
                    loadNextFight(true);
                }
                break;

            case fight_state_t.LOST:
            {
                // Idle in LOST
                clear_active_effects();
                meter = 0f;

                // Play lose dialogue
                bool is_dialogue_done = play_dialogue(SceneResetter.Instance.fname_fight_lose_dialogue);
                if (is_dialogue_done){
                    fight_state = fight_state_t.LOST_AFTER_DIALOGUE;
                }
                break;
            }

            case fight_state_t.LOST_AFTER_DIALOGUE:
                SceneResetter.Instance.fadeOut = false;
                if (!is_inner_dialogue_done){
                    is_inner_dialogue_done = play_inner_dialogue(SceneResetter.Instance.fname_postfight_monologue_L);
                    if (is_inner_dialogue_done){
                        loadNextFight(false);
                    }
                } else {
                    loadNextFight(false);
                }
                break;

            default:
                // If we get here, bug.
                Debug.Assert(false, "[ERROR]: Invalid boss state reached.");
                break;            
        }
    }


    string nextSceneString;
    void loadNextFight(bool won = true)
    {
        if (won){
            if (SceneResetter.Instance.next_fight_W == fight_scene_t.toMenu){
                foreach (string ach in GameManager.Instance.achsNames){
                    Debug.Log(PlayerPrefs.GetInt(ach));
                }
                if (PlayerPrefs.GetInt("Ach2") > 0){
                    SceneResetter.Instance.setup_fight_scene(fight_scene_t.boss_secret);
                    nextSceneString = "FightScene";
                } else {
                    nextSceneString = "MainMenu";
                    MainMenuSingleton.Instance.state = 2;
                }
            } else if (SceneResetter.Instance.next_fight_W == fight_scene_t.endGame){
                nextSceneString = "MainMenu";
                MainMenuSingleton.Instance.state = 4;
            } else {
                SceneResetter.Instance.setup_fight_scene(SceneResetter.Instance.next_fight_W);
                nextSceneString = "FightScene";
            }
        } else {
            if (SceneResetter.Instance.next_fight_L == fight_scene_t.toMenu){
                if (PlayerPrefs.GetInt("Ach2") > 0){
                    MainMenuSingleton.Instance.state = 3;
                } else {
                    MainMenuSingleton.Instance.state = 1;
                }
                nextSceneString = "MainMenu";
            } else {
                SceneResetter.Instance.setup_fight_scene(SceneResetter.Instance.next_fight_L);
                nextSceneString = "FightScene";
            }
        }
        SceneResetter.Instance.fadeOut = false;
        operation = SceneManager.LoadSceneAsync(nextSceneString);
        operation.allowSceneActivation = true;
        fight_state = fight_state_t.INIT;
        is_inner_dialogue_done = false;
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
        float boss_dps_modified = SceneResetter.Instance.boss_dps;
        bool break_bool=false;
        foreach (effect_type_t effect in active_effects){
            switch (effect) {
                case effect_type_t.meter_chunk:
                    meter_additions += meter_chunk_power;
                    break;
                case effect_type_t.mash_rush:
                    mash_addition_modified *= mash_rush_multiplier;
                    break;
                case effect_type_t.mash_block:
                    mash_addition_modified *= 0;
                    break;
                case effect_type_t.boss_chunk:
                    boss_dps_modified += meter_chunk_power / Time.deltaTime;
                    break;
                case effect_type_t.meter_break:
                    break_bool = true;
                    break;
                case effect_type_t.cooldown_refresh:
                    stratagem_manager.set_stratagem_cooldown_timers(0f);
                    break;
                case effect_type_t.stratagem_block:
                    stratagem_manager.set_stratagem_cooldown_timers(600f);
                    break;
                default:
                    Debug.Assert(false, "[ERROR]: Unhandled effect in active effects.");
                    break;
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
        meter_subtractions += boss_dps_modified * Time.deltaTime;

        // Apply the breaks
        if (break_bool){
            meter_subtractions = 0;
        }

        // Update the meter.
        // Meter is clamped to [0,1] for other functions to use it safely.
        meter += meter_additions - meter_subtractions;
        meter = Mathf.Clamp(meter, 0f, 1f);

        // Decay the pitch on every frame.
        exponential_pitch_decay();

        // Active effects are updated at the end of the frame.
        update_active_effects();
    }

    void update_active_effects(){
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
    }

    public void add_active_effect(List<effect_type_t> effects, List<float> durations) {
        active_effects.AddRange(effects);
        active_effect_timers.AddRange(durations);
    }

    public void clear_active_effects() {
        active_effect_timers.Clear();
        active_effects.Clear();
    }

    bool play_dialogue(string fname) {
        if (fname == ""){
            is_dialogue_running = false;
            return true;
        }
        // Play pre fight dialogue
        if (!is_dialogue_running){
            dialogue_object = Instantiate(dialogue_prefab, UI_transform);
            Dialogue dialogue_script = dialogue_object.GetComponent<Dialogue>();
            dialogue_script.dialogueFile = fname;
            dialogue_object.gameObject.SetActive(true);
            is_dialogue_running = true;
        }
        
        // Check if the game object has destroyed itself.
        if (dialogue_object == null && is_dialogue_running){
            is_dialogue_running = false;
            return true;
        }

        return false;
    }

    bool play_inner_dialogue(string fname) {
        if (fname == ""){
            Debug.Log("No inner dialogue");
            is_inner_dialogue_running = false;
            return true;
        }
        // Play pre fight dialogue
        if (!is_inner_dialogue_running){
            dialogue_object = Instantiate(dialogue_prefab, UI_transform);
            Dialogue dialogue_script = dialogue_object.GetComponent<Dialogue>();
            dialogue_script.dialogueFile = fname;
            dialogue_object.gameObject.SetActive(true);
            is_inner_dialogue_running = true;
        }
        
        // Check if the game object has destroyed itself.
        if (dialogue_object == null && is_inner_dialogue_running){
            is_inner_dialogue_running = false;
            return true;
        }

        return false;
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
