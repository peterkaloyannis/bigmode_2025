using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Define all the fight scenes.

enum fight_scene_t {
    wife_0,
    wife_1,
    wife_2,
    wife_3,
    boss_0_wins,
    boss_1_wins,
    boss_2_wins,
};

public class SceneResetter : MonoBehaviour
{
    // The available boss moves to be set before scene transitions.
    public Dictionary<string, Stratagem> available_stratagems = new Dictionary<string, Stratagem>();
    public Dictionary<string, BossMove> available_boss_moves = new Dictionary<string, BossMove>();

    // Boss Noises
    public AudioClip meter_chunk_boss_trigger_noise;
    public AudioClip meter_chunk_boss_activation_noise;
    public AudioClip stratagem_block_boss_trigger_noise;
    public AudioClip stratagem_block_boss_activation_noise;

    // Stratagem Noises
    public AudioClip capacitor_slam_noise;
    public AudioClip obsidian_breaks_noise;
    public AudioClip thorium_rush_noise;
    public AudioClip cartridge_change_slam_noise;

    // Keeping track of the boss moves and stratagems.
    private Dictionary<string, BossMove> boss_moves;
    private Dictionary<string, Stratagem> stratagems;

    public static SceneResetter Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start(){
        // Create the list of boss moves.
        boss_moves = new Dictionary<string, BossMove>() {
            {"Boss Chunk", new BossMove(
                // Cooldown
                20f,
                // Windup Time
                5f,
                // Effects
                new List<effect_type_t>{effect_type_t.boss_chunk},
                // Effect durations
                new List<float>{0f},
                // Trigger Noise
                meter_chunk_boss_trigger_noise,
                // Activation Noise
                meter_chunk_boss_activation_noise
            )},
            {"Stratagem Block", new BossMove(
                // Cooldown
                20f,
                // Windup Time
                5f,
                // Effects
                new List<effect_type_t>{effect_type_t.stratagem_block},
                // Effect durations
                new List<float>{0f},
                // Trigger Noise
                stratagem_block_boss_trigger_noise,
                // Activation Noise
                stratagem_block_boss_activation_noise
            )}
        };

        // Create the list of stratagems.
        stratagems = new Dictionary<string, Stratagem>() {
            {"Capacitor Slam", new Stratagem(
                // Cooldown
                5f,
                // Combo
                new List<stratagem_input_t>(){
                    stratagem_input_t.DOWN,
                    stratagem_input_t.RIGHT,
                    stratagem_input_t.DOWN},
                // Effects
                new List<effect_type_t>{
                    effect_type_t.meter_chunk,
                    effect_type_t.mash_block},
                // Effect durations
                new List<float>{0f, 3f},
                // Trigger Noise
                capacitor_slam_noise
            )},
            {"Obsidian Breaks", new Stratagem(
                // Cooldown
                4f,
                // Combo
                new List<stratagem_input_t>(){
                    stratagem_input_t.LEFT,
                    stratagem_input_t.UP,
                    stratagem_input_t.RIGHT},
                // Effects
                new List<effect_type_t>{effect_type_t.meter_break},
                // Effect durations
                new List<float>{1f},
                // Trigger Noise
                obsidian_breaks_noise
            )},
            {"Thorium Rush", new Stratagem(
                // Cooldown
                5f,
                // Combo
                new List<stratagem_input_t>(){
                    stratagem_input_t.UP,
                    stratagem_input_t.UP,
                    stratagem_input_t.RIGHT},
                // Effects
                new List<effect_type_t>{effect_type_t.mash_rush},
                // Effect durations
                new List<float>{4f},
                // Trigger Noise
                thorium_rush_noise
            )},
            {"Cartridge Change", new Stratagem(
                // Cooldown
                5f,
                // Combo
                new List<stratagem_input_t>(){
                    stratagem_input_t.RIGHT,
                    stratagem_input_t.DOWN,
                    stratagem_input_t.LEFT},
                // Effects
                new List<effect_type_t>{effect_type_t.cooldown_refresh},
                // Effect durations
                new List<float>{0f},
                // Trigger Noise
                cartridge_change_slam_noise
            )},
        };
    }

    void setup_fight_scene(fight_scene_t fight_scene) {
        // Get the list of available stratagems from achievements
        // TODO -- For now we will use the complete set.
        available_stratagems = stratagems;

        // Switch case
        switch (fight_scene){
            case fight_scene_t.wife_0:
                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the boss parameters
                // TODO

                break;
            case fight_scene_t.wife_1:
                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the boss parameters
                // TODO
                break;
            case fight_scene_t.wife_2:
                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the boss parameters
                // TODO
                break;
            case fight_scene_t.wife_3:
                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the boss parameters
                // TODO
                break;
            case fight_scene_t.boss_0_wins:
                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the boss parameters
                // TODO
                break;
            case fight_scene_t.boss_1_wins:
                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>(){
                    {"Boss_Chunk", boss_moves["Boss_Chunk"]}
                };

                // Set the boss parameters
                // TODO
                break;
            case fight_scene_t.boss_2_wins:
                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>(){
                    {"Boss_Chunk", boss_moves["Boss_Chunk"]},
                    {"Stratagem Block", boss_moves["Stratagem Block"]}
                };

                // Set the boss parameters
                // TODO
                break;
            default:
                Debug.Log("[ERROR]: Invalid Fight State.");
                break;
        }
    }

}
