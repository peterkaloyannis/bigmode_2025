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
    public Image achievementsImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Achievements = new Dictionary<int, string>();
        Achievements.Add(0, "Defeat the first boss");
        Achievements.Add(1, "Defeat the second boss");
        Achievements.Add(2, "Defeat the third boss");
        Achievements.Add(3, "Defeat the fourth boss");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBox(){
        AchievementEncloser.gameObject.SetActive(false);
    }

    public void AchivementPress(int achnumber){
        AchievementEncloser.gameObject.SetActive(true);
        achievementsText.text = Achievements[achnumber];
    }
}
