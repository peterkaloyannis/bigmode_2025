using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementsButtons : MonoBehaviour
{
    private Dictionary<int, string> Achievements;
    public Transform AchievementEncloser;
    private TextMeshProUGUI achievementsTitle;
    private TextMeshProUGUI achievementsQuote;
    private TextMeshProUGUI achievementsText;
    private Image achievementsImage;
    public Sprite padLock;
    private List<Sprite> achievementsSprites;
    private List<Transform> achievementsButtons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialize();
    }

    void initialize()
    {
        achievementsTitle = AchievementEncloser.Find("A_desc_title").GetComponent<TextMeshProUGUI>();
        achievementsQuote = AchievementEncloser.Find("A_desc_quote").GetComponent<TextMeshProUGUI>();
        achievementsText = AchievementEncloser.Find("A_desc_description").GetComponent<TextMeshProUGUI>();
        achievementsImage = AchievementEncloser.Find("A_desc_image").GetComponent<Image>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Achievements");
        int count = 0;
        achievementsSprites = new List<Sprite>();
        foreach (Sprite sprite in sprites){
            achievementsSprites.Add(sprite);
            count+=1;
        }

        achievementsButtons = new List<Transform>();
        achievementsButtons.Add(transform.Find("AchievementsImages").Find("Ach1"));
        achievementsButtons.Add(transform.Find("AchievementsImages").Find("Ach2"));
        achievementsButtons.Add(transform.Find("AchievementsImages").Find("Ach3"));
        achievementsButtons.Add(transform.Find("AchievementsImages").Find("Ach4"));

        for (int i=0; i< achievementsButtons.Count; i++){
            achievementsButtons[i].Find("Inside").GetComponent<Image>().sprite = padLock;
            achievementsButtons[i].Find("Logo").GetComponent<Image>().sprite = achievementsSprites[i];
        }

        Achievements = new Dictionary<int, string>();
        Achievements.Add(0, "Defeat the first boss");
        Achievements.Add(1, "Defeat the second boss");
        Achievements.Add(2, "Defeat the third boss");
        Achievements.Add(3, "Defeat the fourth boss");

        UpdateDisplay();
    }

    void Awake()
    {
        initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDisplay()
    {
        for (int i=0; i< Achievements.Count; i++){
            if (PlayerPrefs.GetInt(GameManager.Instance.achsNames[i]) > 0){
                achievementsButtons[i].Find("Inside").GetComponent<Image>().gameObject.SetActive(false);
                achievementsButtons[i].Find("Logo").GetComponent<Image>().gameObject.SetActive(true);
            } else {
                achievementsButtons[i].Find("Inside").GetComponent<Image>().gameObject.SetActive(true);
                achievementsButtons[i].Find("Logo").GetComponent<Image>().gameObject.SetActive(false);
            }
        }
    }

    public void ResetBox(){
        AchievementEncloser.gameObject.SetActive(false);
    }

    public void AchivementPress(int achnumber){
        if (PlayerPrefs.GetInt(GameManager.Instance.achsNames[achnumber]) > 0){
            AchievementEncloser.gameObject.SetActive(true);
            achievementsText.text = Achievements[achnumber];
            achievementsImage.sprite = achievementsSprites[achnumber];
        } else {
            AchievementEncloser.gameObject.SetActive(false);
        }
    }
}
