using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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
        if (GameManager.Instance.checkForAnyAchievements()){
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
        if (PlayerPrefs.GetInt("Ach4") == 1 && PlayerPrefs.GetInt("Ach3") == 1){
            SceneResetter.Instance.setup_fight_scene(fight_scene_t.wife_3);
        } else if (PlayerPrefs.GetInt("Ach2") == 1){
            SceneResetter.Instance.setup_fight_scene(fight_scene_t.wife_2);
        } else if (PlayerPrefs.GetInt("Ach1") == 1){
            SceneResetter.Instance.setup_fight_scene(fight_scene_t.wife_1);
        } else {
            SceneResetter.Instance.setup_fight_scene(fight_scene_t.wife_0);
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync("FightScene");
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
        if (GameManager.Instance.checkForAnyAchievements()){
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
        GameManager.Instance.resetAchievements();
        RefreshButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < GameManager.Instance.achsNames.Count; i++){
                PlayerPrefs.SetInt(GameManager.Instance.achsNames[i], 1);
            }
            RefreshButtons();
        }
    }
}
