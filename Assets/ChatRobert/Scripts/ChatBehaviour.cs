using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatBehaviour : MonoBehaviour
{
    public Text AnswerText;
    public AnimationController animCon;
    public static ChatBehaviour Instance;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Instance = this;
        XfRecognizerListener.OngetResultStr += OnGetVoiceResult;
        IFlyVoice.initIFlyVoice();
    }

    void OnDisable()
    {
        XfRecognizerListener.OngetResultStr -= OnGetVoiceResult;
    }

    public void OnGetVoiceResult(string content)
    {
        Debug.Log(content);
        if (content.Contains("坐下"))
        {
            animCon.SitDown();   
        }
        else if (content.Contains("站起来"))
        {
            animCon.SitUp();
        }
        else if (content.Contains("答对了"))
        {
            animCon.Vectory();
        }
        else if (content.Contains("跑步"))
        {
            animCon.Walk();
        }
        else if (content.Contains("停止"))
        {
            animCon.StopWalk();
        }
        else
        {
            StartCoroutine(GetAnswer(content));
        }
    }

    private IEnumerator GetAnswer(string question)
    {
        string apiUrl = "http://www.tuling123.com/openapi/api";
        WWWForm form = new WWWForm();
        form.AddField("key", "1c0aae395755444788e477788f37a063");
        form.AddField("info",question);
        form.AddField("userid","wechat-robot");
        WWW www = new WWW(apiUrl,form);
        yield return www;
        if (www.isDone)
        {
            RobertResponse response = JsonUtility.FromJson<RobertResponse>(www.text);
            AnswerText.text += response.text + "\n";
            response.text.Speak(SettingPanel.voicer);
        }
    }
}
[System.Serializable]
public class RobertResponse
{
    public string code;
    public string text;
}