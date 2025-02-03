using System.Collections.Generic;
using TMPro;
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
    private Dictionary<int, Sprite> ArmSpritesWife;
    public RectTransform progress_bar_transform;
    private Material barMat;
    private Material armMat;
    public Transform Frame;
    private Material frameMat;
    public Transform boss;
    private Image boss_image;
    private Dictionary<int, Sprite> BossSprites;
    private Dictionary<int, Sprite> BossSpritesWife;
    private Sprite Background;
    private Sprite BackgroundWife;
    private Sprite Table;
    private Sprite TableWife;
    private Vector3 centralPosition;
    public Color colorMain1;
    public Color colorMain2;
    public Color colorVillain1;
    public Color colorVillain2;
    public Color colorWife1;
    public Color colorWife2;
    public Transform progressBar;
    private Material progressBarMat;
    public Transform TableTransform;
    public Transform BackgroundTransform;
    private bool isWife = false;
    private Transform Rounds;

    
    void Start(){
        originalPosition = transform.position;

        // Load if this is the wife
        isWife = SceneResetter.Instance.is_wife;

        Rounds = GameObject.Find("Rounds").transform;

        // load arm
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Arms");
        System.Array.Reverse(sprites);
        int count = 0;
        ArmSprites = new Dictionary<int, Sprite>();
        foreach (Sprite sprite in sprites){
            ArmSprites.Add(count, sprite);
            count+=1;
        }

        sprites = Resources.LoadAll<Sprite>("Sprites/Arms2");
        System.Array.Reverse(sprites);
        count = 0;
        ArmSpritesWife = new Dictionary<int, Sprite>();
        foreach (Sprite sprite in sprites){
            ArmSpritesWife.Add(count, sprite);
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

        sprites = Resources.LoadAll<Sprite>("Sprites/Wife");
        System.Array.Reverse(sprites);
        count = 0;
        BossSpritesWife = new Dictionary<int, Sprite>();
        foreach (Sprite sprite in sprites){
            BossSpritesWife.Add(count, sprite);
            count+=1;
        }

        Background = Resources.Load<Sprite>("Sprites/Background");
        BackgroundWife = Resources.Load<Sprite>("Sprites/BackgroundWife");

        Table = Resources.Load<Sprite>("Sprites/Table");
        TableWife = Resources.Load<Sprite>("Sprites/TableWife");

        numberImages = count;
        barMat = progress_bar_transform.GetComponent<Image>().material;
        armMat = image.material;
        frameMat = Frame.GetComponent<Image>().material;

        boss_image = boss.GetComponent<Image>();
        progressBarMat = progressBar.GetComponent<Image>().material;
        centralPosition = transform.localPosition;

        updateScene();
    }

    public void updateScene()
    {
        last_boss_value = fight_manager.meter;
        frameMat.SetColor("_Main", colorMain1);
        frameMat.SetColor("_Main2", colorMain2);
        progressBarMat.SetColor("_Main", colorMain1);
        progressBarMat.SetColor("_Main2", colorMain2);
        if (isWife){
            frameMat.SetColor("_Villain", colorWife1);
            frameMat.SetColor("_Villain2", colorWife2);
            progressBarMat.SetColor("_Villain", colorWife1);
            progressBarMat.SetColor("_Villain2", colorWife2);
            TableTransform.GetComponent<Image>().sprite = TableWife;
            BackgroundTransform.GetComponent<Image>().sprite = BackgroundWife;
            Rounds.gameObject.SetActive(false);
        } else {
            frameMat.SetColor("_Villain", colorVillain1);
            frameMat.SetColor("_Villain2", colorVillain2);
            progressBarMat.SetColor("_Villain", colorVillain1);
            progressBarMat.SetColor("_Villain2", colorVillain2);
            TableTransform.GetComponent<Image>().sprite = Table;
            BackgroundTransform.GetComponent<Image>().sprite = Background;
            Rounds.gameObject.SetActive(true);
            if (SceneResetter.Instance.current_fight == fight_scene_t.boss_0_wins_0_losses || 
                SceneResetter.Instance.current_fight == fight_scene_t.boss_0_wins_1_losses)
            {
                Rounds.Find("Rounds Won").Find("Won").GetComponent<TextMeshProUGUI>().text = "0";
            } else if (SceneResetter.Instance.current_fight == fight_scene_t.boss_1_wins_0_losses || 
                SceneResetter.Instance.current_fight == fight_scene_t.boss_1_wins_1_losses)
            {
                Rounds.Find("Rounds Won").Find("Won").GetComponent<TextMeshProUGUI>().text = "1";
            }

            if (SceneResetter.Instance.current_fight == fight_scene_t.boss_0_wins_1_losses || 
                SceneResetter.Instance.current_fight == fight_scene_t.boss_1_wins_1_losses)
            {
                Rounds.Find("Rounds Lost").Find("Lost").GetComponent<TextMeshProUGUI>().text = "1";
            } else if (SceneResetter.Instance.current_fight == fight_scene_t.boss_0_wins_0_losses || 
                SceneResetter.Instance.current_fight == fight_scene_t.boss_1_wins_0_losses)
            {
                Rounds.Find("Rounds Lost").Find("Lost").GetComponent<TextMeshProUGUI>().text = "0";
            }

            if (SceneResetter.Instance.current_fight == fight_scene_t.boss_secret){
                Rounds.gameObject.SetActive(false);
            }
        }
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
            if (isWife){
                armMat.SetTexture("_TextureArm", ArmSpritesWife[(int)(Mathf.Clamp(fight_manager.meter*numberImages, 0, numberImages-1))].texture);
            } else {
                armMat.SetTexture("_TextureArm", ArmSprites[(int)(Mathf.Clamp(fight_manager.meter*numberImages, 0, numberImages-1))].texture);
            }
        }
    }

    void UpdateBoss(){
        if (isWife){
            boss_image.sprite = BossSpritesWife[(int)(Mathf.Clamp(fight_manager.meter*numberImages, 0, numberImages-1))];
        } else {
            boss_image.sprite = BossSprites[(int)(Mathf.Clamp(fight_manager.meter*numberImages, 0, numberImages-1))];
        }
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
