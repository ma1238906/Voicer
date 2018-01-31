﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class OutLog : MonoBehaviour
{
#if UNITY_ANDROID

    static List<string> mLines = new List<string>();
    static List<string> mWriteTxt = new List<string>();
    private string outpath;
    void Start()
    {
        //Application.persistentDataPath Unity中只有这个路径是既可以读也可以写的。  
        outpath = Application.persistentDataPath + "/outLog.txt";
        //每次启动客户端删除之前保存的Log  
        if (File.Exists(outpath))
        {
            File.Delete(outpath);
        }
        Application.logMessageReceived += HandleLog;
    }

    void Update()
    {
        if (mWriteTxt.Count > 0)
        {
            string[] temp = mWriteTxt.ToArray();
            foreach (string t in temp)
            {
                using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))
                {
                    writer.WriteLine(t);
                }
                mWriteTxt.Remove(t);
            }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        mWriteTxt.Add(logString);
        if (type == LogType.Error || type == LogType.Exception || type==LogType.Log)
        {
            Log(logString);
            Log(stackTrace);
        }
    }

    public void Log(params object[] objs)
    {
        string text = "";
        for (int i = 0; i < objs.Length; ++i)
        {
            if (i == 0)
            {
                text += objs[i].ToString();
            }
            else
            {
                text += ", " + objs[i];
            }
        }
        if (Application.isPlaying)
        {
            if (mLines.Count > 20)
            {
                mLines.RemoveAt(0);
            }
            mLines.Add(text);
        }
    }
#endif
}
