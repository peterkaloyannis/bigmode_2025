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
    private List<string> achievementTitles = new List<string>(){
        "Gone Hollow",
        "Reach Heaven Through Violence",
        "One Step Further From Home",
        "Diamond Dream"
    };
    private List<string> achievementDescriptions = new List<string>(){
        "An ember barely flickers among the ash.\n\nUnlock the Obsidian Break Overdrive.",
        "One down, 5,999,999,999 to go.\n\nUnlock the Thorium Rush Overdrive.",
        "There's some good in this world, Mr. Frodo, and it's worth fighting for.\n\nUnlock the Cartridge Charge Overdrive.",
        "Go gently into that good night.\n\nThanks for playing!"
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
            if (PlayerPrefs.GetInt(GameManager.Instance.achsNames[state-1]) == 0){
                Debug.Log("Should notify");
                PlayerPrefs.SetInt(GameManager.Instance.achsNames[state-1],1);
                achievementNotification.transform.Find("Logo").GetComponent<Image>().sprite = achievementLogos[state-1];
                achievementNotification.display = true;
                achievementNotification.transform.Find("AchievementTitle").GetComponent<TextMeshProUGUI>().text = achievementTitles[state-1];
                achievementNotification.transform.Find("AchievementText").GetComponent<TextMeshProUGUI>().text = achievementDescriptions[state-1];
            }
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
