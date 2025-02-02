using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public AudioManager audioManager;
    private Transform TitleScreen;
    private Transform Cinema;
    private Transform Buttons;
    private bool titledisappear = false;
    private float countDown = 2f;
    private float trackerCountDown = 0f;
    private float flicker = 0f;
    private TMPro.TextMeshProUGUI pressStartText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Cinema = GameObject.Find("Cinema").transform;
        TitleScreen = GameObject.Find("TitleScreen").transform;
        Buttons = GameObject.Find("Buttons").transform;
        Buttons.gameObject.SetActive(false);
        pressStartText = GameObject.Find("PressStart").GetComponent<TMPro.TextMeshProUGUI>();
        startTitleScreen();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && !titledisappear)
        {
            if (Cinema.gameObject.activeSelf)
            {
                Cinema.gameObject.SetActive(false);

            }
            else if (TitleScreen.gameObject.activeSelf)
            {
                audioManager.startClip(audioManager.TransitionMusic, 0.01);
                audioManager.switchTrack();
                audioManager.startLoopClip(audioManager.MenuMusic, 0.01 + (double)audioManager.TransitionMusic.samples / audioManager.TransitionMusic.frequency);
                titledisappear = true;
            }
        }

        if (titledisappear)
        {
            trackerCountDown += Time.deltaTime;
            flicker += Time.deltaTime;
            if (flicker > 0.1f)
            {
                flicker = 0f;
                pressStartText.alpha = 1-pressStartText.alpha;
            }
            if (trackerCountDown > countDown)
            {
                TitleScreen.gameObject.SetActive(false);
                Buttons.gameObject.SetActive(true);
            }
        }
    }

    void startTitleScreen()
    {
        audioManager.startLoopClip(audioManager.TitleMusic, 0.01);
        TitleScreen.gameObject.SetActive(true);
    }
}
