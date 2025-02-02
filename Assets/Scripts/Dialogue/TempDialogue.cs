using UnityEngine;

public class TempDialogue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip tempSound;
    public Transform bubblePrefab;
    int tracker = 0;
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
            bubble.GetComponent<Bubble>().AssignVariables("Villain", "Oh you little gummibear... im gonna eat you whole", tempSound, true);
        }
    }
}
