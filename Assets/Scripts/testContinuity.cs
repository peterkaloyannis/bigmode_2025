using UnityEngine;
using UnityEngine.SceneManagement;

public class testContinuity : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MainMenuSingleton.Instance.state += 1;
            Debug.Log(MainMenuSingleton.Instance.state);
            AsyncOperation operation = SceneManager.LoadSceneAsync("MainMenu");
            operation.allowSceneActivation = true;
        }
    }
}
