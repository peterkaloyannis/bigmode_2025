using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardEffects : MonoBehaviour
{
    private Dictionary<string, Image> keys;
    private Dictionary<string, Transform> keyTransforms;
    public Sprite emptyKey;
    public Sprite fullKey;
    private Dictionary<KeyCode, string> keyMap;
    private float maxScale = 0.6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyTransforms = new Dictionary<string, Transform>();
        foreach (Transform child in transform)
        {
            keyTransforms.Add(child.name, child);
        }
        keys = new Dictionary<string, Image>();
        foreach (Transform child in transform)
        {
            keys.Add(child.name, child.GetComponent<Image>());
        }
        keyMap = new Dictionary<KeyCode, string>();
        keyMap.Add(KeyCode.A, "Akey");
        keyMap.Add(KeyCode.S, "Skey");
        keyMap.Add(KeyCode.DownArrow, "Downkey");
        keyMap.Add(KeyCode.LeftArrow, "Leftkey");
        keyMap.Add(KeyCode.RightArrow, "Rightkey");
        keyMap.Add(KeyCode.UpArrow, "Upkey");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode keycode in keyMap.Keys)
        {
            if (Input.GetKeyDown(keycode))
            {
                if (keys.ContainsKey(keyMap[keycode]))
                {
                    keys[keyMap[keycode]].sprite = fullKey;
                }
            }
            else if (Input.GetKeyUp(keycode))
            {
                if (keys.ContainsKey(keyMap[keycode]))
                {
                    keys[keyMap[keycode]].sprite = emptyKey;
                }
                keyTransforms[keyMap[keycode]].localScale = new Vector3(0.6f*maxScale, 0.6f*maxScale, maxScale);
            } else if (Input.GetKey(keycode))
            {
                keyTransforms[keyMap[keycode]].localScale = Vector3.Lerp(keyTransforms[keyMap[keycode]].localScale, new Vector3(0.6f*maxScale, 0.6f*maxScale, maxScale), 0.1f);
            }
        }
        foreach (string key in keys.Keys)
        {
            if (keyTransforms[key].localScale != new Vector3(maxScale, maxScale, maxScale)){
                keyTransforms[key].localScale = Vector3.Lerp(keyTransforms[key].localScale, new Vector3(maxScale, maxScale, maxScale), 0.1f);
                if (keyTransforms[key].localScale.x > 0.99f){
                    keyTransforms[key].localScale = new Vector3(maxScale, maxScale, maxScale);
                }
            }
        }
    }
}
