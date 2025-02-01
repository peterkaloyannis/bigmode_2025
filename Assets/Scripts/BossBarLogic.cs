using UnityEngine;
using UnityEngine.UI;

public class BossBarLogic : MonoBehaviour
{
    public uint padding;
    public RectTransform progress_bar_transform;
    // public Image winning_bar;
    // public Image losing_bar;
    public FightManager fight_manager;
    private Material barMat;

    void Start()
    {
        barMat = progress_bar_transform.GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the width master bar. Subtract twice the padding for padding on each side.
        // float progress_bar_width = progress_bar_transform.rect.width - (2*padding);
        // float progress_bar_height = progress_bar_transform.rect.height - (2*padding);

        // // Adjust the winning bar width according to the meter.
        // float winning_bar_width = progress_bar_width * fight_manager.meter;
        // float losing_bar_width = progress_bar_width - winning_bar_width;

        // Set the width of the winning bar according to the meter.
        // winning_bar.rectTransform.sizeDelta = new Vector2(winning_bar_width, progress_bar_height);
        // losing_bar.rectTransform.sizeDelta = new Vector2(losing_bar_width, progress_bar_height);

        // // Set the left most edge of the losing bar to the rightmost edge of the winning bar.
        // float left_most_point = winning_bar.rectTransform.position.x + winning_bar_width;
        // float y_position = winning_bar.rectTransform.position.y;
        // losing_bar.rectTransform.position = new Vector2(left_most_point, y_position); 

        barMat.SetFloat("_Angle", fight_manager.meter);

    }

}
