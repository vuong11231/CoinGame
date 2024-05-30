#if AES_ENCRYPTOR

using AvoEx;
using UnityEngine;
using System.IO;

namespace SteveRogers
{
/// <summary>
/// AesEncryptor.cs chỉnh keyString cho app mới (nếu thích, ko thì thôi)
/// </summary>
public class CryptorManager : SingletonPersistent<CryptorManager>
{
    [SerializeField] private bool deleteTempFolderOnDestroy = true;
    private static byte[] key = null;

    private static byte[] Key
    {
        get
        {
            if (key == null)
            {
                key = Resources.Load<TextAsset>("CryptorKey").bytes;

                if (key == null && CheatManager.Instance)
                    throw new System.Exception("AesCryptorManager - CryptorKey null!");
            }

            return key;
        }
    }

    private void OnDestroy()
    {
        if (deleteTempFolderOnDestroy)
        {
            Debug.LogWarning("deleted temp folder");
            Directory.Delete(Utilities.GetRuntimeTempFolderPath(false), true);
        }
    }

    public static byte[] ReadAllBytes(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw GlobalSteves.ExceptionMan.GetException("AesCryptorManager", "ReadAllBytes - File is not exist!");
        }

        var b = Utilities.ReadAllBytes(filepath);
        return AesEncryptor.DecryptIV(b, Key);
    }

    public static string ReadAllText(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw GlobalSteves.ExceptionMan.GetException("AesCryptorManager", "ReadAllText - File is not exist!");
        }
        
        var b = Utilities.ReadAllText(filepath);
        return AesEncryptor.DecryptIV(b, Key);
    }

    public static void WriteAllText(string filepath, string text)
    {
         Utilities.WriteAllText(filepath, AesEncryptor.EncryptIV(text, Key));
    }

    public static void WriteAllBytes(string filepath, byte[] data)
    {
         File.WriteAllBytes(filepath, AesEncryptor.EncryptIV(data, Key));
    }

    public static string CreateDecryptFile(string inFilepath)
    {
        var b = ReadAllBytes(inFilepath);

        if (b == null)
        {
                throw GlobalSteves.ExceptionMan.GetException("AesCryptorManager", "CreateDecryptFile - Fail! " + inFilepath);
        }

        string path = null;

        do
        {
            path = Utilities.GetRuntimeTempFolderPath(true) + Path.GetRandomFileName();
        }
        while (File.Exists(path));
        
        Utilities.CheckAndCreateDirectory(Utilities.GetRuntimeTempFolderPath(false));
        File.WriteAllBytes(path, b);
        return path;
    }
}
}

#endif



//public string smallFile;
//public string largeFile;
//private void Start()
//{
//    var small = Utilities.ReadAllBytes(smallFile);
//    var data_small = AesEncryptor.EncryptIV(small, Key);
//    Debug.LogWarning(data_small.Length - small.Length);
//    Utilities.WriteAllBytes("smalllll", data_small);
//    Utilities.WriteAllBytes("smalllll-ori", small);

//    var large = Utilities.ReadAllBytes(largeFile);
//    var data_large = AesEncryptor.EncryptIV(large, Key);
//    Debug.LogWarning(data_large.Length - large.Length);
//    Utilities.WriteAllBytes("largeeeeee", data_large);
//    Utilities.WriteAllBytes("largeeeeee-ori", large);
//}