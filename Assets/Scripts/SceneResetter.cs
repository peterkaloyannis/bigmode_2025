using UnityEngine;

public class SceneResetter : MonoBehaviour
{
    private FightAudioManager fightAudioManager;
    private Arms arms;
    public bool Wife;
    public int track;
    private FightManager fightManager;
    public float Mash_addition = 0f;
    public Transform ThoriumRush;
    public bool ThoriumRushActive = false;
    public Transform CapacitorSlam;
    public bool CapacitorSlamActive = false;
    public Transform ObsidianBreaks;
    public bool ObsidianBreaksActive = false;
    public Transform CartridgeChange;
    public bool CartridgeChangeActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fightAudioManager = GameObject.Find("AudioManager").GetComponent<FightAudioManager>();
        arms = GameObject.Find("Arms").GetComponent<Arms>();
        fightManager = GameObject.Find("LogicManager").GetComponent<FightManager>();
        ResetAll();
    }

    public void ResetAll()
    {
        SetStratagems();
        ResetMusic(track);
        ResetBoss();
        ResetEnv(Wife);
    }

    void SetStratagems()
    {
        if (ThoriumRushActive)
        {
            GameObject prefab = Instantiate(ThoriumRush.gameObject, fightManager.transform);
        }
        if (CapacitorSlamActive)
        {
            GameObject prefab = Instantiate(CapacitorSlam.gameObject, fightManager.transform);
        }
        if (ObsidianBreaksActive)
        {
            GameObject prefab = Instantiate(ObsidianBreaks.gameObject, fightManager.transform);
        }
        if (CartridgeChangeActive)
        {
            GameObject prefab = Instantiate(CartridgeChange.gameObject, fightManager.transform);
        }
    }

    void ResetBoss()
    {
        fightManager.fight_state = fight_state_t.INIT;
        fightManager.active_effects.Clear();
        fightManager.active_effect_timers.Clear();
        fightManager.meter = 0.5f;
        fightManager.mash_addition = Mash_addition;
    }

    void ResetMusic(int track)
    {
        fightAudioManager.ResetAll(track);
    }

    void ResetEnv(bool Wife)
    {
        arms.isWife = Wife;
        arms.updateScene();
    }
}
