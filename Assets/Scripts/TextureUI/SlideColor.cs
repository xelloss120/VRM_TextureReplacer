using UnityEngine;
using UnityEngine.UI;

public class SlideColor : MonoBehaviour
{
    [SerializeField] Image Image;
    [SerializeField] Slider Slider;
    [SerializeField] InputField InputField;
    [SerializeField] string Color;

    /// <summary>
    /// スライダーによる色編集
    /// </summary>
    public void Changed()
    {
        var color = Image.color;
        color.r = Color == "R" ? Slider.value : color.r;
        color.g = Color == "G" ? Slider.value : color.g;
        color.b = Color == "B" ? Slider.value : color.b;
        color.a = 1;
        Image.color = color;

        var text = InputField.text;
        text = Color == "R" ? ((byte)(Slider.value * 255)).ToString("X2") + text.Substring(2, 2) + text.Substring(4, 2) : text;
        text = Color == "G" ? text.Substring(0, 2) + ((byte)(Slider.value * 255)).ToString("X2") + text.Substring(4, 2) : text;
        text = Color == "B" ? text.Substring(0, 2) + text.Substring(2, 2) + ((byte)(Slider.value * 255)).ToString("X2") : text;
        InputField.text = text;
    }
}
