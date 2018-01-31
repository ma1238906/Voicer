using System;
using UnityEngine;

public class XfWakeupListener : AndroidJavaProxy
{
    public static Action<int> OnError;

    public XfWakeupListener() : base("com.iflytek.cloud.WakeuperListener")
    {

    }

    public void onResult(AndroidJavaObject result)
    {
        "唤醒".ShowAsToast();
        if (null != result)
        {
            try
            {
                AndroidJavaObject res = result.Call<AndroidJavaObject>("getResultString");
                byte[] resultByte = res.Call<byte[]>("getBytes");
                string tempStr = System.Text.Encoding.Default.GetString(resultByte);
                WakeupResult wakeupResult = JsonUtility.FromJson<WakeupResult>(tempStr);
                wakeupResult.sst.ShowAsToast();
            }
            catch (Exception e)
            {
                e.ToString().ShowAsToast();
            }
        }
    }

    public void onError(AndroidJavaObject error)
    {
        int errorCode = error.Call<int>("getErrorCode");
        if (OnError != null)
        {
            OnError.Invoke(errorCode);
        }
    }

    public void onBeginOfSpeech()
    {
        "开始说话".ShowAsToast();
    }

    public void onEvent(int EventType, int isLast, int arg2, AndroidJavaObject BundleObj)
    {
        
    }
}

public class WakeupResult
{
    public string sst;//wakeup表示语音唤醒，oneshot表示唤醒+识别
    public string id;
    public string score;
    public string bos;
    public string eos;
}
