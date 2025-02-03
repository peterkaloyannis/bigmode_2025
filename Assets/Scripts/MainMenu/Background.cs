using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    private List<Sprite> ListSprites;
    private Image image;
    private float timer = 0f;
    private int indx = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Main");
        System.Array.Reverse(sprites);
        ListSprites = new List<Sprite>();
        foreach (Sprite sprite in sprites){
            ListSprites.Add(sprite);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.2f)
        {
            timer = 0f;
            indx = (indx + 1)%(int)(ListSprites.Count-1);
            image.sprite = ListSprites[indx];
        }
    }
}
