using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceBehaviour : MonoBehaviour
{
    public Button voiceToStringButton;
    public Button stringToVoiceButton;
    public Text voiceResultText;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        stringToVoiceButton.onClick.AddListener((() =>
        {
            string hello = "您好，感谢您对Mr.Ma的支持！";
            IFlyVoice.startSpeaking(hello);
            hello.ShowAsToast();
            voiceResultText.text += hello;
            Debug.Log(hello);
        }));

        voiceToStringButton.onClick.AddListener((() =>
        {
            IFlyVoice.startRecognize();
        }));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}