using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Button MoonButton;
    public GameObject SettingGO;

    void Start()
    {
        MoonButton.onClick.AddListener(()=>{SettingGO.SetActive(true);});
        StartCoroutine(CopyFile("59549e41.jet"));
    }

    private IEnumerator CopyFile(string fileName)
    {
        string src = getStreamingPath_for_www() + fileName;
        string des = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(des)) yield break;
        WWW www = new WWW(src);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("www.error:" + www.error);
        }
        else
        {
            FileStream fsDes = File.Create(des);
            fsDes.Write(www.bytes, 0, www.bytes.Length);
            fsDes.Flush();
            fsDes.Close();
        }
        www.Dispose();
    }

    string getStreamingPath_for_www()
    {
        string pre = "file://";
#if UNITY_EDITOR
        pre = "file://";
#elif UNITY_ANDROID
        pre = "";  
#elif UNITY_IPHONE
        pre = "file://";  
#endif
        string path = pre + Application.streamingAssetsPath + "/";
        return path;
    }
}
