using UnityEngine;

public class FightAudioManager : MonoBehaviour
{
    private AudioClip[] Loops;
    private AudioSource[] MusicSources;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Loops = Resources.LoadAll<AudioClip>("Sounds/Music/FightScene");
        MusicSources = new AudioSource[Loops.Length];
        foreach (AudioClip loop in Loops){
            GameObject audioObject = new GameObject(loop.name);
            audioObject.transform.SetParent(this.transform);
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            MusicSources[System.Array.IndexOf(Loops, loop)] = audioSource;
            audioSource.clip = loop;
        }

        foreach (AudioSource Music in MusicSources){
            Music.Play();
            Music.loop = true;
            Music.volume = 0f;
        }
    }


    void SelectTrack(int track){
        foreach (AudioSource Music in MusicSources){
            Music.volume = 0f;
        }
        MusicSources[track].volume = 1f;
    }

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
    }
}
