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
    public Dictionary<string, Material> loadingBars;
    public Dictionary<string, List<Image>> HorizontalGroups;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadingBars = new Dictionary<string, Material>();
        HorizontalGroups = new Dictionary<string, List<Image>>();
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

            GameObject loadingBar = newInput.transform.Find("LoadingBar").gameObject;
            loadingBar.GetComponent<Image>().material = new Material(loadingBar.GetComponent<Image>().material);
            loadingBars.Add(stratagem_manager.stratagem_names[i], loadingBar.GetComponent<Image>().material);

            GameObject HorizontalGroup = newInput.transform.Find("HorizontalGroup").gameObject;
            GameObject Title = newInput.transform.Find("Title").gameObject;
            GameObject Arrow = HorizontalGroup.transform.Find("Arrow").gameObject;

            Title.GetComponent<TextMeshProUGUI>().text = stratagem_manager.stratagem_names[i];
            loadingBar.GetComponent<Image>().fillAmount = 0f;
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

            HorizontalGroups.Add(stratagem_manager.stratagem_names[i], images);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(Container.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < stratagem_manager.stratagem_matches.Count; i++){
            for (int j = 0; j < stratagem_manager.stratagem_combos[i].Count; j++){
                if (j < stratagem_manager.stratagem_matches[i]){
                    HorizontalGroups[stratagem_manager.stratagem_names[i]][j].color = Color.red;
                } else {
                    HorizontalGroups[stratagem_manager.stratagem_names[i]][j].color = Color.white;
                }
            }

            loadingBars[stratagem_manager.stratagem_names[i]].SetFloat("_Cooldown", (float)stratagem_manager.stratagem_cooldown_timers[i] / (float)stratagem_manager.stratagem_cooldowns[i]);
        }
    }
}
