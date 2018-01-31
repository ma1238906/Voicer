using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void ShowAsToast(this string text, AndroidJavaObject activity = null)
    {
        if (activity == null)
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            activity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", text);
            Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT")).Call("show");
        }
        ));

    }

    public static AndroidJavaObject ToJavaString(this string csharpString)
    {
        return new AndroidJavaObject("java.lang.String",csharpString);
    }

    public static void Speak(this string text, string voicer = "xiaoyan")
    {
        IFlyVoice.startSpeaking(text, voicer);
    }
}
