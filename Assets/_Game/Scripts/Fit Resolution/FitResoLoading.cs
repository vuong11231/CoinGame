using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class FitReso
{
    public static bool IS_VERY_TIGHT_WIDTH_PLATFORM = false;
    public static bool IS_ASPECT_9_18_ABOVE = false;
    public static bool IS_IPHONE_5S_BELOW = false;
    public static bool IS_IPHONE_X = false;
    public static bool IS_IPAD = false;
    public static bool IS_TABLET_7 = false;
    public static bool IS_TABLET_10 = false;
}

public class FitResoLoading : MonoBehaviour
{
    private void Start()
    {
        float aspect = (float)Screen.width / (float)Screen.height;

        // Check is iPhone 5s & below
        if (SystemInfo.deviceModel == "iPhone6,1" || SystemInfo.deviceModel == "iPhone6,2" || // iPhone 5s
            SystemInfo.deviceModel == "iPhone5,3" || SystemInfo.deviceModel == "iPhone5,4" || // iPhone 5c
            SystemInfo.deviceModel == "iPhone5,1" || SystemInfo.deviceModel == "iPhone5,2" || // iPhone 5
            SystemInfo.deviceModel == "iPhone4,1") // iPhone 4s
        {
            FitReso.IS_IPHONE_5S_BELOW = true;
        }

        //
        // is Iphone X & above
        //
       
        //
        // 9 : 19,5
        //
        else if (aspect <= 0.47f)
        {
            FitReso.IS_VERY_TIGHT_WIDTH_PLATFORM = true;
            FitReso.IS_ASPECT_9_18_ABOVE = true;
        }

        //
        // 9 : 18.5
        //
        else if (aspect <= 0.49f)
        {
            FitReso.IS_VERY_TIGHT_WIDTH_PLATFORM = true;
            FitReso.IS_ASPECT_9_18_ABOVE = true;
        }


        //
        // 9 : 18
        //
        else if (aspect <= 0.54f)
        {
            FitReso.IS_ASPECT_9_18_ABOVE = true;
        }

        //r
        // 9 : 16
        //
        else if (aspect <= 0.563f)
        {
            FitReso.IS_ASPECT_9_18_ABOVE = false;
            FitReso.IS_IPHONE_5S_BELOW = false;
            FitReso.IS_IPHONE_X = false;
            FitReso.IS_IPAD = false;
            FitReso.IS_TABLET_7 = false;
            FitReso.IS_TABLET_10 = false;
        }

        //
        // 10 : 16 (Tablet 7-inch)
        //
        else if (aspect <= 0.625f)
        {
            FitReso.IS_TABLET_7 = true;
        }


        //
        // Tablet 10-inch
        //
        else if (aspect <= 0.7f)
        {
            FitReso.IS_TABLET_10 = true;
        }

        //
        // 3 : 4
        //
        else if (aspect <= 0.8f)
        {
            FitReso.IS_IPAD = true;
        }
    }
}