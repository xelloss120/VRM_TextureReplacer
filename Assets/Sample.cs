using System.IO;
using UnityEngine;
using UniGLTF;
using VRM;

public class Sample : MonoBehaviour
{
    VRMImporterContext Context;
    RuntimeGltfInstance Loaded;

    void Start()
    {
        var path = @"D:\play\UnityProject\VRM_DollPlay2018\_exe\VRM\AoNana.vrm";
        var bytes = File.ReadAllBytes(path);

        var data = new GlbFileParser(path).Parse();
        var vrm = new VRMData(data);
        Context = new VRMImporterContext(vrm);

        Loaded = default(RuntimeGltfInstance);
        Loaded = Context.Load();
        Loaded.ShowMeshes();
    }

    byte[] tex2png(Texture tex)
    {
        var tex2d = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);

        var render = RenderTexture.active;
        var renderTex = new RenderTexture(tex.width, tex.height, 32);
        var rect = new Rect(0, 0, renderTex.width, renderTex.height);
        Graphics.Blit(tex, renderTex);
        RenderTexture.active = renderTex;
        tex2d.ReadPixels(rect, 0, 0);
        tex2d.Apply();
        RenderTexture.active = render;

        return tex2d.EncodeToPNG();
    }

    Texture png2tex(string path)
    {
        var tex = new Texture2D(1, 1);
        var img = File.ReadAllBytes(path);
        tex.LoadImage(img);

        return tex;
    }

    public void Export()
    {
        var no = 0;
        var meshes = Loaded.Root.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                var name = mat.name.Replace(" (Instance)", "");

                var mainTex = mat.GetTexture("_MainTex");
                if (mainTex != null)
                {
                    File.WriteAllBytes(@"_tex\" + no.ToString("00") + "_" + name + "_Main.png", tex2png(mainTex));
                    no++;
                }

                var shadeTex = mat.GetTexture("_ShadeTexture");
                if (shadeTex != null)
                {
                    File.WriteAllBytes(@"_tex\" + no.ToString("00") + "_" + name + "_Shade.png", tex2png(shadeTex));
                    no++;
                }
            }
        }
    }

    public void Import()
    {
        var no = 0;
        var files = Directory.GetFiles(@"_tex\", "*.png");
        var meshes = Loaded.Root.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                var mainTex = mat.GetTexture("_MainTex");
                if (mainTex != null)
                {
                    mat.SetTexture("_MainTex", png2tex(files[no]));
                    no++;
                }

                var shadeTex = mat.GetTexture("_ShadeTexture");
                if (shadeTex != null)
                {
                    mat.SetTexture("_ShadeTexture", png2tex(files[no]));
                    no++;
                }
            }
        }
    }
}
