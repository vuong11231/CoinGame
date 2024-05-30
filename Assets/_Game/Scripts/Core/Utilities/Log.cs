
// Contain declaration for Conditional attribute
using System;
using System.Diagnostics;
using System.Net;
using UnityEngine;
// Prevent Type conflict with System.Diagnostics.Log
using Debug = UnityEngine.Debug;


public class Log
{
    [Conditional("ENABLE_LOG")]
    public static void Info(object message)
    {
        Debug.Log("Info : " + message);
    }

    [Conditional("ENABLE_LOG")]
    public static void Warning(object message)
    {
        Debug.LogWarning("Warning : " + message);
    }

    [Conditional("ENABLE_LOG")]
    public static void Error(object message)
    {
        Debug.LogError("Error : " + message);
    }

    public static void LogGitlab(string description)
    {
        string title = description;
        ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => (true);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
        description += "\n\n  ";
        description += "******************* SYSTEM INFO *******************  \n";
        description += "DeviceModel : " + SystemInfo.deviceModel+ "  \n";
        description += "GraphicsDeviceName : " + SystemInfo.graphicsDeviceName+ "  \n";
        description += "GraphicsDeviceType : " + SystemInfo.graphicsDeviceType+ "  \n";
        description += "GraphicsDeviceVersion : " + SystemInfo.graphicsDeviceVersion+ "  \n";
        description += "GraphicsMemorySize : " + SystemInfo.graphicsMemorySize+ "  \n";
        description += "MaxTextureSize : " + SystemInfo.maxTextureSize+ "  \n";
        description += "NPotSupport : " + SystemInfo.npotSupport+ "  \n";
        description += "OperatingSystem : " + SystemInfo.operatingSystem+ "  \n";
        description += "ProcessingType : " + SystemInfo.processorType+ "  \n";
        description += "SystemMemorySize : " + SystemInfo.systemMemorySize+ "  \n";

        var req = WebRequest.Create(new Uri("http://35.198.248.251/api/v4/projects/21/issues?title=" + title + "&description=" + description));
        req.Headers.Add("Private-Token", "EUea8RyqH_Pm-wnwv4rv");
        req.Method = "POST";
        req.GetResponse();
    }
}
