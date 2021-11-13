using UnityEngine;

public class Export : MonoBehaviour
{
    [SerializeField] Vrm Vrm;

    public string Path;

    /// <summary>
    /// エクスポート（vrmの保存）
    /// </summary>
    public void Save()
    {
#if UNITY_EDITOR
        var path = Application.dataPath + "/../_Debug/Export.vrm";
#else
        var path = VRM.SimpleViewer.FileDialogForWindows.SaveDialog("save VRM", Path);
#endif
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        else if (string.Compare(System.IO.Path.GetExtension(path), ".vrm", true) != 0)
        {
            path += ".vrm";
        }

        Vrm.Save(path);
    }
}
