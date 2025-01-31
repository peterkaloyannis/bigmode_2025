using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource Source1;
    public AudioClip pressStart;
    public TextMeshProUGUI pressStartText;
    private float countDown = 2f;
    private float trackerCountDown = 0f;
    private float flicker = 0f;
    private bool disappear = false;
    public Transform Cinema;
    void Start()
    {

    }

    void Awake()
    {
        trackerCountDown = 0f;
        flicker = 0f;
        disappear = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Cinema.gameObject.activeSelf)
        {
            Source1.PlayOneShot(pressStart);
            disappear = true;
        }

        if (disappear)
        {
            trackerCountDown += Time.deltaTime;
            flicker += Time.deltaTime;
            if (flicker > 0.1f)
            {
                flicker = 0f;
                pressStartText.alpha = 1-pressStartText.alpha;
            }
            if (trackerCountDown > countDown)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
