using System.IO;
using UnityEngine;

public class IFlyVoice
{
    const string AppID = "59549e41";
    //---------------------------------------
    const string SpeechConstant_PARAMS = "params";
    const string SpeechConstant_ENGINE_TYPE = "engine_type";
    const string SpeechConstant_TYPE_CLOUD = "cloud";
    const string SpeechConstant_VOICE_NAME = "voice_name";
    const string SpeechConstant_SPEED = "speed";
    const string SpeechConstant_PITCH = "pitch";
    const string SpeechConstant_VOLUME = "volume";
    const string SpeechConstant_STREAM_TYPE = "stream_type";
    const string SpeechConstant_KEY_REQUEST_FOCUS = "request_audio_focus";
    const string SpeechConstant_AUDIO_FORMAT = "audio_format";
    const string SpeechConstant_TTS_AUDIO_PATH = "tts_audio_path";
    const string SpeechConstant_RESULT_TYPE = "result_type";
    const string SpeechConstant_LANGUAGE = "language";
    const string SpeechConstant_ACCENT = "accent";
    const string SpeechConstant_VAD_BOS = "vad_bos";
    const string SpeechConstant_VAD_EOS = "vad_eos";
    const string SpeechConstant_ASR_PTT = "asr_ptt";
    const string SpeechConstant_ASR_AUDIO_PATH = "asr_audio_path";

    //参数名称使用cmd编译java文件，运行打印得到 编译时需要导入Msc.jar -cp Msc.jar
    private const string SpeechConstant_IVW_SST = "sst";//唤醒模式：wakeup 只是唤醒不追加其他语音，oneshot 唤醒后直接添加其他语音，enroll训练识别词
    private const string SpeechConstant_IVW_THRESHOLD = "ivw_threshold";//唤醒的阈值，就相当于门限值，当用户输入的语音的置信度大于这一个值的时候，才被认定为成功唤醒。
    private const string SpeechConstant_IVW_KEEPALIVE = "keep_alive";//持续唤醒
    private const string SpeechConstant_IVW_IVW_MODE = "ivw_net_mode";//关闭闭环优化 若开启代表会对用户的识别词发音识别进行优化，以提高识别率(ps 开启优化功能，允许向服务端发送本地挑选数据。需要开发者自行进行优化资源的查询下载，及对资源的使用进行管理)
    private const string SpeechConstant_IV2_IVW_RES_PATH = "ivw_res_path";//识别词资源路径
    //---------------------------------------

    //AndroidJavaClass
    private static AndroidJavaClass UnityPlayer;

    private static AndroidJavaObject currentActivity;

    private static AndroidJavaClass SpeechSynthesizer;
    private static AndroidJavaClass SpeechRecognizer;
    private static AndroidJavaClass SpeechWakeup;
    //AndroidJavaObject
    private static AndroidJavaObject mTts;
    private static AndroidJavaObject mIat;
    private static AndroidJavaObject mIvw;

    private static XfInitListener mInitListener;
    private static XfSynthesizerListener mTtsListener;
    private static XfRecognizerListener mRecognizerListener;
    private static XfWakeupListener mWakeupListener;
    //to judge if the program has execute initFlyVoice before speak or recognize
    private static bool inited = false;

