using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public List<string> move_names; // List of move names for debug.
    public float next_move_countdown; // Countdown timer to each move.
    public float next_move_countdown_timer; // Countdown timer to each move.
    public List<float> move_windup_times; // List of the move windup times.
    public List<float> move_cooldowns; // List of the move cooldowns.
    public List<float> move_cooldown_timers; // List of the move cooldown timers.
    public List<List<effect_type_t>> move_effects; // List of the move effects.
    public List<List<float>> move_effect_durations; // List of the move effect durations.
    public List<AudioClip> move_trigger_noises; // Sound to play on the trigger of a move.
    public List<AudioClip> move_activation_noises; // Sound to play on the activation of a move.
    public int active_move; // The index of the active move. -1 when inactive.
    public float active_move_windup_timer; // The windup timer of the active move.
    public FightManager fight_manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init the empty list
        move_names = new List<string>();
        move_windup_times = new List<float>();
        move_cooldowns = new List<float>();
        move_cooldown_timers = new List<float>();
        move_effects = new List<List<effect_type_t>>();
        move_effect_durations = new List<List<float>>();
        move_trigger_noises = new List<AudioClip>();
        move_activation_noises = new List<AudioClip>();

        // Grab all game objects tagged as moves.
        GameObject[] move_objects = GameObject.FindGameObjectsWithTag("boss_move");

        // Put something in here, worry about JSON loading later.
        for (int i = 0; i < move_objects.Length; i++){
            // Extract the move information
            BossMove move = move_objects[i].GetComponent<BossMove>();
   
            move_effects.Add(move.effects);
            move_windup_times.Add(move.windup_time);
            move_effect_durations.Add(move.effect_durations);
            move_names.Add(move.name);
            move_trigger_noises.Add(move.trigger_noise);
            move_activation_noises.Add(move.activation_noise);
            move_cooldowns.Add(move.cooldown);
            move_cooldown_timers.Add(0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the game state is not fight, clear the active moves.
        if (fight_manager.fight_state != fight_state_t.PLAY) {
            // Active move changes.
            active_move = -1;
            active_move_windup_timer = 0f;

            // Clear the move cooldowns.
            set_move_cooldown_timers(0f);

            return;
        }

        // Update the cooldowns of all the moves.
        for (int i=0; i<move_names.Count; i++) {
            // Evolve the cooldown timer.
            float cooldown_timer =  move_cooldown_timers[i];
            cooldown_timer = Mathf.Clamp(
                cooldown_timer - Time.deltaTime,
                0,
                600
            );
            move_cooldown_timers[i] = cooldown_timer;
        }

        // Run the logic to advance the present boss move.
        if (active_move != -1){
            // Set the time to next move to the max.
            next_move_countdown_timer = next_move_countdown;

            // Update the windup timer.
            active_move_windup_timer = Mathf.Clamp(
                active_move_windup_timer - Time.deltaTime,
                0f,
                600f
            );

            // If the active move dropes to 0.
            Debug.Assert(active_move < move_names.Count, "[ERROR] Active boss move is not a valid boss move index.");
            if (active_move_windup_timer == 0f) {
                // Add the move effect
                fight_manager.add_active_effect(
                    move_effects[active_move],
                    move_effect_durations[active_move]
                );

                // Play the move sound.
                make_combo_noise(move_activation_noises[active_move]);

                // Reset the boss to the idle state.
                // Timer should already be at 0.
                active_move = -1;
            }

            // No need to run the move selection algorithm if a move is active.
            return;
        }

        // When there is no active move, evolve the move timer.
        next_move_countdown_timer = Mathf.Clamp(
                next_move_countdown_timer - Time.deltaTime,
                0f,
                next_move_countdown
            );
        if (next_move_countdown_timer == 0f){
            Debug.Log("BOSS MOVE");
            // Loop through all the moves, recording the indices
            // of available moves.
            List<int> valid_moves = new List<int>();
            for (int i=0; i<move_cooldown_timers.Count; i++){
                if (move_cooldown_timers[i] ==0){
                    valid_moves.Add(i);
                }
            }

            // If the list is of length 0, return,
            int n_valid_moves = valid_moves.Count;
            if (n_valid_moves==0){
                return;
            }

            // Else, draw a random number. Multiply it by the list of
            // valid moves, and then floor it to get the move index.
            // Small subtraction offset to prevent bad flooring.
            float randomFloat = UnityEngine.Random.Range(0f, ((float)n_valid_moves)-0.001f);
            int selected_move_index = Mathf.FloorToInt(randomFloat);
            Debug.Assert(selected_move_index < move_names.Count, "[ERROR] Bad boss move index.");

            // Execute the move and play the activation noise.
            active_move = selected_move_index;
            active_move_windup_timer = move_windup_times[selected_move_index];
            make_combo_noise(move_trigger_noises[selected_move_index]);
        }

        return;
    }

    public void set_move_cooldown_timers(float time) {
        for (int i=0; i<move_cooldown_timers.Count; i++) {
            move_cooldown_timers[i] = time;
        }
    }

    void make_combo_noise(AudioClip noise, float pitch = 1f) {
        // Play the audio
        GameObject tempAudio = new GameObject("TempAudioSource");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.volume = 0.6f;
        audioSource.priority = 130;
        audioSource.clip = noise;
        audioSource.pitch = pitch;
        audioSource.Play();

        Destroy(tempAudio, noise.length);  // Cleanup after playing
    }

}
