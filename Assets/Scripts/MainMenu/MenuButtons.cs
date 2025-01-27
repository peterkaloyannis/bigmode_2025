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
}
