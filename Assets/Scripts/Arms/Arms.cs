using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arms : MonoBehaviour
{
    public BossManagerLogic boss_manager;
    public Image image;
    private int currentAngle = 50;
    private Dictionary<int, string> pathRef;
    public float rumbleIntensity = 0.1f;  // How far the sprite will move during the rumble
    private float maxRumbleIntensity = 0.5f;
    private float minRumbleIntensity = 0.1f;
    public float rumbleSpeed = 10f;       // How quickly the sprite will rumble
    private Vector3 originalPosition;
    private float last_boss_value = 0f;

    void Start(){
        originalPosition = transform.position;
        pathRef = new Dictionary<int, string>();
        for (int i = 0; i < 10; i++){
            pathRef.Add(i, "Sprites/Arms/Arm00" + (10*i).ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAngle != (int)(boss_manager.meter*10)){
            image.sprite = Resources.Load<Sprite>(pathRef[(int)(boss_manager.meter*10)]);
        }


        float offsetX = Random.Range(-rumbleIntensity, rumbleIntensity);
        float offsetY = Random.Range(-rumbleIntensity, rumbleIntensity);
        
        // Apply the rumble effect
        transform.position = originalPosition + new Vector3(offsetX, 0, offsetY);
        float effort = Mathf.Clamp(boss_manager.meter - last_boss_value, 0f, 1f);
        rumbleIntensity = Mathf.Clamp( rumbleIntensity - 0.05f*Time.deltaTime + effort, minRumbleIntensity, maxRumbleIntensity);

        currentAngle = (int)(boss_manager.meter*10);
        last_boss_value = boss_manager.meter;
    }
}
