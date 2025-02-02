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
    public List<int> achievements_needed;
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
    public TextMeshProUGUI speakerComponent;
    public TextMeshProUGUI textComponent;
    public float textSpeed;
    public string dialogueFile;

    // Which scene in a scene sequence this instance is associated with.
    // This is here so that a dev can specify this value in the Unity GUI.
    // This allows the GameManager to uniquely identify each dialogue instance,
    // and to associate it with a "scene", which is one of the entries in a json
    // file in Assets/Resources/SceneSequence/ that defines the course of states
    // that the game steps through.
    public int sceneNumber;

    // On Start(), dialogue is loaded in from a json file at "Assets/Resources/Dialogue/" + dialogueFile
    private string dialogueFileDir = Application.dataPath + "/Resources/Dialogue/";
    private string dialogueFilePath;

    // The currently active DialogueLine, and an index for tracking how far along
    // that set of Lines you are.
    private DialogueBranch[] dialogueBranches;
    private DialogueLine[] dialogueLines;
    private int line_idx;

    // Index denoting which line of text is currently active.
    private int text_idx;

    // Check that the desired file exists and register with the GameManager.
    void Awake()
    {
        Debug.Log("Checking Dialogue File Existence.");
        dialogueFilePath = GameManager.checkFileExists(dialogueFileDir, dialogueFile);
        
        if (!GameManager.Instance.registerDialogueInstance(this))
        {
            // If registration fails, throw an exception since this is a configuration error.
            throw new System.Exception("Failed to register Dialogue with sceneNumber: " + sceneNumber);
        }

        // Read the JSON file.
        string json = File.ReadAllText(dialogueFilePath);

        // Deserialize the JSON into an array of DialogueBranch objects.
        dialogueBranches = DialogueJsonHelper.FromJson<DialogueBranch>(json);
        Debug.Assert(
            dialogueBranches.Length > 0,
            "Got no dialogue branches in dialogue file: " + dialogueFilePath
        );
        Debug.Assert(
            dialogueBranches[0].achievements_needed.Count == 0,
            "First branch in dialogue file must require no achievements: " + dialogueFilePath
        );
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        Debug.Assert(false, "[ERROR] The block of code just before this selects what dialogue "
        + "to display based on the unlocked set of achievments. This decision will probably "
        + "need to happen somewhere else than the Start() function, which only runs once, "
        + "in order to support dynamically changing the dialogue in a given scene.");

        // Now we have our desired dialogueLines! Set the text boxes to empty, then start.
        textComponent.text = string.Empty;
        speakerComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        // Click spacebar to proceed.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Extract the current set of lines for convenience.
            string[] text_lines = dialogueLines[line_idx].text;
            if (textComponent.text == text_lines[text_idx])
            {
                // If this line of text is complete, move to the next line of text.
                // This includes moving to the next speaker.
                NextLine();
            }
            else
            {
                // If this line of text is incomplete,
                // immediately complete it.
                StopAllCoroutines();
                textComponent.text = text_lines[text_idx];
            }
        }
    }

    void StartDialogue()
    {
        line_idx = 0;
        text_idx = 0;
        speakerComponent.text = getCurrentSpeaker();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Type each character 1 by 1
        foreach (char c in dialogueLines[line_idx].text[text_idx].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (text_idx < dialogueLines[line_idx].text.Length - 1)
        {
            // If we haven't reached the end of the current block of text,
            // we need to display the next bit of text in this block.
            // Increment the text index, empty the displayed text in this component,
            // and start displaying the next line.
            text_idx++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else if (line_idx < dialogueLines.Length - 1)
        {
            // In this case, we have reached the end of the current speaker's block
            // of text, and there exists another speaker.
            // Reset the text index, and increment the line index.
            // Empty the component's text, and set the new speaker.
            // Finally, start displaying the next line.
            text_idx = 0;
            line_idx++;
            textComponent.text = string.Empty;
            speakerComponent.text = getCurrentSpeaker();
            StartCoroutine(TypeLine());
        }
        else
        {
            // The dialogue is done. De-activate this and tell the GameManager
            // to advance.
            // setGameObjectActive(false);
            Debug.Assert(false, "This gameObject.SetActive() call can probably go inside the setGameObjectActive(false) once that function is working.");
            gameObject.SetActive(false);

            GameManager.Instance.advanceScene();
        }
    }

    // Getter for the currently active speaker.
    public string getCurrentSpeaker()
    {
        return dialogueLines[line_idx].speaker;
    }

    public void setGameObjectActive(bool active)
    {
        Debug.Assert(false, "This is a function that the GameManager can call to disable "
        + "this Dialogue component, i.e. by setting the gameObject to inactive or something. "
        + "My idea was that components would be enabled/disabled by the GameManager, but "
        + "I couldn't make it work.");
    }
}
