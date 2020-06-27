using System;
using UnityEngine;
using UnityEngine.UI;

public class HexColor : MonoBehaviour
{
    [SerializeField] Image Image;
    [SerializeField] Slider SliderR;
    [SerializeField] Slider SliderG;
    [SerializeField] Slider SliderB;

    /// <summary>
    /// 16進色指定
    /// </summary>
    public void EndEdit(string text)
    {
        if (text.Length != 6) return;

        var color = new Color32();
        color.r = Convert.ToByte(text.Substring(0, 2), 16);
        color.g = Convert.ToByte(text.Substring(2, 2), 16);
        color.b = Convert.ToByte(text.Substring(4, 2), 16);
        color.a = 255;

        Image.color = color;
        SliderR.value = color.r / 255;
        SliderG.value = color.g / 255;
        SliderB.value = color.b / 255;
    }
}
