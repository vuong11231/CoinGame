using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public static class D2mTween
{
    public static void MoveAnchorX(RectTransform objectRect, float To, float duration, Action onComplete)
    {
        var tween = D2mTweenAction.AddTweenAction(objectRect.gameObject, (value) =>
        {
            objectRect.anchoredPosition = new Vector2(value, objectRect.anchoredPosition.y);
        }, 
            new Vector2(objectRect.anchoredPosition.x, To), 
            duration);

        // Delegate
        if (onComplete != null)
            tween.onFinish = onComplete;
    }

    public static void MoveAnchorY(RectTransform objectRect, float To, float duration, Action onComplete)
    {
        var tween = D2mTweenAction.AddTweenAction(objectRect.gameObject, (value) =>
        {
            objectRect.anchoredPosition = new Vector2(objectRect.anchoredPosition.x, value);
        },
            new Vector2(objectRect.anchoredPosition.y, To),
            duration);

        // Delegate
        if (onComplete != null)
            tween.onFinish = onComplete;
    }
}