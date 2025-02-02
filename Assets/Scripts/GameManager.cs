/*
 * GameManager is a Singleton class, meaning there is guaranteed to only ever exist a single
 * instance of it. It is globally accessible and thus can be used to persist state across
 * scenes.
 */

using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;


// Classes for json deserialization
[System.Serializable]
public class SceneSequence
{
    public List<Scene_> scenes;
}

#nullable enable  // Enable nullable reference types
// Classes for json deserialization
[System.Serializable]
public class Scene_
{
    public string name = "";
    public List<NextSceneOption> next_scene_options = new List<NextSceneOption>();
    public int achievement_unlocked;
}

// Classes for json deserialization
[System.Serializable]
public class NextSceneOption
{
    public int target_scene_idx;
    public List<int> achievement_conditions = new List<int>();
    public string? outcome_condition;
}
#nullable disable  // Enable nullable reference types

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Publically set in the Unity GUI.
    public string sceneSequenceFile;

    // List tracking set of unlocked achievements.
    private List<int> achievementsUnlocked = new List<int>();

    // On Awake(), the scene sequence is loaded in from a json file at "Assets/Resources/SceneSequence/" + sceneSequenceFile
    private string sceneSequenceDir = Application.dataPath + "/Resources/SceneSequence/";
    private string sceneSequenceFilepath;
    private List<Scene_> sceneList;

    // Track the current scene via its index.
    private int currentSceneIdx = 0;

    // Track the list of dialogue boxes.
    private Dictionary<int, Dialogue> allDialogues = new Dictionary<int, Dialogue>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }

        // Load in json from the file.
        // Debug.Log("Checking Scene Sequence File Existence.");
        // sceneSequenceFilepath = checkFileExists(sceneSequenceDir, sceneSequenceFile);
        // string json = File.ReadAllText(sceneSequenceFilepath);
        // SceneSequence sceneSequence = JsonUtility.FromJson<SceneSequence>(json);

        // // Extract and store the list of scenes.
        // sceneList = sceneSequence.scenes;

        AchievementsCheck();

        // Debug.Assert(sceneList.Count > 0, "[ERROR] Zero-length scene sequence.");
    }

    private void Start()
    {
        // Kick things off with the initial advanceScene() call.
        Debug.Log("GameManager Start() called. Initial advanceScene() call.");
        advanceScene();
    }

    // Add a Dialogue instance to the global registry.
    // NOTE: THERE WILL PROBABLY NEED TO BE AN ANALOGOUS REGISTRATION FUNCTION FOR THE FIGHTS
    public bool registerDialogueInstance(Dialogue d)
    {

        if (allDialogues.ContainsKey(d.sceneNumber))
        {
            // Return false since registration was unsuccessful.
            return false;
        }

        allDialogues[d.sceneNumber] = d;
        return true;
    }

    // Check if dir/file exists, and return dir + file if so.
    public static string checkFileExists(string dir, string file)
    {
        string filePath = dir + file;
        if (File.Exists(filePath))
        {
            Debug.Log("Found file: " + filePath);
        }
        else
        {
            throw new System.Exception("Did not find file: " + filePath);
        }
        return filePath;
    }


    public List<string> achsNames = new List<string>(){"Ach1","Ach2","Ach3","Ach4"};
    private void AchievementsCheck()
    {
        if (!PlayerPrefs.HasKey("Ach1")){
            for (int i = 0; i < achsNames.Count; i++){
                PlayerPrefs.SetInt(achsNames[i], 1);
            }
        }
    }

    public bool checkForAnyAchievements()
    {
        bool hasAchievements = false;
        for (int i = 0; i < achsNames.Count; i++){
            if (PlayerPrefs.GetInt(achsNames[i]) > 0){
                hasAchievements = true;
            }
        }
        return hasAchievements;
    }

    public void resetAchievements()
    {
        for (int i = 0; i < achsNames.Count; i++){
            PlayerPrefs.SetInt(achsNames[i], 0);
        }
    }

    // Given a list of integers (treated as a set), check if that
    // list is a subset of the list of unlocked achievements.
    public bool branchIsValid(List<int> branchAchievements)
    {
        // Confirm that branchAchievements is a set.
        if (branchAchievements.Distinct().Count() != branchAchievements.Count)
        {
            // Throw an exception if not, since this is a configuration error.
            throw new System.Exception("Got non-set branchAchievements: " + string.Join(", ", branchAchievements));
        }

        return branchAchievements.All(item => achievementsUnlocked.Contains(item));
    }

    // Clear the set of unlocked achievements. Functionally, this resets the player's save.
    public void clearAchievementsUnlocked()
    {
        achievementsUnlocked.Clear();
    }

    // Knowing the current state, check the state sequence and find the next state.
    // Advance the current state to that state.
    public void advanceScene(string outcome = null)
    {
        int? targetSceneIdx = null;
        Scene_ currentScene = sceneList[currentSceneIdx];

        foreach (NextSceneOption nextSceneOption in currentScene.next_scene_options)
        {
            // This loop will select the next scene which is furthest down the
            // list of next_scene_options whose set of conditions is a subset
            // of the currently unlocked achievements.
            

            // Check whether the provided outcome matches the next_scene outcome condition.
            // The default values for both of these are null. Any mismatch implies that
            // this next_scene is not a valid choice.
            if (outcome != nextSceneOption.outcome_condition)
            {
                continue;
            }

            // If this is a valid choice, check for achievement conditions matching.
            if (branchIsValid(nextSceneOption.achievement_conditions))
            {
                //Debug.Log("Selecting targetSceneIdx: " + nextSceneOption.target_scene_idx);
                targetSceneIdx = nextSceneOption.target_scene_idx;
            }
        }
        Debug.Assert(targetSceneIdx != null, "[ERROR] Failed to find scene. currentScene.name: " + currentScene.name + ", outcome: " + outcome);
        int targetSceneIdxInt = targetSceneIdx.GetValueOrDefault();

        // Check for achievement completion in the current scene.
        // Only add the achievement if it is not already present in the list of unlocked achievements.
        if ((currentScene.achievement_unlocked != 0) && !achievementsUnlocked.Contains(currentScene.achievement_unlocked))
        {
            achievementsUnlocked.Add(currentScene.achievement_unlocked);
            Debug.Log("Adding achievement " + currentScene.achievement_unlocked
                + ". New set of achievements is " + string.Join(", ", achievementsUnlocked));
        }

        // Advance to the next scene.
        Scene_ targetScene = sceneList[targetSceneIdxInt];
        Debug.Log("Advancing to scene " + targetSceneIdxInt + ": " + targetScene.name);
        currentSceneIdx = targetSceneIdxInt;

        // If the next scene has dialogue, enable that Dialogue instance.
        if (allDialogues.ContainsKey(targetSceneIdxInt))
        {
            Debug.Log("Enabling Dialogue instance: " + targetSceneIdxInt);
            allDialogues[targetSceneIdxInt].setGameObjectActive(true);
        }
    }
}
