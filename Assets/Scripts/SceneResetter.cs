using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Define all the fight scenes.

public enum fight_scene_t {
    wife_0,
    wife_1,
    wife_2,
    wife_3,
    boss_0_wins_0_losses,
    boss_0_wins_1_losses,
    boss_1_wins_0_losses,
    boss_1_wins_1_losses,
    boss_secret,
    toMenu,
    endGame
};

public class SceneResetter : MonoBehaviour
{
    // The available boss moves to be set before scene transitions.
    public Dictionary<string, Stratagem> available_stratagems = new Dictionary<string, Stratagem>();
    public Dictionary<string, BossMove> available_boss_moves = new Dictionary<string, BossMove>();

    // Phase dependent boss DPS
    public float boss_dps;

    // Is this a wife scene for rendering.
    public bool is_wife = false;

    // Dialogue outputs.
    public string fname_pre_fight_dialogue ="";
    public string fname_fight_win_dialogue ="";
    public string fname_fight_lose_dialogue ="";

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
    public fight_scene_t next_fight_W;
    public fight_scene_t next_fight_L;
    public fight_scene_t current_fight;
    public string fname_prefight_monologue = null;
    public string fname_postfight_monologue_W = null;
    public string fname_postfight_monologue_L = null;
    public bool fadeOut = false;
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
                600f,
                // Windup Time
                2f,
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
                // Max Cooldown.
                600f,
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
                new List<float>{0f, 2f},
                // Trigger Noise
                capacitor_slam_noise
            )},
            {"Obsidian Breaks", new Stratagem(
                // Cooldown
                2f,
                // Max Cooldown.
                600f,
                // Combo
                new List<stratagem_input_t>(){
                    stratagem_input_t.LEFT,
                    stratagem_input_t.UP,
                    stratagem_input_t.RIGHT},
                // Effects
                new List<effect_type_t>{effect_type_t.meter_break},
                // Effect durations
                new List<float>{1.5f},
                // Trigger Noise
                obsidian_breaks_noise
            )},
            {"Thorium Rush", new Stratagem(
                // Cooldown
                5f,
                // Max Cooldown.
                600f,
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
                0f,
                // Max Cooldown.
                0f,
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

    public void setup_fight_scene(fight_scene_t fight_scene) {
        // Get the list of available stratagems from achievements
        get_active_stratagems_from_achievements();
        int highest_achievement = GameManager.Instance.getHighestAchievement();
        if (highest_achievement == 5){
            highest_achievement = 4;
        }
        // Switch case
        switch (fight_scene){
            case fight_scene_t.wife_0:
                // Set iswife
                is_wife = true;
                current_fight = fight_scene_t.wife_0;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue= GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_start.json");
                fname_prefight_monologue = GameManager.Instance.setPathDialogue("intro_" + highest_achievement.ToString() + "_internal_dialogue.json");
                fname_fight_win_dialogue=GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_win.json");;
                fname_postfight_monologue_W = GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_internal_dialogue.json");;
                fname_fight_lose_dialogue="";
                fname_postfight_monologue_L = "";

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.wife_0;
                next_fight_W = fight_scene_t.boss_0_wins_0_losses;

                // Set the boss parameters
                boss_dps = 0.03f;

                break;
            case fight_scene_t.wife_1:
                // Set iswife
                is_wife = true;
                current_fight = fight_scene_t.wife_1;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue= GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_start.json");
                fname_prefight_monologue = GameManager.Instance.setPathDialogue("intro_" + highest_achievement.ToString() + "_internal_dialogue.json");
                fname_fight_win_dialogue=GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_win.json");;
                fname_postfight_monologue_W = GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_internal_dialogue.json");;
                fname_fight_lose_dialogue="";
                fname_postfight_monologue_L = "";

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.wife_1;
                next_fight_W = fight_scene_t.boss_0_wins_0_losses;

                // Set the boss parameters
                boss_dps = 0.03f;

                break;
            case fight_scene_t.wife_2:
                // Set iswife
                is_wife = true;
                current_fight = fight_scene_t.wife_2;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue= GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_start.json");
                fname_prefight_monologue = GameManager.Instance.setPathDialogue("intro_" + highest_achievement.ToString() + "_internal_dialogue.json");
                fname_fight_win_dialogue=GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_win.json");;
                fname_postfight_monologue_W = GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_internal_dialogue.json");;
                fname_fight_lose_dialogue="";
                fname_postfight_monologue_L = "";

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.wife_2;
                next_fight_W = fight_scene_t.boss_0_wins_0_losses;

                // Set the boss parameters
                boss_dps = 0.03f;

                break;
            case fight_scene_t.wife_3:
                // Set iswife
                is_wife = true;
                current_fight = fight_scene_t.wife_3;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue= GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_start.json");
                fname_prefight_monologue = GameManager.Instance.setPathDialogue("intro_" + highest_achievement.ToString() + "_internal_dialogue.json");
                fname_fight_win_dialogue=GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_win.json");;
                fname_postfight_monologue_W = GameManager.Instance.setPathDialogue("wife_" + highest_achievement.ToString() + "_internal_dialogue.json");;
                fname_fight_lose_dialogue="";
                fname_postfight_monologue_L = "";

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.wife_3;
                next_fight_W = fight_scene_t.boss_0_wins_0_losses;

                // Set the boss parameters
                boss_dps = 0.03f;

                break;
            case fight_scene_t.boss_0_wins_0_losses:
                // Set iswife
                is_wife = false;
                current_fight = fight_scene_t.boss_0_wins_0_losses;

                fname_pre_fight_dialogue=GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_start.json");
                fname_prefight_monologue = "";

                fname_fight_win_dialogue="in_between_rounds_W.json";
                fname_postfight_monologue_W = "";

                fname_fight_lose_dialogue="in_between_rounds_L.json";
                fname_postfight_monologue_L = "";

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.boss_0_wins_1_losses;
                next_fight_W = fight_scene_t.boss_1_wins_0_losses;

                // Set the boss parameters
                boss_dps = 0.06f;

                break;
            case fight_scene_t.boss_1_wins_0_losses:
                // Set iswife
                is_wife = false;
                current_fight = fight_scene_t.boss_1_wins_0_losses;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue="";
                fname_prefight_monologue = "";

                fname_fight_win_dialogue=GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_win.json");
                fname_postfight_monologue_W = "";

                fname_fight_lose_dialogue="in_between_rounds_L.json";
                fname_postfight_monologue_L = "";

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>(){
                    {"Boss Chunk", boss_moves["Boss Chunk"]}
                };

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.boss_1_wins_1_losses;
                next_fight_W = fight_scene_t.toMenu;

                // Set the boss parameters
                boss_dps = 0.09f;
                break;
            case fight_scene_t.boss_0_wins_1_losses:
                // Set iswife
                is_wife = false;
                current_fight = fight_scene_t.boss_0_wins_1_losses;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue="";
                fname_prefight_monologue = "";

                fname_fight_win_dialogue="in_between_rounds_W.json";
                fname_postfight_monologue_W = "";

                fname_fight_lose_dialogue= GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_win_internal_dialogue.json");
                fname_postfight_monologue_L = GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_lose_internal_dialogue.json");

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>();

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.toMenu;
                next_fight_W = fight_scene_t.boss_1_wins_1_losses;

                // Set the boss parameters
                boss_dps = 0.06f;
                break;
            case fight_scene_t.boss_1_wins_1_losses:
                // Set iswife
                is_wife = false;
                current_fight = fight_scene_t.boss_1_wins_1_losses;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue="";
                fname_prefight_monologue = "";

                fname_fight_win_dialogue=GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_win.json");
                fname_postfight_monologue_W = GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_win_internal_dialogue.json");

                fname_fight_lose_dialogue=GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_lose.json");
                fname_postfight_monologue_L = GameManager.Instance.setPathDialogue("boss_0_fight_" + highest_achievement.ToString() + "_lose_internal_dialogue.json");

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>(){
                    {"Boss Chunk", boss_moves["Boss Chunk"]}
                };

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.toMenu;
                next_fight_W = fight_scene_t.toMenu;

                // Set the boss parameters
                boss_dps = 0.09f;
                break;

            case fight_scene_t.boss_secret:
                // Set iswife
                is_wife = false;
                current_fight = fight_scene_t.boss_secret;

                // Set the file paths for each dialogue tree.
                fname_pre_fight_dialogue="";
                fname_prefight_monologue = "";

                fname_fight_win_dialogue=GameManager.Instance.setPathDialogue("boss_1_fight_" + highest_achievement.ToString() + "_win.json");
                fname_postfight_monologue_W = GameManager.Instance.setPathDialogue("boss_1_fight_" + highest_achievement.ToString() + "_win_internal_dialogue.json");

                fname_fight_lose_dialogue=GameManager.Instance.setPathDialogue("boss_1_fight_" + highest_achievement.ToString() + "_lose.json");
                fname_postfight_monologue_L = GameManager.Instance.setPathDialogue("boss_1_fight_" + highest_achievement.ToString() + "_lose_internal_dialogue.json");

                // Set the boss moves.
                available_boss_moves = new Dictionary<string, BossMove>(){
                    {"Boss Chunk", boss_moves["Boss Chunk"]},
                    {"Stratagem Block", boss_moves["Stratagem Block"]}
                };

                // Set the next fight scenes.
                next_fight_L = fight_scene_t.toMenu;
                next_fight_W = fight_scene_t.endGame;

                // Set the boss parameters
                boss_dps = 0.12f;
                break;
            case fight_scene_t.toMenu:
                break;
            case fight_scene_t.endGame:
                break;
            default:
                Debug.Log("[ERROR]: Invalid Fight State.");
                break;
        }
    }

    void get_active_stratagems_from_achievements() {
        // Create the available stratagems dictionary.
        available_stratagems = new Dictionary<string, Stratagem>();

        // We will always have capacitor slam.
        available_stratagems["Capacitor Slam"] = stratagems["Capacitor Slam"];

        // Check all achievements and have them then add the corresponding stratagem.
        if (PlayerPrefs.GetInt("Ach1")==1){
            available_stratagems["Obsidian Breaks"] = stratagems["Obsidian Breaks"];
        }
        if (PlayerPrefs.GetInt("Ach2")==1){
            available_stratagems["Thorium Rush"] = stratagems["Thorium Rush"];
        }
        if (PlayerPrefs.GetInt("Ach3")==1){
            available_stratagems["Cartridge Change"] = stratagems["Cartridge Change"];
        }
    }

}
