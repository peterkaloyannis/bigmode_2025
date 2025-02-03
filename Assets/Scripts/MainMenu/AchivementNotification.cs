using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class AchivementNotification : MonoBehaviour
{
    public AudioClip notifSound;
    public AudioSource notifSource;
    private float offScreenPosition_y;
    private float onScreenPosition_y;
    private bool isOnScreen = false;
    private float countDown = 6f;
    private float trackerCountDown = 0f;
    private float waitTimer = 0f;
    public bool display = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offScreenPosition_y = 377.2f;
        onScreenPosition_y = 240.79f;
    }

    // Update is called once per frame
    void Update()
    {
        if (display)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 1f)
            {
                if (!isOnScreen)
                {
                    isOnScreen = true;
                    trackerCountDown = 0f;
                    notifSource.PlayOneShot(notifSound);
                }
                display = false;
            }
        }
        if (isOnScreen)
        {
            if (Mathf.Abs(transform.localPosition.y - onScreenPosition_y) < 0.1f)
            {
                trackerCountDown += Time.deltaTime;
                if (trackerCountDown > countDown)
                {
                    isOnScreen = false;
                }
            } else {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, onScreenPosition_y, transform.localPosition.z), 0.1f);
            }
        } else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, offScreenPosition_y, transform.localPosition.z), 0.1f);
        }
    }
}
