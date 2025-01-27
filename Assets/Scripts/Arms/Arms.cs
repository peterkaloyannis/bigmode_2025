using UnityEngine;
using UnityEngine.UI;

public class Arms : MonoBehaviour
{
    public BossManagerLogic boss_manager;
    public Sprite middleArm;
    public Sprite leftArm;
    public Sprite rightArm;
    public Image image;

    // Update is called once per frame
    void Update()
    {
        if (boss_manager.meter < 0.25f){
            image.sprite = leftArm;
        } else if (boss_manager.meter < 0.75f){
            image.sprite = middleArm;
        } else {
            image.sprite = rightArm;
        }
    }
}
