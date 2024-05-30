////////////////////////////////////////////////////////////////////////////
// bl_Extensions
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

public static class bl_Extensions
{

    public static void SetBool(this GameObject preb,string key, bool value)
    {
        int bi = (value == true) ? 1 : 0;
        PlayerPrefs.SetInt(key, bi);
    }

    public static bool GetBool(this GameObject preb, string key, bool defaultValue = false)
    {
        int dbi = (defaultValue == true) ? 1 : 0;
        int bi = PlayerPrefs.GetInt(key, dbi);
        bool v = (bi == 1) ? true : false;
        return v;
    }

    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }


    public static Vector3 CalculatePositionFromTransformToRectTransform(this Canvas _Canvas, Vector3 _Position, Camera _Cam)
    {
        Vector3 Return = Vector3.zero;
        if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Return = _Cam.WorldToScreenPoint(_Position);
        }
        else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 tempVector = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_Canvas.transform as RectTransform, _Cam.WorldToScreenPoint(_Position), _Cam, out tempVector);
            Return = _Canvas.transform.TransformPoint(tempVector);
        }

        return Return;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static int ClosestTo(this int[] collection, int target)
    {
        // NB Method will return int.MaxValue for a sequence containing no elements.
        // Apply any defensive coding here as necessary.
        var closest = int.MaxValue;
        var minDifference = int.MaxValue;
        foreach (var element in collection)
        {
            var difference = Mathf.Abs((long)element - target);
            if (minDifference > difference)
            {
                minDifference = (int)difference;
                closest = element;
            }
        }

        return closest;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static int ClosestToIndex(this int[] collection, int target)
    {
        // NB Method will return int.MaxValue for a sequence containing no elements.
        // Apply any defensive coding here as necessary.
        int closest = int.MaxValue;
        int minDifference = int.MaxValue;
        foreach (int element in collection)
        {
            float difference = Mathf.Abs((long)element - target);
            if (minDifference > difference)
            {
                minDifference = (int)difference;
                closest = element;
            }
        }
        List<int> il = new List<int>();
        il.AddRange(collection);

        return il.IndexOf(closest);
    }

    public static int ClosestToIndex(this List<int> collection, int target)
    {
        // NB Method will return int.MaxValue for a sequence containing no elements.
        // Apply any defensive coding here as necessary.
        int closest = int.MaxValue;
        int minDifference = int.MaxValue;
        foreach (int element in collection)
        {
            float difference = Mathf.Abs((long)element - target);
            if (minDifference > difference)
            {
                minDifference = (int)difference;
                closest = element;
            }
        }
        List<int> il = new List<int>();
        il.AddRange(collection);

        return il.IndexOf(closest);
    }

    /// <summary>
    /// Calulates Position for RectTransform.position Mouse Position. Does not Work with WorldSpace Canvas!
    /// </summary>
    /// <param name="_Canvas">The Canvas parent of the RectTransform.</param>
    /// <param name="_Cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
    /// <returns></returns>
    public static Vector3 CalculatePositionFromMouseToRectTransform(this Canvas _Canvas, Camera _Cam)
    {
        Vector3 Return = Vector3.zero;

        if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Return = Input.mousePosition;
        }
        else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 tempVector = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_Canvas.transform as RectTransform, Input.mousePosition, _Cam, out tempVector);
            Return = _Canvas.transform.TransformPoint(tempVector);
        }

        return Return;
    }

    /// <summary>
    /// Calculates Position for "Transform".position from a "RectTransform".position. Does not Work with WorldSpace Canvas!
    /// </summary>
    /// <param name="_Canvas">The Canvas parent of the RectTransform.</param>
    /// <param name="_Position">Position of the "RectTransform" UI element you want the "Transform" object to be placed to.</param>
    /// <param name="_Cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
    /// <returns></returns>
    public static Vector3 CalculatePositionFromRectTransformToTransform(this Canvas _Canvas, Vector3 _Position, Camera _Cam)
    {
        Vector3 Return = Vector3.zero;
        if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Return = _Cam.ScreenToWorldPoint(_Position);
        }
        else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_Canvas.transform as RectTransform, _Cam.WorldToScreenPoint(_Position), _Cam, out Return);
        }
        return Return;
    }
}