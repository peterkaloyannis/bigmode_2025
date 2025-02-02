using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuSingleton : MonoBehaviour
{
    public static MainMenuSingleton Instance { get; private set; }
    public int state;
    public AudioClip Ending1;
    public AudioClip Ending2;
    public AudioClip Ending3;
    public AudioClip Ending4;
    public AudioClip IntroMusic;
    public Transform MainMenuPrefab;
    private AudioManager audioManager;
    private AchivementNotification achievementNotification;
    private Transform Achievements;
    private AchievementsButtons achievementsButtons;
    public Sprite achievement1;
    public Sprite achievement2;
    public Sprite achievement3;
    public Sprite achievement4;
    public List<Sprite> achievementLogos;
    private List<string> achivementTitles = new List<string>(){
        "Achievement 1",
        "Achievement 2",
        "Achievement 3",
        "Achievement 4"
    };
    private List<string> achievementDescriptions = new List<string>(){
        "Unlock the second overdrive",
        "Unlock the third overdrive",
        "Unlock the fourth overdrive",
        "Finish the game"
    };
    void Awake()
    {
        if ( Instance != null && Instance != this )
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Initialize();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Initialize();
        }
    }

    void Initialize()
    {
        Debug.Log("hi");
        achievementLogos = new List<Sprite>();
        achievementLogos.Add(achievement1);
        achievementLogos.Add(achievement2);
        achievementLogos.Add(achievement3);
        achievementLogos.Add(achievement4);

        GameObject mainMenuPrefab = Instantiate(MainMenuPrefab).gameObject;
        mainMenuPrefab.SetActive(true);
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.TitleMusic = musicPicker(state);
        achievementNotification = GameObject.Find("AchievementContainer").GetComponent<AchivementNotification>();
        Achievements = GameObject.Find("Achievements").transform;
        achievementsButtons = Achievements.GetComponent<AchievementsButtons>();
        Achievements.gameObject.SetActive(false);
        if (state != 0){
            achievementNotification.transform.Find("Logo").GetComponent<Image>().sprite = achievementLogos[state-1];
            achievementNotification.display = true;
            achievementNotification.transform.Find("AchievementTitle").GetComponent<TextMeshProUGUI>().text = achivementTitles[state-1];
            achievementNotification.transform.Find("AchievementText").GetComponent<TextMeshProUGUI>().text = achievementDescriptions[state-1];
        }
    }

    AudioClip musicPicker(int i)
    {
        switch (i)
        {
            default:
                return IntroMusic;
            case 0:
                return IntroMusic;
            case 1:
                return Ending1;
            case 2:
                return Ending2;
            case 3:
                return Ending3;
            case 4:
                return Ending4;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
