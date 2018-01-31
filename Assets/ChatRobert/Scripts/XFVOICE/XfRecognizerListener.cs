using System;
using UnityEngine;

public class XfRecognizerListener : AndroidJavaProxy
{

    public XfRecognizerListener() : base("com.iflytek.cloud.RecognizerListener")
    {

    }

    public static Action<string> OngetResultStr;
    public static Action<int> OnError;

    public string resultString = "";

    public void onVolumeChanged(int volume, byte[] data)
    {
        string showText = "当前正在说话，音量是：" + volume;
        showText.ShowAsToast();
        Debug.Log("返回音频数据:" + data.Length);
    }

    public void onResult(AndroidJavaObject result, bool isLast)
    {
        string text = "";
        string temp_str = "";
        Debug.Log("onresult");
        if (null != result)
        {
            try
            {
                AndroidJavaObject res = result.Call<AndroidJavaObject>("getResultString");
                byte[] resultByte = res.Call<byte[]>("getBytes");
                text = System.Text.Encoding.Default.GetString(resultByte);
                ResultJson temp = JsonUtility.FromJson<ResultJson>(text);
                
                for (int i = 0; i < temp.ws.Length; i++)
                {
                    for (int j = 0; j < temp.ws[i].cw.Length; j++)
                    {
                        temp_str += temp.ws[i].cw[j].w;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        resultString = resultString + temp_str;
        if (isLast)
        {
            if (OngetResultStr!=null)
            {
                OngetResultStr.Invoke(resultString);
            }
//            resultString.ShowAsToast();
        }
    }

    public void onEndOfSpeech()
    {
//        "结束说话".ShowAsToast();
    }

    public void onBeginOfSpeech()
    {
        resultString = "";
    }

    public void onError(AndroidJavaObject error)
    {
        int errorCode = error.Call<int>("getErrorCode");
        if (OnError != null)
        {
            OnError.Invoke(errorCode);
        }
    }
    public void onEvent(int eventType, int arg1, int arg2, AndroidJavaObject BundleObj)
    {
        // 以下代码用于获取与云端的会话id，当业务出错时将会话id提供给讯飞云的技术支持人员，可用于查询会话日志，定位出错原因
//        if (SpeechEvent.EVENT_SESSION_ID == eventType) {
//                String sid = obj.getString(SpeechEvent.KEY_EVENT_SESSION_ID);
//                Log.d(TAG, "session id =" + sid);
//        }
    }

}
[System.Serializable]
public class ResultJson
{
    public int sn;
    public bool ls;
    public int bg;
    public int ed;
    public ResultWord[] ws;
}
[Serializable]
public class ResultWord
{
    public int bg;
    public ResultLetter[] cw;
}
[Serializable]
public class ResultLetter
{
    public string w;
    public int sc;
}
