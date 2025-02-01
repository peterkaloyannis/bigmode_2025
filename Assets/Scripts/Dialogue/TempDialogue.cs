using UnityEngine;

public class TempDialogue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip tempSound;
    public Transform bubblePrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Transform bubble = Instantiate(bubblePrefab, transform);
            bubble.gameObject.SetActive(true);
            bubble.GetComponent<Bubble>().AssignVariables("Title", "Text Test. text test.", tempSound, true);
        }
    }
}
