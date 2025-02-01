using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arms : MonoBehaviour
{
    public FightManager fight_manager;
    public Image image;
    private int currentAngle = 50;
    public float rumbleIntensity = 0.1f;  // How far the sprite will move during the rumble
    private float maxRumbleIntensity = 0.5f;
    private float minRumbleIntensity = 0f;
    public float rumbleSpeed = 10f;       // How quickly the sprite will rumble
    private Vector3 originalPosition;
    private float last_boss_value = 0f;
    private int numberImages;
    private Dictionary<int, Sprite> ArmSprites;
    public RectTransform progress_bar_transform;
    private Material barMat;
    private Material armMat;
    public Transform Frame;
    private Material frameMat;
    public Transform boss;
    private Image boss_image;
    private Dictionary<int, Sprite> BossSprites;
    private Vector3 centralPosition;

    void Start(){
        originalPosition = transform.position;

        // load arm
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Arms");
        System.Array.Reverse(sprites);
        int count = 0;
        ArmSprites = new Dictionary<int, Sprite>();
        foreach (Sprite sprite in sprites){
            ArmSprites.Add(count, sprite);
            count+=1;
        }

        // load boss
        sprites = Resources.LoadAll<Sprite>("Sprites/Boss");
        System.Array.Reverse(sprites);
        count = 0;
        BossSprites = new Dictionary<int, Sprite>();
        foreach (Sprite sprite in sprites){
            BossSprites.Add(count, sprite);
            count+=1;
        }

        numberImages = count;
        barMat = progress_bar_transform.GetComponent<Image>().material;
        armMat = image.material;
        last_boss_value = fight_manager.meter;
        frameMat = Frame.GetComponent<Image>().material;
        boss_image = boss.GetComponent<Image>();

        centralPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = Random.Range(-rumbleIntensity, rumbleIntensity);
        float offsetY = Random.Range(-rumbleIntensity, rumbleIntensity);
        
        // Apply the rumble effect
        transform.position = originalPosition + new Vector3(offsetX, 0, offsetY);
        float effort = Mathf.Clamp(fight_manager.meter - last_boss_value, 0f, 1f);
        rumbleIntensity = Mathf.Clamp( rumbleIntensity - 0.05f*Time.deltaTime + effort, minRumbleIntensity, maxRumbleIntensity);

        UpdateShaderBar();
        UpdateShaderArm();
        UpdateBoss();
        UpdateArmPosition();

        // Update the values for the next call
        currentAngle = (int)(fight_manager.meter*numberImages);
        last_boss_value = fight_manager.meter;
    }

    void UpdateShaderBar(){
        barMat.SetFloat("_Angle", fight_manager.meter);
        frameMat.SetFloat("_Angle", fight_manager.meter);
        barMat.SetFloat("_Shake", rumbleIntensity);
        frameMat.SetFloat("_Shake", rumbleIntensity);
        if (rumbleIntensity > 0.2f){
            barMat.SetFloat("_ElectricityMain", 1f);
            barMat.SetFloat("_ElectricityVillain", 1f);
        } else {
            barMat.SetFloat("_ElectricityMain", 0f);
            barMat.SetFloat("_ElectricityVillain", 0f);
        }
    }

    void UpdateShaderArm(){
        armMat.SetFloat("_Meter", fight_manager.meter);
        if (currentAngle != (int)(fight_manager.meter*numberImages)){
            armMat.SetTexture("_TextureArm", ArmSprites[(int)(Mathf.Clamp(fight_manager.meter*numberImages, 0, numberImages-1))].texture);
        }
    }

    void UpdateBoss(){
        boss_image.sprite = BossSprites[(int)(Mathf.Clamp(fight_manager.meter*numberImages, 0, numberImages-1))];
    }

    void UpdateArmPosition(){
        if (fight_manager.fight_state == fight_state_t.PLAY || fight_manager.fight_state == fight_state_t.INIT){
            if (Input.GetKey(KeyCode.LeftArrow)){
                transform.localPosition = new Vector3(centralPosition.x - 8f, centralPosition.y, centralPosition.z);
            } else if (Input.GetKey(KeyCode.RightArrow)){
                transform.localPosition = new Vector3(centralPosition.x + 8f, centralPosition.y, centralPosition.z);
            } else if (Input.GetKey(KeyCode.UpArrow)){
                transform.localPosition = new Vector3(centralPosition.x, centralPosition.y + 8f, centralPosition.z);
            } else if (Input.GetKey(KeyCode.DownArrow)){
                transform.localPosition = new Vector3(centralPosition.x, centralPosition.y - 8f, centralPosition.z);
            } else {
                transform.localPosition = centralPosition;
            }
        }
    }
}
