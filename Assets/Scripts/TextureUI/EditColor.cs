using UnityEngine;
using UnityEngine.UI;

public class EditColor : MonoBehaviour
{
    [SerializeField] GameObject MainUI;
    [SerializeField] GameObject EditUI;
    [SerializeField] Image Image;
    [SerializeField] Slider SliderR;
    [SerializeField] Slider SliderG;
    [SerializeField] Slider SliderB;
    [SerializeField] InputField InputField;

    /// <summary>
    /// 色指定時の色編集UI初期化
    /// </summary>
    public void Click()
    {
        MainUI.SetActive(false);
        EditUI.SetActive(true);

        InputField.text =
            ((byte)(Image.color.r * 255)).ToString("X2") +
            ((byte)(Image.color.g * 255)).ToString("X2") +
            ((byte)(Image.color.b * 255)).ToString("X2");

        SliderR.value = Image.color.r;
        SliderG.value = Image.color.g;
        SliderB.value = Image.color.b;
    }
}
