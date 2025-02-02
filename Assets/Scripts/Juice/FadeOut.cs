using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    private Image targetImage;
    private Color color;
    private float alpha = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetImage = GetComponent<Image>();
        color = targetImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
        alpha -= Time.deltaTime;
        color = targetImage.color;
        color.a = alpha;
        targetImage.color = color;
    }
}
