using UnityEngine;
using UnityEngine.UI;

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
    public GameObject dialogue_prefab;
    private GameObject dialogue_object;
    private int n_dialogues = 0;
    private TMPro.TextMeshProUGUI pressStartText;
    public Sprite winImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Cinema = GameObject.Find("Cinema").transform;
        TitleScreen = GameObject.Find("TitleScreen").transform;
        Buttons = GameObject.Find("Buttons").transform;
        Buttons.gameObject.SetActive(false);
        pressStartText = GameObject.Find("PressStart").GetComponent<TMPro.TextMeshProUGUI>();
        
        if (MainMenuSingleton.Instance.state == 0)
        {
            // Spawn a dialogue object.
            dialogue_object = Instantiate(dialogue_prefab, Cinema);
            Dialogue dialogue_script = dialogue_object.GetComponent<Dialogue>();
            dialogue_script.dialogueFile = "credits.json";
            dialogue_object.gameObject.SetActive(true);
        }

        if (MainMenuSingleton.Instance.state == 4)
        {
            Cinema.Find("Image").GetComponent<Image>().sprite = winImage;
            Cinema.Find("Image").GetComponent<Image>().color = Color.white;
        } else {
            Cinema.Find("Image").GetComponent<Image>().sprite = null;
            Cinema.Find("Image").GetComponent<Image>().color = Color.black;
        }

        startTitleScreen();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogue_object != null){
            return;
        }
        else if (dialogue_object == null && n_dialogues == 0){
            n_dialogues ++;
            
            // Spawn the second dialogue.
            dialogue_object = Instantiate(dialogue_prefab, Cinema);
            Dialogue dialogue_script = dialogue_object.GetComponent<Dialogue>();
            dialogue_script.dialogueFile = "intro.json";
            dialogue_object.gameObject.SetActive(true);
            return;
        }

        // if (Input.GetKeyDown(KeyCode.Space) && !titledisappear)
        // {
        if (Cinema.gameObject.activeSelf)
        {
            Cinema.gameObject.SetActive(false);
        }
        if (!titledisappear){
            audioManager.startClip(audioManager.TransitionMusic, 0.01);
            audioManager.switchTrack();
            audioManager.startLoopClip(audioManager.MenuMusic, 0.01 + (double)audioManager.TransitionMusic.samples / audioManager.TransitionMusic.frequency);
            titledisappear = true;
        }
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

    void startTitleScreen()
    {
        audioManager.startLoopClip(audioManager.TitleMusic, 0.01);
        TitleScreen.gameObject.SetActive(true);
    }
}
