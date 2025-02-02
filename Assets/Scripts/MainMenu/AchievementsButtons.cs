using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementsButtons : MonoBehaviour
{
    private Dictionary<int, string> Achievements;
    public Transform AchievementEncloser;
    public TextMeshProUGUI achievementsTitle;
    public TextMeshProUGUI achievementsQuote;
    public TextMeshProUGUI achievementsText;
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
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Achievements");
        System.Array.Reverse(sprites);
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
            if (PlayerPrefs.GetInt("Achievements") > i){
                achievementsButtons[i].Find("Inside").GetComponent<Image>().sprite = achievementsSprites[i];
            } else {
                achievementsButtons[i].Find("Inside").GetComponent<Image>().sprite = padLock;
            }
        }
    }

    public void ResetBox(){
        AchievementEncloser.gameObject.SetActive(false);
    }

    public void AchivementPress(int achnumber){
        if (PlayerPrefs.GetInt("Achievements") > achnumber){
            AchievementEncloser.gameObject.SetActive(true);
            achievementsText.text = Achievements[achnumber];
        } else {
            AchievementEncloser.gameObject.SetActive(false);
        }
    }
}
