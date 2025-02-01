using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class FightAudioManager : MonoBehaviour
{
    private AudioClip[] Loops;
    private AudioSource[] MusicSources;
    private List<double> goalTime;
    private double initGoalTime = 0f;
    public AudioMixerGroup mixerGroup;
    public FightManager fight_manager;
    private bool fighting = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Loops = Resources.LoadAll<AudioClip>("Sounds/Music/FightScene");
        MusicSources = new AudioSource[2*Loops.Length];
        initGoalTime = AudioSettings.dspTime + 1;
        goalTime = new List<double>();
        for (int i=0; i<Loops.Length; i++){goalTime.Add(AudioSettings.dspTime + 1 + (double)(Loops[0].samples) / Loops[0].frequency);}

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

        SelectTrack(0);
    }


    void SelectTrack(int track){
        for (int i = 0; i < Loops.Length; i++){
            MusicSources[2*i].volume = 0f;
            MusicSources[2*i+1].volume = 0f;
        }
        MusicSources[2*track].volume = 1f;
        MusicSources[2*track+1].volume = 1f;
    }

    void toggle_hp_filter()
    {
        foreach (AudioSource MusicSource in MusicSources){
            MusicSource.bypassEffects = !MusicSource.bypassEffects;
            MusicSource.outputAudioMixerGroup.audioMixer.SetFloat("HPCutoff", MusicSource.bypassEffects ? 0f : 1476f);
        }
    }

    int osc;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            SelectTrack(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)){
            SelectTrack(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)){
            SelectTrack(2);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)){
            SelectTrack(3);
        } else if (Input.GetKeyDown(KeyCode.Alpha5)){
            SelectTrack(4);
        }

        if (fighting){
            if (fight_manager.fight_state == fight_state_t.PAUSED || fight_manager.fight_state == fight_state_t.WON || fight_manager.fight_state == fight_state_t.LOST){
                fighting = false;
                toggle_hp_filter();
            }
        } else {
            if (fight_manager.fight_state == fight_state_t.PLAY || fight_manager.fight_state == fight_state_t.INIT){
                fighting = true;
                toggle_hp_filter();
            }
        }

        for (int i=0; i<goalTime.Count; i++){
            if (AudioSettings.dspTime > goalTime[i] - 1)
            {
                if (MusicSources[2*i].isPlaying){
                    osc = 1;
                } else {
                    osc = 0;
                }
                MusicSources[2*System.Array.IndexOf(Loops, Loops[i])+osc].clip = Loops[i];
                MusicSources[2*System.Array.IndexOf(Loops, Loops[i])+osc].PlayScheduled(goalTime[i]);
                goalTime[i] = goalTime[i] + (double)(Loops[0].samples) / Loops[0].frequency;
            }
        }
    }
}
