using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    private Image targetImage;
    private Color color;
    private float alpha = 1f;
    void Awake()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetImage = GetComponent<Image>();
        color = targetImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneResetter.Instance.fadeOut)
        {
            alpha -= Time.deltaTime;
            alpha = Mathf.Max(alpha, 0f);
        } else {
            alpha += 10*Time.deltaTime;
            alpha = Mathf.Min(alpha, 1f);
        }
        color = targetImage.color;
        color.a = alpha;
        targetImage.color = color;
    }
}
