using UnityEngine;

public class FightAudioManager : MonoBehaviour
{
    private AudioClip[] Loops;
    private AudioSource[] MusicSources;
    private double goalTime = 0f;
    private double initGoalTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Loops = Resources.LoadAll<AudioClip>("Sounds/Music/FightScene");
        MusicSources = new AudioSource[2*Loops.Length];
        initGoalTime = AudioSettings.dspTime + 1;
        goalTime = AudioSettings.dspTime + 1 + (double)(Loops[0].samples) / Loops[0].frequency;
        foreach (AudioClip loop in Loops){
            GameObject audioObject = new GameObject(loop.name);
            audioObject.transform.SetParent(this.transform);
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            MusicSources[2*System.Array.IndexOf(Loops, loop)] = audioSource;
            AudioSource audioSource2 = audioObject.AddComponent<AudioSource>();
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

        if (AudioSettings.dspTime > goalTime - 1)
        {
            if (MusicSources[0].isPlaying){
                osc = 1;
            } else {
                osc = 0;
            }

            foreach (AudioClip loop in Loops){
                MusicSources[2*System.Array.IndexOf(Loops, loop)+osc].clip = loop;
                MusicSources[2*System.Array.IndexOf(Loops, loop)+osc].PlayScheduled(goalTime);
            }
            goalTime = goalTime + (double)(Loops[0].samples) / Loops[0].frequency;
        }
    }
}
