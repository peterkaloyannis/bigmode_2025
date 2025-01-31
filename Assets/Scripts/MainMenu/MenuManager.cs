using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public AudioManager audioManager;
    public Transform TitleScreenContainer;
    private bool titleScreenActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTitleScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (titleScreenActive && Input.GetKeyDown(KeyCode.Space))
        {
            audioManager.startClip(audioManager.TransitionMusic, 0.01);
            TitleScreenContainer.gameObject.SetActive(false);
            titleScreenActive = false;
            audioManager.switchTrack();
            audioManager.startLoopClip(audioManager.MenuMusic, 0.01 + (double)audioManager.TransitionMusic.samples / audioManager.TransitionMusic.frequency);
        }
    }

    void startTitleScreen()
    {
        audioManager.startLoopClip(audioManager.TitleMusic, 0.01);
        TitleScreenContainer.gameObject.SetActive(true);
        titleScreenActive = true;
    }
}
