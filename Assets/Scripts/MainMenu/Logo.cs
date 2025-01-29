using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    private Dictionary<int, Sprite> logoSprites;
    private float timer;
    private int indx;
    private Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logoSprites = new Dictionary<int, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Logo");
        indx = 0;
        foreach (Sprite sprite in sprites){
            logoSprites.Add(indx, sprite);
            indx+=1;
        }
        timer += 0f;
        indx = 0;
        image = transform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (indx != (int)(timer) % logoSprites.Keys.Count){
            image.sprite = logoSprites[indx];
        }
        indx = (int)(timer) % logoSprites.Keys.Count;
    }
}
