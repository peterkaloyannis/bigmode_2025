using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource Source1;
    public AudioSource Source2;
    private List<AudioSource> Sources;
    public AudioClip TransitionMusic;
    public AudioClip MenuMusic;
    public AudioClip TitleMusic;

    // Other
    public bool looping = false;
    private AudioClip loopingClip;


    void Awake()
    {
        Sources = new List<AudioSource>();
        Sources.Add(Source1);
        Source1.volume = 0.5f;
        Sources.Add(Source2);
        Source2.volume = 0.5f;
    }

    public int trackSwitch = 0;
    private double goalTime = 0f;
    public void startLoopClip(AudioClip clip, double initGoalTime)
    {
        Sources[trackSwitch].clip = clip;
        Sources[trackSwitch].PlayScheduled(AudioSettings.dspTime + initGoalTime);

        goalTime = AudioSettings.dspTime + initGoalTime + (double)(clip.samples) / clip.frequency;
        looping = true;
        loopingClip = clip;
    }

    public void switchTrack()
    {
        trackSwitch = (trackSwitch + 1) % 2;
    }

    public void startClip(AudioClip clip, double initGoalTime = 0)
    {
        Sources[trackSwitch].clip = clip;
        Sources[trackSwitch].PlayScheduled(AudioSettings.dspTime + initGoalTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (looping){
            if (AudioSettings.dspTime > goalTime - 1)
            {
                switchTrack();
                Sources[(trackSwitch)%2].clip = loopingClip;
                Sources[(trackSwitch)%2].PlayScheduled(goalTime);
                goalTime = goalTime + (double)(loopingClip.samples) / loopingClip.frequency;
            }
        }
    }
}
