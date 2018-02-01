using System;
using System.Collections;
using UnityEngine;

public class XfWakeupListener : AndroidJavaProxy
{
    public static Action<int> OnError;

    public XfWakeupListener() : base("com.iflytek.cloud.WakeuperListener")
    {

    }

    public void onResult(AndroidJavaObject result)
    {
        if (null != result)
        {
            try
            {
                AndroidJavaObject res = result.Call<AndroidJavaObject>("getResultString");
                byte[] resultByte = res.Call<byte[]>("getBytes");
                string tempStr = System.Text.Encoding.Default.GetString(resultByte);
                WakeupResult wakeupResult = JsonUtility.FromJson<WakeupResult>(tempStr);
                int score = int.Parse(wakeupResult.score);
                if (score<40)
                {
                    "你说话要清楚啊！".Speak();
                }
                else
                {
                    int ran = UnityEngine.Random.Range(0, 3);
                    switch (ran)
                    {
                        case 0:
                            "你好".Speak(SettingPanel.voicer);
                            break;
                        case 1:
                            "在这呢".Speak(SettingPanel.voicer);
                            break;
                        case 2:
                            "跑得快".Speak(SettingPanel.voicer);
                            break;
                    }
                    ChatBehaviour.Instance.StartCoroutine(Rec());
                }
            }
            catch (Exception e)
            {
                e.ToString().ShowAsToast();
            }
        }
    }

    private IEnumerator Rec()
    {
        yield return new WaitForSeconds(1f);
        IFlyVoice.startRecognize();
    }

    public void onError(AndroidJavaObject error)
    {
        int errorCode = error.Call<int>("getErrorCode");
        if (OnError != null)
        {
            OnError.Invoke(errorCode);
        }
        ("唤醒error"+errorCode).ShowAsToast();
    }

    public void onBeginOfSpeech()
    {
//        "唤醒开始说话".ShowAsToast();
    }

    public void onEvent(int EventType, int isLast, int arg2, AndroidJavaObject BundleObj)
    {
        
    }

    public void OnVolumeChanged(int volume)
    {
//        ("音量"+volume).ShowAsToast();
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
