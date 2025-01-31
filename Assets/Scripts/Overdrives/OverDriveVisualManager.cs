using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverDriveVisualManager : MonoBehaviour
{
    public Sprite upSprite;
    public Sprite rightSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Transform Container;
    public StratagemManagerLogic stratagem_manager;
    public GameObject stratagemPrefab;
    private List<List<Image>> HorizontalGroups;
    private List<Transform> Titles;
    private Sprite[] Logos;
    private List<Material> LogoMats;
    private List<Transform> LogoTransforms;
    private Color arrowColor;
    public Color coolDownArrowColor;
    private Vector3 titleOriginalPosition = Vector3.zero;
    public Transform ActiveStratagemsContainer;
    public Transform ActiveStratagemsPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorUtility.TryParseHtmlString( "#00FDFF" , out arrowColor);
        HorizontalGroups = new List<List<Image>>();
        Titles = new List<Transform>();
        Logos = Resources.LoadAll<Sprite>("Sprites/Stratagems");
        LogoMats = new List<Material>();
        LogoTransforms = new List<Transform>();
        RedoLayout();
    }

    Sprite GetArrow(stratagem_input_t arrow){
        switch (arrow){
            case stratagem_input_t.UP:
                return upSprite;
            case stratagem_input_t.RIGHT:
                return rightSprite;
            case stratagem_input_t.DOWN:
                return downSprite;
            case stratagem_input_t.LEFT:
                return leftSprite;
        }
        return upSprite;
    }

    public void RedoLayout(){
        foreach (Transform child in Container)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < stratagem_manager.stratagem_combos.Count; i++){
            GameObject newInput = Instantiate(stratagemPrefab, Container.transform);
            newInput.name = stratagem_manager.stratagem_names[i];

            GameObject HorizontalGroup = newInput.transform.Find("HorizontalGroup").gameObject;
            GameObject Title = newInput.transform.Find("Title").gameObject;
            titleOriginalPosition = Title.transform.localPosition;
            Titles.Add(Title.transform);
            GameObject Arrow = HorizontalGroup.transform.Find("Arrow").gameObject;

            LogoTransforms.Add(newInput.transform.Find("Logo"));
            LogoTransforms[i].GetComponent<Image>().material = new Material(LogoTransforms[i].GetComponent<Image>().material);
            LogoMats.Add(LogoTransforms[i].GetComponent<Image>().material);
            foreach (Sprite sprite in Logos){
                if (sprite.name == stratagem_manager.stratagem_names[i]){
                    newInput.transform.Find("Logo").GetComponent<Image>().sprite = sprite;
                    LogoMats[i].SetTexture("_MainTex", sprite.texture);
                }
            }

            Title.GetComponent<TextMeshProUGUI>().text = stratagem_manager.stratagem_names[i];
            int count = 0;

            List<Image> images = new List<Image>();
            images.Add(Arrow.GetComponent<Image>());

            foreach (stratagem_input_t arrow in stratagem_manager.stratagem_combos[i]){
                if (count == 0){
                    Arrow.GetComponent<Image>().sprite = GetArrow(arrow);
                } else {
                    GameObject arrowObject = Instantiate(Arrow, HorizontalGroup.transform);
                    arrowObject.GetComponent<Image>().sprite = GetArrow(arrow);
                    images.Add(arrowObject.GetComponent<Image>());
                }
                count+=1;
            }

            HorizontalGroups.Add(images);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(Container.GetComponent<RectTransform>());
    }

    void UpdateActiveEffects()
    {
        for (int i=0; i<stratagem_manager.active_effects.Count; i++){
            for (int j=0; j<stratagem_manager.stratagem_effects.Count; j++){
                int indxOfStratagem = stratagem_manager.stratagem_names.IndexOf(Titles[j].GetComponent<TextMeshProUGUI>().text);
                if (stratagem_manager.stratagem_effects[j].Contains(stratagem_manager.active_effects[i])){
                    // Logos in overdrive menu
                    float offsetX = Random.Range(-0.5f, 0.5f);
                    float offsetY = Random.Range(-0.5f, 0.5f);
                    
                    Titles[j].localPosition += new Vector3(offsetX, 0, offsetY);
                    Titles[j].GetComponent<TextMeshProUGUI>().color = Color.white;

                    
                    // Active effect icons
                    bool isEffectAlreadyInContainer = false;
                    foreach (Transform child in ActiveStratagemsContainer)
                    {
                        if (child.name == stratagem_manager.stratagem_names[j])
                        {
                            isEffectAlreadyInContainer = true;
                        }
                    }

                    if (!isEffectAlreadyInContainer && stratagem_manager.active_effect_timers[i] > 0)
                    {
                        Transform newEffect = Instantiate(ActiveStratagemsPrefab, ActiveStratagemsContainer);
                        newEffect.name = stratagem_manager.stratagem_names[indxOfStratagem];
                        newEffect.GetComponent<Image>().sprite = LogoTransforms[indxOfStratagem].GetComponent<Image>().sprite;
                    }

                }
            }
        }
        foreach (Transform child in ActiveStratagemsContainer)
        {
            int indxOfStratagem = stratagem_manager.stratagem_names.IndexOf(child.name);
            int total = 0;
            foreach (effect_type_t effect in stratagem_manager.active_effects)
            {
                if (stratagem_manager.stratagem_effects[indxOfStratagem].Contains(effect))
                {
                    total += 1;
                }
            }
            if (total == 0)
            {
                Destroy(child.gameObject);
            } else {
                child.GetChild(0).GetComponent<TextMeshProUGUI>().text = total.ToString();
            }
        }
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < stratagem_manager.stratagem_matches.Count; i++){
            bool isInCooldown = (stratagem_manager.stratagem_cooldown_timers[i]!=0);
            for (int j = 0; j < stratagem_manager.stratagem_combos[i].Count; j++){
                if (j < stratagem_manager.stratagem_matches[i]){
                    HorizontalGroups[i][j].color = arrowColor;
                } else if (isInCooldown){
                    HorizontalGroups[i][j].color = coolDownArrowColor;
                } else {
                    HorizontalGroups[i][j].color = Color.white;
                }
            }
            LogoMats[i].SetFloat("_Cooldown", (float)stratagem_manager.stratagem_cooldown_timers[i] / (float)stratagem_manager.stratagem_cooldowns[i]);

            if (stratagem_manager.stratagem_cooldown_timers[i] > 0){
                LogoTransforms[i].localScale = new Vector3(0.3f, 0.3f, 0.3f);
            } else {
                LogoTransforms[i].localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }
        } 
    }
    

    // Update is called once per frame
    void Update()
    {
        
        UpdateCooldowns();

        foreach (Transform title in Titles)
        {
            title.localPosition = titleOriginalPosition;
            title.GetComponent<TextMeshProUGUI>().color = arrowColor;
        }

        UpdateActiveEffects();
        
    }
}
