using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Mat : MonoBehaviour
{
    public string Name;
    public string Path;

    public Material Material;
    public RawImage RawImage;

    public Image Main;
    public Image Shade;
    public Image Outline;

    /// <summary>
    /// 対象のテクスチャのエクスポート
    /// </summary>
    public void Export()
    {
#if UNITY_EDITOR
        var path = Application.dataPath + "/../_exe/_Texture/" + Name + ".png";
#else
        var path = VRM.Samples.FileDialogForWindows.SaveDialog("save Texture", Path + Name + ".png");
#endif
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        else if (string.Compare(System.IO.Path.GetExtension(path), ".png", true) != 0)
        {
            path += ".png";
        }
        var tex = Material.GetTexture("_MainTex");
        File.WriteAllBytes(path, Tex.tex2png(tex));
    }

    /// <summary>
    /// 対象のテクスチャのインポート
    /// </summary>
    public void Import()
    {
#if UNITY_EDITOR
        var path = Application.dataPath + "/../_exe/_Texture/" + Name + ".png";
#else
        var path = VRM.Samples.FileDialogForWindows.FileDialog("open Texture", ".png");
#endif
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        var tex = Tex.png2tex(path);
        if (tex != null)
        {
            // メインテクスチャと影テクスチャは同じ想定
            Material.SetTexture("_MainTex", tex);
            Material.SetTexture("_ShadeTexture", tex);
            RawImage.texture = tex;
        }
    }

    /// <summary>
    /// 主色の設定
    /// </summary>
    public void SetMainColor()
    {
        Material.SetColor("_Color", Main.color);
    }

    /// <summary>
    /// 影色の設定
    /// </summary>
    public void SetShadeColor()
    {
        Material.SetColor("_ShadeColor", Shade.color);
    }

    /// <summary>
    /// 線色の設定
    /// </summary>
    public void SetOutlineColor()
    {
        Material.SetColor("_OutlineColor", Outline.color);
    }
}
