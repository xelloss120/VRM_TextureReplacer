using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Mat : MonoBehaviour
{
    public string Path;

    public Material Material;
    public RawImage RawImage;

    public Image Main;
    public Image Shade;
    public Image Outline;
    public Dropdown Dropdown;

    /// <summary>
    /// 対象のテクスチャのエクスポート
    /// </summary>
    public void Export()
    {
#if UNITY_EDITOR
        var path = Application.dataPath + "/../_exe/_Texture/" + Material.name + ".png";
#else
        var path = VRM.SimpleViewer.FileDialogForWindows.SaveDialog("save Texture", Path + Material.name + ".png");
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
        var path = Application.dataPath + "/../_exe/_Texture/" + Material.name + ".png";
#else
        var path = VRM.SimpleViewer.FileDialogForWindows.FileDialog("open Texture", ".png");
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

    enum RenderMode
    {
        Opaque = 0,
        Cutout = 1,
        Transparent = 2,
        TransparentWithZWrite = 3,
    }
    
    struct RenderQueueRequirement
    {
        public int DefaultValue;
        public int MinValue;
        public int MaxValue;
    }

    public const string TagRenderTypeKey = "RenderType";
    public const string TagRenderTypeValueOpaque = "Opaque";
    public const string TagRenderTypeValueTransparentCutout = "TransparentCutout";
    public const string TagRenderTypeValueTransparent = "Transparent";

    public const string PropSrcBlend = "_SrcBlend";
    public const string PropDstBlend = "_DstBlend";
    public const string PropZWrite = "_ZWrite";
    public const string PropAlphaToMask = "_AlphaToMask";

    public const string KeyAlphaTestOn = "_ALPHATEST_ON";
    public const string KeyAlphaBlendOn = "_ALPHABLEND_ON";
    public const string KeyAlphaPremultiplyOn = "_ALPHAPREMULTIPLY_ON";
    public const int DisabledIntValue = 0;
    public const int EnabledIntValue = 1;

    /// <summary>
    /// レンダリングタイプの設定
    /// </summary>
    /// <remarks>
    /// .\VRMShaders\VRM\MToon\MToon\Scripts\Utils.cs
    /// .\VRMShaders\VRM\MToon\MToon\Scripts\UtilsSetter.cs
    /// </remarks>
    public void SetRenderingType(int value)
    {
        var renderMode = (RenderMode)value;
        var material = Material;
        switch (renderMode)
        {
            case RenderMode.Opaque:
                material.SetOverrideTag(TagRenderTypeKey, TagRenderTypeValueOpaque);
                material.SetInt(PropSrcBlend, (int)BlendMode.One);
                material.SetInt(PropDstBlend, (int)BlendMode.Zero);
                material.SetInt(PropZWrite, EnabledIntValue);
                material.SetInt(PropAlphaToMask, DisabledIntValue);
                SetKeyword(material, KeyAlphaTestOn, false);
                SetKeyword(material, KeyAlphaBlendOn, false);
                SetKeyword(material, KeyAlphaPremultiplyOn, false);
                break;
            case RenderMode.Cutout:
                material.SetOverrideTag(TagRenderTypeKey, TagRenderTypeValueTransparentCutout);
                material.SetInt(PropSrcBlend, (int)BlendMode.One);
                material.SetInt(PropDstBlend, (int)BlendMode.Zero);
                material.SetInt(PropZWrite, EnabledIntValue);
                material.SetInt(PropAlphaToMask, EnabledIntValue);
                SetKeyword(material, KeyAlphaTestOn, true);
                SetKeyword(material, KeyAlphaBlendOn, false);
                SetKeyword(material, KeyAlphaPremultiplyOn, false);
                break;
            case RenderMode.Transparent:
                material.SetOverrideTag(TagRenderTypeKey, TagRenderTypeValueTransparent);
                material.SetInt(PropSrcBlend, (int)BlendMode.SrcAlpha);
                material.SetInt(PropDstBlend, (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt(PropZWrite, DisabledIntValue);
                material.SetInt(PropAlphaToMask, DisabledIntValue);
                SetKeyword(material, KeyAlphaTestOn, false);
                SetKeyword(material, KeyAlphaBlendOn, true);
                SetKeyword(material, KeyAlphaPremultiplyOn, false);
                break;
            case RenderMode.TransparentWithZWrite:
                material.SetOverrideTag(TagRenderTypeKey, TagRenderTypeValueTransparent);
                material.SetInt(PropSrcBlend, (int)BlendMode.SrcAlpha);
                material.SetInt(PropDstBlend, (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt(PropZWrite, EnabledIntValue);
                material.SetInt(PropAlphaToMask, DisabledIntValue);
                SetKeyword(material, KeyAlphaTestOn, false);
                SetKeyword(material, KeyAlphaBlendOn, true);
                SetKeyword(material, KeyAlphaPremultiplyOn, false);
                break;
        }

        var requirement = GetRenderQueueRequirement(renderMode);
        material.renderQueue = requirement.DefaultValue;
    }

    void SetKeyword(Material mat, string keyword, bool required)
    {
        if (required)
            mat.EnableKeyword(keyword);
        else
            mat.DisableKeyword(keyword);
    }

    RenderQueueRequirement GetRenderQueueRequirement(RenderMode renderMode)
    {
        const int shaderDefaultQueue = -1;
        const int firstTransparentQueue = 2501;
        const int spanOfQueue = 50;

        switch (renderMode)
        {
            case RenderMode.Opaque:
                return new RenderQueueRequirement()
                {
                    DefaultValue = shaderDefaultQueue,
                    MinValue = shaderDefaultQueue,
                    MaxValue = shaderDefaultQueue,
                };
            case RenderMode.Cutout:
                return new RenderQueueRequirement()
                {
                    DefaultValue = (int)RenderQueue.AlphaTest,
                    MinValue = (int)RenderQueue.AlphaTest,
                    MaxValue = (int)RenderQueue.AlphaTest,
                };
            case RenderMode.Transparent:
                return new RenderQueueRequirement()
                {
                    DefaultValue = (int)RenderQueue.Transparent,
                    MinValue = (int)RenderQueue.Transparent - spanOfQueue + 1,
                    MaxValue = (int)RenderQueue.Transparent,
                };
            case RenderMode.TransparentWithZWrite:
                return new RenderQueueRequirement()
                {
                    DefaultValue = firstTransparentQueue,
                    MinValue = firstTransparentQueue,
                    MaxValue = firstTransparentQueue + spanOfQueue - 1,
                };
            default:
                throw new ArgumentOutOfRangeException("renderMode", renderMode, null);
        }
    }
}