    public static void initIFlyVoice()
    {
#if UNITY_ANDROID
        //Initialize AndroidJavaClass(Please do not delete the commended codes for that those code are for test and check)
        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        //
        string param = "appid=" + AppID + ",engine_mode=msc";
        // Set APPID
        AndroidJavaClass SpeechUtility = new AndroidJavaClass("com.iflytek.cloud.SpeechUtility");

        SpeechUtility.CallStatic<AndroidJavaObject>("createUtility",
                currentActivity.Call<AndroidJavaObject>("getApplicationContext"),
                new AndroidJavaObject("java.lang.String", param)
        );

        //Init Listeners
        mInitListener = new XfInitListener();
        mTtsListener = new XfSynthesizerListener();
        mRecognizerListener = new XfRecognizerListener();
        mWakeupListener = new XfWakeupListener();

        //Init mTts and mIat
        if (mInitListener != null)
        {
            SpeechSynthesizer = new AndroidJavaClass("com.iflytek.cloud.SpeechSynthesizer");
            SpeechRecognizer = new AndroidJavaClass("com.iflytek.cloud.SpeechRecognizer");
            SpeechWakeup = new AndroidJavaClass("com.iflytek.cloud.VoiceWakeuper");

            mTts = SpeechSynthesizer.CallStatic<AndroidJavaObject>("createSynthesizer", currentActivity, mInitListener);
            mIat = SpeechRecognizer.CallStatic<AndroidJavaObject>("createRecognizer", currentActivity, mInitListener);
            mIvw = SpeechWakeup.CallStatic<AndroidJavaObject>("createWakeuper", currentActivity, mInitListener);
            //创建唤醒对象后直接在init中启用（这个项目中没有单独在程序初始化时调用init，而是写作static方法）
            setIvwParam();
            int code = mIvw.Call<int>("startListening", mWakeupListener);
            if (code==0)
            {
                "唤醒初始化完成".ShowAsToast();
            }
        }
        else
        {
            "初始化失败".ShowAsToast();
        }
        inited = true;
#endif
    }

    public static void startSpeaking(string text, string voicer = "xiaoyan")
    {
        if (!inited)
        {
            initIFlyVoice();
        }
        setTtsParam(voicer);
        int code = mTts.Call<int>("startSpeaking", text.ToJavaString(), mTtsListener);
        if (code != 0)
        {
            Debug.LogError("SpeakFailed,ErrorCode" + code);
        }
    }

    public static void startRecognize(string language = "mandarin")
    {
        if (!inited)
        {
            initIFlyVoice();
        }
        setIatParam(language);//设置识别参数及语种
        int ret = mIat.Call<int>("startListening", mRecognizerListener);
        if (ret != 0)
        {
            Debug.LogError("听写失败,错误码：" + ret);
        }
        else
        {
            "请开始说话".ShowAsToast(currentActivity);
        }
    }

    private static void setTtsParam(string voicer)
    {
        if (mTts == null)
        {
            Debug.LogError("mTts=null");
            return;
        }
        //清空参数
        mTts.Call<bool>("setParameter", SpeechConstant_PARAMS.ToJavaString(), null);

        //设置合成
        //设置使用云端引擎
        mTts.Call<bool>("setParameter", SpeechConstant_ENGINE_TYPE.ToJavaString(), SpeechConstant_TYPE_CLOUD.ToJavaString());

        //设置发音人
        mTts.Call<bool>("setParameter", SpeechConstant_VOICE_NAME.ToJavaString(), voicer.ToJavaString());
        //设置合成语速
        mTts.Call<bool>("setParameter", SpeechConstant_SPEED.ToJavaString(), "50".ToJavaString());
        //设置合成音调
        mTts.Call<bool>("setParameter", SpeechConstant_PITCH.ToJavaString(), "50".ToJavaString());
        //设置合成音量
        mTts.Call<bool>("setParameter", SpeechConstant_VOLUME.ToJavaString(), "50".ToJavaString());
        //设置播放器音频流类型
        mTts.Call<bool>("setParameter", SpeechConstant_STREAM_TYPE.ToJavaString(), "3".ToJavaString());

        // 设置播放合成音频打断音乐播放，默认为true
        mTts.Call<bool>("setParameter", SpeechConstant_KEY_REQUEST_FOCUS.ToJavaString(), "true".ToJavaString());

        // 设置音频保存路径，保存音频格式支持pcm、wav，设置路径为sd卡请注意WRITE_EXTERNAL_STORAGE权限
        // 注：AUDIO_FORMAT参数语记需要更新版本才能生效
        mTts.Call<bool>("setParameter", SpeechConstant_AUDIO_FORMAT.ToJavaString(), "wav".ToJavaString());

        AndroidJavaClass Environment = new AndroidJavaClass("android.os.Environment");
        AndroidJavaObject rootDir = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<AndroidJavaObject>("toString");
        rootDir = rootDir.Call<AndroidJavaObject>("concat", "/msc/tts.wav".ToJavaString());

        mTts.Call<bool>("setParameter", SpeechConstant_TTS_AUDIO_PATH.ToJavaString(), rootDir);
    }

