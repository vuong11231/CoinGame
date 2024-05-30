////////////////////////////////////////////////////////////////////////////
// bl_LovattoMobileUtils
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.IO;

public static class bl_LovattoMobileUtils 
{
    public static bool isSharing = false;

    public static void RateUs(string AppID)
    {
        Application.OpenURL("market://details?id=" + AppID);
    }

    public static void ShareScreenShot()
    {

    }

    public static IEnumerator TakeScreenShotAndShare(float delay)
    {
        isSharing = true;
        yield return new WaitForSeconds(delay);
        int width = Screen.width;
        int height = Screen.height;

        yield return new WaitForEndOfFrame();
        Texture2D textured = new Texture2D(width, height, TextureFormat.RGB24, true);
        textured.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
        textured.Apply();


        byte[] dataToSave = textured.EncodeToPNG();
        string str = "MyRecordImageSideBall.png";
        string path = Application.persistentDataPath + "/" + str;
        File.WriteAllBytes(path, dataToSave);

        if (!Application.isEditor)
        {
            AndroidJavaClass JavaClass2 = new AndroidJavaClass("com.lovattostudio.plugin.MainActivity");
            JavaClass2.CallStatic("ShareImage", "Head Line", "my Subject", path);
            Debug.Log("File exits: " + File.Exists(path) + " --- " + path);
        }
        else
        {
            Application.OpenURL(path);
        }
        isSharing = false;
    }

    public static Quaternion LookAt2D(Transform position,Transform targetPosition)
    {
        Vector3 dir = targetPosition.position - position.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public static Quaternion LookAt2D(Vector3 position, Vector3 targetPosition)
    {
        Vector3 dir = targetPosition - position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}