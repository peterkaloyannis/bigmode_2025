using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
    public Transform Title1;
    public Transform Title2;
    public Transform Title3;
    public AudioClip TitleMusic;
    public AudioClip TransitionMusic;
    public AudioClip MenuMusic;
    public AudioSource Music1;
    public AudioSource Music2;
    private bool isTransitioning = false;
    private bool inTileMenu = true;
    private float fadeDuration = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Music1.clip = TitleMusic;
        Music1.Play();
        Music1.loop = true;
        Music2.clip = TransitionMusic;
    }

    void Update()
    {
        if (!inTileMenu) {
            if (Music2.isPlaying == false){
                if (Music2.clip != MenuMusic){
                    Music2.clip = MenuMusic;
                }
                Music2.Play();
                Music2.loop = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            if (Title1.gameObject.activeSelf){
                Title1.gameObject.SetActive(false);
                Title2.gameObject.SetActive(true);
            } else if (Title2.gameObject.activeSelf){
                Title2.gameObject.SetActive(false);
                Title3.gameObject.SetActive(true);
            } else if (Title3.gameObject.activeSelf && !isTransitioning && inTileMenu){
                StartCoroutine(TransitionToMainMenu());
            }
        }

        
    }

    // private void playClip(AudioSource audioSource, AudioClip clip, bool repeat=false){
    //     double startTime = AudioSettings.dspTime;
    //     audioSource.clip = clip;
    //     audioSource.PlayScheduled(startTime);

    //     if (repeat){
    //         nextStartTime = startTime + clip1.length;

    //         // Schedule the second clip
    //         audioSource.clip = clip2;
    //         audioSource.PlayScheduled(nextStartTime);
    //     }
    // }

    private IEnumerator TransitionToMainMenu()
    {
        isTransitioning = true;

        // Fade out title screen
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Music1.volume = 1f - (elapsedTime / fadeDuration); // Fade out intro music
            yield return null;
        }
        Music1.Stop();

        // Start main menu music
        Music2.Play();
        isTransitioning = false;
        inTileMenu = false;
        Title3.gameObject.SetActive(false);
    }
}