    private static void setIatParam(string lag)
    {
        // 清空参数
        mIat.Call<bool>("setParameter", SpeechConstant_PARAMS.ToJavaString(), null);
        // 设置引擎
        mIat.Call<bool>("setParameter", SpeechConstant_ENGINE_TYPE.ToJavaString(), SpeechConstant_TYPE_CLOUD.ToJavaString());
        // 设置返回结果格式
        mIat.Call<bool>("setParameter", SpeechConstant_RESULT_TYPE.ToJavaString(), "json".ToJavaString());

        if (lag.Equals("en_us"))
        {
            // 设置语言
            mIat.Call<bool>("setParameter", SpeechConstant_LANGUAGE.ToJavaString(), "en_us".ToJavaString());
        }
        else
        {
            // 设置语言
            mIat.Call<bool>("setParameter", SpeechConstant_LANGUAGE.ToJavaString(), "zh_cn".ToJavaString());
            // 设置语言区域
            mIat.Call<bool>("setParameter", SpeechConstant_ACCENT.ToJavaString(), lag.ToJavaString());
        }

        // 设置语音前端点:静音超时时间，即用户多长时间不说话则当做超时处理
        mIat.Call<bool>("setParameter", SpeechConstant_VAD_BOS.ToJavaString(), "4000".ToJavaString());

        // 设置语音后端点:后端点静音检测时间，即用户停止说话多长时间内即认为不再输入， 自动停止录音
        mIat.Call<bool>("setParameter", SpeechConstant_VAD_EOS.ToJavaString(), "1000".ToJavaString());

        // 设置标点符号,设置为"0"返回结果无标点,设置为"1"返回结果有标点
        mIat.Call<bool>("setParameter", SpeechConstant_ASR_PTT.ToJavaString(), "1".ToJavaString());

        // 设置音频保存路径，保存音频格式支持pcm、wav，设置路径为sd卡请注意WRITE_EXTERNAL_STORAGE权限
        // 注：AUDIO_FORMAT参数语记需要更新版本才能生效
        mIat.Call<bool>("setParameter", SpeechConstant_AUDIO_FORMAT.ToJavaString(), "wav".ToJavaString());

        AndroidJavaClass Environment = new AndroidJavaClass("android.os.Environment");
        AndroidJavaObject rootDir = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<AndroidJavaObject>("toString");
        rootDir = rootDir.Call<AndroidJavaObject>("concat", "/msc/iat.wav".ToJavaString());
        mIat.Call<bool>("setParameter", SpeechConstant_ASR_AUDIO_PATH.ToJavaString(), rootDir);
    }

    private static void setIvwParam()
    {
        mIvw.Call<bool>("setParameter",SpeechConstant_PARAMS,null);
        mIvw.Call<bool>("setParameter",SpeechConstant_IVW_THRESHOLD.ToJavaString(),"0:0".ToJavaString());
        mIvw.Call<bool>("setParameter", SpeechConstant_IVW_SST.ToJavaString(), "wakeup".ToJavaString());
        mIvw.Call<bool>("setParameter", SpeechConstant_IVW_KEEPALIVE.ToJavaString(), "1".ToJavaString());
        mIvw.Call<bool>("setParameter", SpeechConstant_IVW_IVW_MODE.ToJavaString(), "0".ToJavaString());

        AndroidJavaObject fo = "fo|".ToJavaString();
        AndroidJavaObject paramStr = "/sdcard/msc/ivw/59549e41.jet".ToJavaString();
        AndroidJavaObject verticalSymbol = "|".ToJavaString();
        AndroidJavaObject zere = "0".ToJavaString();
        AndroidJavaObject end = "58560".ToJavaString();
        AndroidJavaObject rootDir = fo.Call<AndroidJavaObject>("concat", paramStr);
        rootDir.Call<AndroidJavaObject>("concat", verticalSymbol);
        rootDir.Call<AndroidJavaObject>("concat", zere);
        rootDir.Call<AndroidJavaObject>("concat", verticalSymbol);
        rootDir.Call<AndroidJavaObject>("concat", end);

        mIvw.Call<bool>("setParameter", SpeechConstant_IV2_IVW_RES_PATH.ToJavaString(), rootDir);
    }
}
