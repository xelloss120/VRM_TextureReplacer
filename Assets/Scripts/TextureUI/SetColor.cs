using UnityEngine;
using UnityEngine.UI;

public class SetColor : MonoBehaviour
{
    [SerializeField] Image Image;
    [SerializeField] Image Button;
    [SerializeField] GameObject MainUI;
    [SerializeField] GameObject EditUI;

    /// <summary>
    /// 色編集後に適用
    /// </summary>
    public void Click()
    {
        Button.color = Image.color;
        MainUI.SetActive(true);
        EditUI.SetActive(false);
    }
}
