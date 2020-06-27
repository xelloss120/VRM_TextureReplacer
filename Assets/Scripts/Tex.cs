using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Tex : MonoBehaviour
{
    [SerializeField] Vrm Vrm;
    [SerializeField] GameObject Content;
    [SerializeField] GameObject TexturePrefab;

    public string Path;

    /// <summary>
    /// 全テクスチャのエクスポート
    /// </summary>
    public void AllExport()
    {
        if (Vrm.VRM == null) return;

        var no = 0;
        var meshes = Vrm.VRM.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                var tex = mat.GetTexture("_MainTex");
                if (tex != null)
                {
                    // ファイル名のフォーマット：二桁連番_オブジェクト名_マテリアル名.png
                    var name = mesh.gameObject.name + "_" + mat.name.Replace(" (Instance)", "");
#if UNITY_EDITOR
                    var path = Application.dataPath + "/../_exe/_Texture/" + no.ToString("00") + "_" + name + ".png";
#else
                    var path = Application.dataPath + "/../_Texture/" + no.ToString("00") + "_" + name + ".png";
#endif
                    File.WriteAllBytes(path, tex2png(tex));
                    no++;
                }
            }
        }
    }

    /// <summary>
    /// 全テクスチャのインポート
    /// </summary>
    public void AllImport()
    {
        if (Vrm.VRM == null) return;

        var no = 0;
#if UNITY_EDITOR
        var path = Application.dataPath + "/../_exe/_Texture/";
#else
        var path = Application.dataPath + "/../_Texture/";
#endif
        var files = Directory.GetFiles(path, "*.png");
        var meshes = Vrm.VRM.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                var mainTex = mat.GetTexture("_MainTex");
                var shadeTex = mat.GetTexture("_ShadeTexture");
                if ((mainTex != null || shadeTex != null) && files.Length > no)
                {
                    var tex = png2tex(files[no]);
                    if (tex != null)
                    {
                        // メインテクスチャと影テクスチャは同じ想定
                        mat.SetTexture("_MainTex", tex);
                        mat.SetTexture("_ShadeTexture", tex);
                        no++;
                    }
                }
            }
        }

        // 全インポートでUIをリセット
        Get();
    }

    /// <summary>
    /// マテリアルごとのUIを生成
    /// </summary>
    public void Get()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var meshes = Vrm.VRM.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                var tex = mat.GetTexture("_MainTex");
                if (tex != null)
                {
                    var prefab = Instantiate(TexturePrefab);
                    prefab.transform.parent = Content.transform;

                    // UIの取得
                    var name = prefab.transform.Find("TextureName").GetComponent<Text>();
                    var image = prefab.transform.Find("MainUI/RawImage").GetComponent<RawImage>();
                    var export = prefab.transform.Find("MainUI/Export").GetComponent<Button>();
                    var import = prefab.transform.Find("MainUI/Import").GetComponent<Button>();
                    var main = prefab.transform.Find("MainUI/MainColor").GetComponent<Image>();
                    var shade = prefab.transform.Find("MainUI/ShadeColor").GetComponent<Image>();
                    var outline = prefab.transform.Find("MainUI/OutlineColor").GetComponent<Image>();

                    // マテリアル編集準備
                    var matCom = prefab.GetComponent<Mat>();
                    matCom.Name = mesh.gameObject.name + "_" + mat.name.Replace(" (Instance)", "");
                    matCom.Path = System.IO.Path.GetDirectoryName(Path) + "/";
                    matCom.Material = mat;
                    matCom.RawImage = image;
                    matCom.Main = main;
                    matCom.Shade = shade;
                    matCom.Outline = outline;

                    // UIの初期表示設定
                    image.texture = tex;
                    name.text = matCom.Name;
                    export.onClick.AddListener(matCom.Export);
                    import.onClick.AddListener(matCom.Import);
                    main.color = mat.GetColor("_Color");
                    shade.color = mat.GetColor("_ShadeColor");
                    outline.color = mat.GetColor("_OutlineColor");

                    // 色設定UIの取得
                    var mainBtn = prefab.transform.Find("MainRGB/Set").GetComponent<Button>();
                    var shadeBtn = prefab.transform.Find("ShadeRGB/Set").GetComponent<Button>();
                    var outlineBtn = prefab.transform.Find("OutlineRGB/Set").GetComponent<Button>();

                    mainBtn.onClick.AddListener(matCom.SetMainColor);
                    shadeBtn.onClick.AddListener(matCom.SetShadeColor);
                    outlineBtn.onClick.AddListener(matCom.SetOutlineColor);
                }
            }
        }
    }

    /// <summary>
    /// テクスチャ→png画像データ
    /// </summary>
    public static byte[] tex2png(Texture tex)
    {
        var render = RenderTexture.active;
        var renderTex = new RenderTexture(tex.width, tex.height, 32);
        var tex2d = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        var rect = new Rect(0, 0, renderTex.width, renderTex.height);

        Graphics.Blit(tex, renderTex);
        RenderTexture.active = renderTex;
        tex2d.ReadPixels(rect, 0, 0);
        tex2d.Apply();
        RenderTexture.active = render;

        return tex2d.EncodeToPNG();
    }

    /// <summary>
    /// png画像ファイル→テクスチャ
    /// </summary>
    public static Texture png2tex(string path)
    {
        var tex = new Texture2D(1, 1);
        var img = File.ReadAllBytes(path);
        tex.LoadImage(img);

        return tex;
    }
}
