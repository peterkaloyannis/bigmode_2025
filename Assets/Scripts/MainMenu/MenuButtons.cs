using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuButtons : MonoBehaviour
{
    public Transform AchievementScreen;
    private Transform ResetProgressButton;
    private Transform AchievementsButton;
    public AchievementsButtons achievementsButtons;

    void Start()
    {
        AchievementScreen.gameObject.SetActive(false);
        ResetProgressButton = transform.Find("Buttons").Find("ResetProgressButton");
        AchievementsButton = transform.Find("Buttons").Find("AchievementsButton");

        RefreshButtons();
    }

    void RefreshButtons()
    {
        if (PlayerPrefs.GetInt("Achievements") > 0){
            ResetProgressButton.Find("Padlock").gameObject.SetActive(false);
            ResetProgressButton.Find("Text").gameObject.SetActive(true);
            AchievementsButton.Find("Padlock").gameObject.SetActive(false);
            AchievementsButton.Find("Text").gameObject.SetActive(true);
        } else {
            ResetProgressButton.Find("Padlock").gameObject.SetActive(true);
            ResetProgressButton.Find("Text").gameObject.SetActive(false);
            AchievementsButton.Find("Padlock").gameObject.SetActive(true);
            AchievementsButton.Find("Text").gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("FightScene1");
        operation.allowSceneActivation = true; // Prevent immediate activation
    }

    public void QuitButton()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void OpenAchievements()
    {
        if (PlayerPrefs.GetInt("Achievements") > 0){
            AchievementScreen.gameObject.SetActive(true);
            AchievementScreen.GetComponent<AchievementsButtons>().ResetBox();
            achievementsButtons.UpdateDisplay();
        }
    }

    public void BackToMenuFromAchievements()
    {
        AchievementScreen.gameObject.SetActive(false);
    }

    public void ResetProgress()
    {
        PlayerPrefs.SetInt("Achievements", 0);
        RefreshButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)){ 
            PlayerPrefs.SetInt("Achievements", PlayerPrefs.GetInt("Achievements") + 1);
            RefreshButtons();
        }
    }
}
