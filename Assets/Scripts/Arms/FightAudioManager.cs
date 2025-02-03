using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class FightAudioManager : MonoBehaviour
{
    private AudioClip[] Loops;
    private AudioSource[] MusicSources;
    private List<double> goalTime;
    private double initGoalTime = 0f;
    public AudioMixerGroup mixerGroup;
    public FightManager fight_manager;
    private bool fighting = false;
    private bool shouldBePlaying = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Loops = Resources.LoadAll<AudioClip>("Sounds/Music/FightScene");
        MusicSources = new AudioSource[2*Loops.Length];

        foreach (AudioClip loop in Loops){
            GameObject audioObject = new GameObject(loop.name);
            audioObject.transform.SetParent(this.transform);
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = mixerGroup;
            MusicSources[2*System.Array.IndexOf(Loops, loop)] = audioSource;
            AudioSource audioSource2 = audioObject.AddComponent<AudioSource>();
            audioSource2.outputAudioMixerGroup = mixerGroup;
            MusicSources[2*System.Array.IndexOf(Loops, loop)+1] = audioSource2;
            audioSource.clip = loop;
            MusicSources[2*System.Array.IndexOf(Loops, loop)].PlayScheduled(initGoalTime);

            MusicSources[2*System.Array.IndexOf(Loops, loop)].volume = 0f;
            MusicSources[2*System.Array.IndexOf(Loops, loop)+1].volume = 0f;
        }
    }



    void StartMusic(double initGoalTime){
        foreach (AudioClip loop in Loops){
            MusicSources[2*System.Array.IndexOf(Loops, loop)].PlayScheduled(initGoalTime);
        }
    }


    void SelectTrack(int track){
        for (int i = 0; i < Loops.Length; i++){
            MusicSources[2*i].volume = 0f;
            MusicSources[2*i+1].volume = 0f;
        }
        MusicSources[2*track].volume = 0.5f;
        MusicSources[2*track+1].volume = 0.5f;
    }

    public void ResetAll(int track){
        foreach (AudioSource MusicSource in MusicSources){
            MusicSource.Stop();
        }
        
        goalTime = new List<double>();
        for (int i=0; i<Loops.Length; i++){goalTime.Add(AudioSettings.dspTime + 0.1 + (double)(Loops[i].samples) / Loops[i].frequency);}
        
        StartMusic(AudioSettings.dspTime + 0.1);
        SelectTrack(track);
        shouldBePlaying = true;
    }


    IEnumerator ToggleHpFilterGradually(float duration)
    {
        bool enableFilter = MusicSources[0].bypassEffects; // Check current state
        float startValue = enableFilter ? 0f : 1476f;
        float endValue = enableFilter ? 1476f : 0f;
        
        foreach (AudioSource MusicSource in MusicSources)
        {
            MusicSource.bypassEffects = !enableFilter;
        }

        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            foreach (AudioSource MusicSource in MusicSources)
            {
                MusicSource.outputAudioMixerGroup.audioMixer.SetFloat("HPCutoff", value);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final value is set
        foreach (AudioSource MusicSource in MusicSources)
        {
            MusicSource.outputAudioMixerGroup.audioMixer.SetFloat("HPCutoff", endValue);
        }
    }
    int osc;
    // Update is called once per frame
    void Update()
    {
        if (!shouldBePlaying && fight_manager.fight_state != fight_state_t.INIT){
            shouldBePlaying = true;
            if (SceneResetter.Instance.is_wife){
                ResetAll(5);
            } else {
                if (SceneResetter.Instance.current_fight == fight_scene_t.boss_0_wins_0_losses)
                {
                    ResetAll(0);
                } else if (SceneResetter.Instance.current_fight == fight_scene_t.boss_1_wins_0_losses)
                {
                    ResetAll(1);
                } else if (SceneResetter.Instance.current_fight == fight_scene_t.boss_1_wins_0_losses)
                {
                    ResetAll(2);
                } else if (SceneResetter.Instance.current_fight == fight_scene_t.boss_1_wins_1_losses)
                {
                    ResetAll(3);
                } else if (SceneResetter.Instance.current_fight == fight_scene_t.boss_0_wins_1_losses)
                {
                    ResetAll(3);
                } else if (SceneResetter.Instance.current_fight == fight_scene_t.boss_secret)
                {
                    ResetAll(4);
                }
            }
        }

        if (shouldBePlaying){
            if (fighting){
                if (fight_manager.fight_state == fight_state_t.PAUSED || fight_manager.fight_state == fight_state_t.WON || fight_manager.fight_state == fight_state_t.LOST){
                    fighting = false;
                    // toggle_hp_filter();
                    StartCoroutine(ToggleHpFilterGradually(0.5f));
                }
            } else {
                if (fight_manager.fight_state == fight_state_t.PLAY || fight_manager.fight_state == fight_state_t.INIT){
                    fighting = true;
                    // toggle_hp_filter();
                    StartCoroutine(ToggleHpFilterGradually(0.5f));
                }
            }

            for (int i=0; i<goalTime.Count; i++){
                if (AudioSettings.dspTime > goalTime[i] - 2)
                {
                    if (MusicSources[2*i].isPlaying){
                        osc = 1;
                    } else {
                        osc = 0;
                    }
                    MusicSources[2*System.Array.IndexOf(Loops, Loops[i])+osc].clip = Loops[i];
                    MusicSources[2*System.Array.IndexOf(Loops, Loops[i])+osc].PlayScheduled(goalTime[i]);
                    goalTime[i] = goalTime[i] + (double)(Loops[i].samples) / Loops[i].frequency;
                }
            }
        }
    }
}
