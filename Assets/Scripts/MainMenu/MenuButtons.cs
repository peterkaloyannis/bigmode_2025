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
    public Transform MainScreen;
    public Transform AchievementScreen;
    public bool AchievementsLocked;

    void Start()
    {
        AchievementsLocked = false;
        AchievementScreen.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("FightScene1");
        operation.allowSceneActivation = true; // Prevent immediate activation
    }

    public static void QuitButton()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void OpenAchievements()
    {
        if (!AchievementsLocked){
            AchievementScreen.gameObject.SetActive(true);
            AchievementScreen.GetComponent<AchievementsButtons>().ResetBox();
        }
    }

    public void BackToMenuFromAchievements()
    {
        AchievementScreen.gameObject.SetActive(false);
    }

    public void ResetProgress()
    {
        
    }
}
