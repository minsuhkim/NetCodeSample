using System;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

public static class Logger
{
    //Info
    // 입력: 메시지
    [Conditional("DEV_VER")]
    public static void Info(string message)
    {
        // 0에 타임스탬프
        // 1에 메시지
        Debug.LogFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss.fff"), message);
    }
    //Warning
    [Conditional("DEV_VER")]
    public static void Warning(string message)
    {
        // 0에 타임스탬프
        // 1에 메시지
        Debug.LogWarningFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss.fff"), message);
    }
    //Error
    public static void Error(string message)
    {
        // 0에 타임스탬프
        // 1에 메시지
        Debug.LogErrorFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss.fff"), message);
    }
}