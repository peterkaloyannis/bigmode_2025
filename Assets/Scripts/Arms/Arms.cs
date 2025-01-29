using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arms : MonoBehaviour
{
    public BossManagerLogic boss_manager;
    public Image image;
    private int currentAngle = 50;
    public float rumbleIntensity = 0.1f;  // How far the sprite will move during the rumble
    private float maxRumbleIntensity = 0.5f;
    private float minRumbleIntensity = 0.1f;
    public float rumbleSpeed = 10f;       // How quickly the sprite will rumble
    private Vector3 originalPosition;
    private float last_boss_value = 0f;
    private int numberImages;
    private Dictionary<int, Sprite> ArmSprites;
    public RectTransform progress_bar_transform;
    private Material barMat;
    private Material armMat;

    void Start(){
        originalPosition = transform.position;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Arms");
        System.Array.Reverse(sprites);
        int count = 0;
        ArmSprites = new Dictionary<int, Sprite>();
        foreach (Sprite sprite in sprites){
            ArmSprites.Add(count, sprite);
            count+=1;
        }
        numberImages = count;
        barMat = progress_bar_transform.GetComponent<Image>().material;
        armMat = image.material;
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = Random.Range(-rumbleIntensity, rumbleIntensity);
        float offsetY = Random.Range(-rumbleIntensity, rumbleIntensity);
        
        // Apply the rumble effect
        transform.position = originalPosition + new Vector3(offsetX, 0, offsetY);
        float effort = Mathf.Clamp(boss_manager.meter - last_boss_value, 0f, 1f);
        rumbleIntensity = Mathf.Clamp( rumbleIntensity - 0.05f*Time.deltaTime + effort, minRumbleIntensity, maxRumbleIntensity);

        UpdateShaderBar();
        UpdateShaderArm();

        // Update the values for the next call
        currentAngle = (int)(boss_manager.meter*numberImages);
        last_boss_value = boss_manager.meter;
    }

    void UpdateShaderBar(){
        barMat.SetFloat("_Angle", boss_manager.meter);
        barMat.SetFloat("_Shake", rumbleIntensity);
    }

    void UpdateShaderArm(){
        armMat.SetFloat("_Meter", boss_manager.meter);
        if (currentAngle != (int)(boss_manager.meter*numberImages)){
            armMat.SetTexture("_TextureArm", ArmSprites[(int)(Mathf.Clamp(boss_manager.meter*numberImages, 0, numberImages-1))].texture);
            // image.sprite = ArmSprites[(int)(Mathf.Clamp(boss_manager.meter*numberImages, 0, numberImages-1))];
        }
    }
}
