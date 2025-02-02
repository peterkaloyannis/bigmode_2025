using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private TextMeshProUGUI Title;
    public TextMeshProUGUI Text;
    public int numChar;
    public int tracker = 0;
    private float timer = 0;
    private float TimeToLive = 0.08f;
    private float TimeToLiveSpace = 0.1f;
    private float TimeToLiveDot = 1f;
    private float bopFrequency = 0.08f;
    private float bopTimer = 0f;
    private AudioClip audioClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        Text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Text.maxVisibleCharacters = 0;
        numChar = Text.text.Length;
    }

    public void AssignVariables(string assignedTitle, string assignedText, AudioClip assignedAudioClip, bool alignmentLeft){
        Title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        Text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        timer = 0;
        bopTimer = 0;
        tracker = 0;
        Title.text = assignedTitle;
        Text.text = assignedText;
        audioClip = assignedAudioClip;
        numChar = Text.text.Length;
        if (alignmentLeft)
        {
            Title.alignment = TextAlignmentOptions.Left;
        }
        else
        {
            Title.alignment = TextAlignmentOptions.Right;
        }
    }

    bool checkTimetoLive(float timer, string character){
        if (character == " "){
            return (timer>TimeToLiveSpace);
        } else if (character == "."){
            return (timer>TimeToLiveDot);
        }
        return (timer>TimeToLive);
    }

    bool checkforBop(float timer, string character){
        return (timer>bopFrequency) && (character != " ") && (character != ".");
    }
    // Update is called once per frame
    void Update()
    {
        if (tracker < numChar){
            timer += Time.deltaTime;
            bopTimer += Time.deltaTime;
            if (checkTimetoLive(timer, Text.text[Mathf.Min(tracker, numChar-1)].ToString()))
            {
                timer = 0f;
                tracker += 1;
                Text.maxVisibleCharacters = tracker;
            }
            if (checkforBop(bopTimer, Text.text[Mathf.Min(tracker, numChar-1)].ToString())){
                createBop();
                bopTimer = 0f;
            }

            UpdateTextLength();
        }
    }

    void UpdateTextLength()
    {
        Text.maxVisibleCharacters = tracker;
    }

    void createBop()
    {
        GameObject tempAudio = new GameObject("TempAudioSource");
        // tempAudio.transform.parent = BopContainer;
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();

        Destroy(tempAudio, audioClip.length);
    }

    public void FinishSentence()
    {
        Text.maxVisibleCharacters = numChar;
        tracker = numChar;
    }
}
