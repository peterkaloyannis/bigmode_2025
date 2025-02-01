using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

// Wrapper class to support loading dialogue in from json files.
[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string[] text;
}

// Wrapper class to support loading dialogue in from json files.
[System.Serializable]
public class DialogueBranch
{
    public int[] achievements_needed;
    public DialogueLine[] lines;
}

// Wrapper class to support loading dialogue in from json files.
[System.Serializable]
public class DialogueTree
{
    public DialogueBranch[] dialogueTrees;
}

// Need this to import json. Ref:
// https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
public static class DialogueJsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.branches;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] branches;
    }
}

public class Dialogue : MonoBehaviour
{
    // Settings public to the Unity GUI.
    // public TextMeshProUGUI speakerComponent;
    // public TextMeshProUGUI textComponent;
    public Transform bubblePrefab;
    private Transform currentBubble;
    public AudioClip audioClip;
    public string dialogueFile;

    // On Start(), dialogue is loaded in from a json file at "Assets/Resources/Dialogue/" + dialogueFile
    private string dialogueFileDir = Application.dataPath + "/Resources/Dialogue/";
    private string dialogueFilePath;

    // The currently active DialogueLine, and an index for tracking how far along
    // that set of Lines you are.
    private DialogueLine[] dialogueLines;
    private int line_idx;
    private int text_idx = 0;


    // Check that the desired file exists.
    void Awake()
    {
        dialogueFilePath = dialogueFileDir + dialogueFile;
        if (File.Exists(dialogueFilePath))
        {
            Debug.Log("Found Dialogue File: " + dialogueFilePath);
        }
        else
        {
           throw new System.Exception("Did not find Dialogue File: " + dialogueFilePath);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Read the JSON file.
        string json = File.ReadAllText(dialogueFilePath);

        // Deserialize the JSON into an array of DialogueBranch objects.
        DialogueBranch[] dialogueBranches = DialogueJsonHelper.FromJson<DialogueBranch>(json);
        Debug.Assert(
            dialogueBranches.Length > 0,
            "Got no dialogue branches in dialogue file: " + dialogueFilePath
        );
        Debug.Assert(
            dialogueBranches[0].achievements_needed.Length == 0,
            "First branch in dialogue file must require no achievements: " + dialogueFilePath
        );

        // Select which dialogue branch to choose. The selection logic is as follows:
        // Iterate through all branches and check the achievements_needed set.
        // If the set is a subset of the total set (known by GameManager.Instance),
        // the branch is a candidate. Accept the candidate which appears furthest down
        // in the list of branches.
        dialogueLines = dialogueBranches[0].lines;
        for (int i = 1; i < dialogueBranches.Length; i++)
        {
            DialogueBranch candidateBranch = dialogueBranches[i]; 
            if (GameManager.Instance.branchIsValid(candidateBranch.achievements_needed))
            {
                // Overwrite the branch if it is valid.
                dialogueLines = candidateBranch.lines;
            }
        }

        // Now we have our desired dialogueLines! Set the text boxes to empty, then start.
        // textComponent.text = string.Empty;
        // speakerComponent.text = string.Empty;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        line_idx = 0;
        InstantiateBubble(dialogueLines[line_idx].speaker, ConcText(dialogueLines[line_idx]), audioClip, true);
    }

    // Update is called once per frame
    void Update()
    {
        // Click spacebar to proceed.
        if (Input.GetKeyDown(KeyCode.Space) && currentBubble != null && line_idx <= dialogueLines.Length - 1)
        {
            // Extract the current set of lines for convenience.
            string[] text_lines = dialogueLines[line_idx].text;
            if (currentBubble.GetComponent<Bubble>().tracker >= currentBubble.GetComponent<Bubble>().numChar && line_idx <= dialogueLines.Length - 2)
            {
                line_idx++;
                InstantiateBubble(dialogueLines[line_idx].speaker, ConcText(dialogueLines[line_idx]), audioClip, true);
            }
            else
            {
                currentBubble.GetComponent<Bubble>().FinishSentence();
            }
        }
    }

    string ConcText(DialogueLine text_lines)
    {
        string concText = "";
        foreach (string line in text_lines.text)
        {
            concText += line + "\n";
        }
        return concText;
    }

    void InstantiateBubble(string speaker, string text, AudioClip tempSound, bool alignmentLeft)
    {
        Transform bubble = Instantiate(bubblePrefab, transform);
        bubble.gameObject.SetActive(true);
        bubble.GetComponent<Bubble>().AssignVariables(speaker, text, tempSound, true);
        RectTransform rt = bubble.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 40 + 18*text.Split('\n').Length);
        currentBubble = bubble;
    }

    // Getter for the currently active speaker.
    string getCurrentSpeaker()
    {
        return dialogueLines[line_idx].speaker;
    }
}
