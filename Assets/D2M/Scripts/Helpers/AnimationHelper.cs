using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine.Events;
using Hellmade.Sound;
using SteveRogers;

public static class AnimationHelper
{
    static float multiply = -1f;

    #region Pop-ups
    public static void AnimatePopupShowScaleFull(MonoBehaviour Root, Image background, GameObject Popup, CanvasGroup PopupCanvas, float speed = 1.0f, Action completeCallback = null, float multipleSuitIPhoneX = 1f)
    {
        if (multiply == -1)
        {
            if (FitReso.IS_IPAD)
                multiply = 0.875133f;
            else if (FitReso.IS_IPHONE_X)
                multiply = 0.8836f;
            else if (FitReso.IS_ASPECT_9_18_ABOVE)
                multiply = 0.85f;
            else if (FitReso.IS_TABLET_7)
                multiply = 0.97f;
            else if (FitReso.IS_TABLET_10)
                multiply = 0.85f;
            else
                multiply = 1f;
        }

        ///
        /// Setup
        ///
        GameStatics.IsAnimating = true;

        Popup.SetActive(true);
        Popup.transform.localScale = Vector3.zero;
        PopupCanvas.alpha = 0;

        ///
        /// Animate
        ///

        // Background
        background.gameObject.SetActive(true);
        background.color = new Color(0, 0, 0, 0);


        LeanTween.alpha(background.GetComponent<RectTransform>(), 0.8f, 0.125f / speed).setOnComplete(delegate ()
        {

            // Popup
            if (completeCallback != null)
                LeanTween.scale(Popup.GetComponent<RectTransform>(), Vector3.one * multiply, 1 / speed).setEaseOutBack().setOnComplete(() =>
                {
                    GameStatics.IsAnimating = false;
                    if (completeCallback != null)
                        completeCallback.Invoke();
                }
                 ).delay = 0.25f / speed;
            else
            {
                if (!Popup || !Popup.GetComponent<RectTransform>())
                {
                    return;
                }
                LeanTween.scale(Popup.GetComponent<RectTransform>(), Vector3.one * multiply, 1 / speed)
                    .setOnComplete(() =>
                    {
                        GameStatics.IsAnimating = false;
                    })
                    .setEaseOutBack().delay = 0.25f / speed;
            }

            LeanTween.alphaCanvas(PopupCanvas, 1, 0.3f / speed);

            //         Root.StartCoroutine(CanvasAlpha(PopupCanvas, true, 0.6f, 0));
        });
    }

    public static void AnimatePopupShowScaleHalf(MonoBehaviour Root, Image background, GameObject Popup, CanvasGroup PopupCanvas, float speed = 1.0f, Action completeCallback = null, float multipleSuitIPhoneX = 1f)
    {
        // Sound
        //if (!Sounds.IgnorePopupShow)
        //    EazySoundManager.PlaySound(Sounds.Instance.Popup_Show);
        //else
        //    Sounds.IgnorePopupShow = false;

        //
        
        multiply = -1;

        if (Root is PopupUpgradePlanet)
        {
            multiply = 1.15f;
        }
        else if (Root is PopupName || 
                Root is PopupConfirm)
        {
            multiply = 1f;
        }
        else if (Root is PopupDailyMission)
        {
            multiply = 1.15f;
        }
        else if (Root is PopupUpgradeSun)
        {
            multiply = 0.9f;
        }
        else if (Root is PopupAchievements)
        {
            multiply = 1f;
        }
        else if (Root is PopupBattle)
        {
            multiply = 1f;
        }
        else if (Root is PopupSetting)
        {
            multiply = 1f;
        }
        else if (multiply == -1)
        {
            if (FitReso.IS_IPAD)
                multiply = 0.76f /*0.875133f*/;
            else if (FitReso.IS_IPHONE_X)
                multiply = 0.8836f;
            else if (FitReso.IS_ASPECT_9_18_ABOVE)
                multiply = 0.85f;
            else if (FitReso.IS_TABLET_7)
                multiply = 0.97f;
            else if (FitReso.IS_TABLET_10)
                multiply = 0.85f;
            else
                multiply = 1f;
        }

        GameStatics.IsAnimating = true;

        Popup.SetActive(true);
        Popup.transform.localScale = Vector3.one * multiply * 0.75f;
        PopupCanvas.alpha = 0;

        ///
        /// Animate
        ///

        // Background
        background.gameObject.SetActive(true);
        //background.color = new Color(background.color.r, background.color.g, background.color.b, );


        //LeanTween.alpha(background.GetComponent<RectTransform>(), 0.8f, 0.1f / speed);

        LeanTween.delayedCall(0.05f / speed, () =>
        {
            // Popup
            if (completeCallback != null)
                LeanTween.scale(Popup.GetComponent<RectTransform>(), Vector3.one * multiply, 1 / speed).setEaseOutBack().setOnComplete(() =>
                {
                    if (completeCallback != null)
                        completeCallback.Invoke();

                    GameStatics.IsAnimating = false;
                }

                    ).delay = 0.25f / speed;
            else
            {
                if (!Popup || !Popup.GetComponent<RectTransform>())
                {
                    return;
                }

                LeanTween.scale(Popup.GetComponent<RectTransform>(), Vector3.one * multiply, 1 / speed)
                    .setEaseOutBack()
                    .setOnComplete(() =>
                    {
                        GameStatics.IsAnimating = false;
                    })
                    .delay = 0.25f / speed;
            }

            LeanTween.alphaCanvas(PopupCanvas, 1, 0.3f / speed);
        });
    }

    public static void AnimatePopupCloseScaleFull(MonoBehaviour Root, Image background, GameObject Popup, CanvasGroup PopupCanvas, float speed = 1.0f, Action completeCallback = null)
    {
        if (multiply == -1)
        {
            if (FitReso.IS_IPAD)
                multiply = 0.875133f;
            else if (FitReso.IS_IPHONE_X)
                multiply = 0.8836f;
            else if (FitReso.IS_ASPECT_9_18_ABOVE)
                multiply = 0.85f;
            else if (FitReso.IS_TABLET_7)
                multiply = 0.97f;
            else if (FitReso.IS_TABLET_10)
                multiply = 0.85f;
            else
                multiply = 1f;
        }


        ///
        /// Popup
        ///
        GameStatics.IsAnimating = true;

        LeanTween.scale(Popup.GetComponent<RectTransform>(), Vector3.zero, 0.3f / speed).setEaseInBack();

        LeanTween.alphaCanvas(PopupCanvas, 0, 0.125f / speed).setOnComplete(delegate ()
        {
            // invisible popup
            Popup.SetActive(false);

            ///
            /// Background
            ///
            LeanTween.alpha(background.GetComponent<RectTransform>(), 0, 0.125f / speed).setOnComplete(delegate ()
            {

                background.gameObject.SetActive(false);
                GameStatics.IsAnimating = false;

                if (completeCallback != null)
                    completeCallback.Invoke();

            });

        }).delay = 0.175f / speed;
    }

    public static void AnimatePopupCloseScaleHalf(MonoBehaviour Root, Image background, GameObject Popup, CanvasGroup PopupCanvas, float speed = 1.0f, Action completeCallback = null)
    {
        // Sound
        //if (!Sounds.IgnorePopupClose)
        //    EazySoundManager.PlaySound(Sounds.Instance.Popup_Close);
        //else
        //    Sounds.IgnorePopupClose = false;

        //
        if (multiply == -1)
        {
            if (FitReso.IS_IPAD)
                multiply = 0.875133f;
            else if (FitReso.IS_IPHONE_X)
                multiply = 0.8836f;
            else if (FitReso.IS_ASPECT_9_18_ABOVE)
                multiply = 0.85f;
            else if (FitReso.IS_TABLET_7)
                multiply = 0.97f;
            else if (FitReso.IS_TABLET_10)
                multiply = 0.85f;
            else
                multiply = 1f;
        }

        ///
        /// Popup
        ///
        GameStatics.IsAnimating = true;

        LeanTween.scale(Popup.GetComponent<RectTransform>(), Vector3.one * multiply * 0.75f, 0.3f / speed).setEaseInBack();

        LeanTween.alphaCanvas(PopupCanvas, 0, 0.125f / speed).setOnComplete(delegate ()
        {
            GameStatics.IsAnimating = false;

            // invisible popup
            Popup.SetActive(false);

            background.gameObject.SetActive(false);

            if (completeCallback != null)
                completeCallback.Invoke();

            ///
            /// Background
            ///
            //LeanTween.alpha(background.GetComponent<RectTransform>(), 0, 0.125f / speed).setOnComplete(delegate () {

            //    background.gameObject.SetActive(false);

            //    if (completeCallback != null)
            //        completeCallback.Invoke();

            //});

        }).delay = 0.175f / speed;
    }
    #endregion

    #region Objects
    public static IEnumerator BlinkUI(Image image, float distance, float duration, UnityAction callback = null)
    {
        bool reverse = false;
        Color cOn = new Color(1, 1, 1, 1);
        Color cOff = new Color(1, 1, 1, 0);

        float elapsedTimeDuration = 0;
        float elapsedTimeDistance = 0;

        while (true)
        {
            // Distance
            elapsedTimeDistance += Time.deltaTime;

            if (elapsedTimeDistance >= distance)
            {
                elapsedTimeDistance = 0;

                if (!reverse)
                    image.color = cOff;
                else
                    image.color = cOn;

                reverse = !reverse;
            }

            // duration
            elapsedTimeDuration += Time.deltaTime;
            if (elapsedTimeDuration >= duration)
            {
                break;
            }

            yield return 0;
        }

        image.color = cOn;

        if (callback != null)
            callback.Invoke();

        yield return null;
    }

    public static IEnumerator ScaleInstantLoopPingPong(GameObject go, float from, float to, float durationEachLoop)
    {
        if (LeanTween.isTweening(go))
            LeanTween.cancel(go);

        // Reset All
        go.transform.localScale = Vector3.one * from;

        // Animate

        while (true)
        {
            yield return new WaitForSeconds(durationEachLoop / 2);
            go.transform.localScale = Vector3.one * to;
            yield return new WaitForSeconds(durationEachLoop / 2);
            go.transform.localScale = Vector3.one * from;
        }
    }

    public static IEnumerator ScalePingPong(GameObject go, float from, float to, int jumps, float durationEachLoop, UnityAction callbackDone)
    {
        if (LeanTween.isTweening(go))
            LeanTween.cancel(go);

        // Reset All
        go.transform.localScale = Vector3.one * from;

        // Animate
        for (int i = 0; i < jumps; i++)
        {
            LeanTween.scale(go, Vector3.one * to, durationEachLoop / 2f).setOnComplete(() =>
            {
                LeanTween.scale(go, Vector3.one * from, durationEachLoop / 2f);
            });

            yield return new WaitForSeconds(durationEachLoop);
        }

        if (callbackDone != null)
            callbackDone.Invoke();
    }

    public static void ScaleHalfShow(GameObject go, CanvasGroup goCanvasGroup, float originScale, float speed = 1.0f, Action completeCallback = null)
    {
        ///
        /// Setup
        ///
        go.SetActive(true);
        go.transform.localScale = Vector3.one * originScale * 0.5f;
        goCanvasGroup.alpha = 0;

        ///
        /// Animate
        ///
        LeanTween.scale(go.GetComponent<RectTransform>(), Vector3.one * originScale, 1 / speed).setEaseOutBack().setOnComplete(() =>
        {
            if (completeCallback != null)
                completeCallback.Invoke();
        }).delay = 0.25f / speed;

        LeanTween.alphaCanvas(goCanvasGroup, 1, 0.3f / speed);
    }

    public static void ScaleHalfClose(GameObject go, CanvasGroup goCanvasGroup, float originScale, float speed = 1.0f, Action completeCallback = null)
    {
        LeanTween.scale(go.GetComponent<RectTransform>(), Vector3.one * originScale * 0.5f, 0.3f / speed).setEaseInBack();

        LeanTween.alphaCanvas(goCanvasGroup, 0, 0.125f / speed).setOnComplete(delegate ()
        {
            // invisible
            go.SetActive(false);

            if (completeCallback != null)
                completeCallback.Invoke();

        }).delay = 0.175f / speed;
    }

    public static void ScaleFullShow(GameObject go, CanvasGroup goCanvasGroup, float originScale, float speed = 1.0f, Action completeCallback = null)
    {
        ///
        /// Setup
        ///
        go.SetActive(true);
        go.transform.localScale = Vector3.zero;
        goCanvasGroup.alpha = 0;

        ///
        /// Animate
        ///
        LeanTween.scale(go.GetComponent<RectTransform>(), Vector3.one * originScale, 1 / speed).setEaseOutBack().setOnComplete(() =>
        {
            if (completeCallback != null)
                completeCallback.Invoke();
        }).delay = 0.25f / speed;

        LeanTween.alphaCanvas(goCanvasGroup, 1, 0.3f / speed);
    }

    public static void ScaleFullClose(GameObject go, CanvasGroup goCanvasGroup, float originScale, float speed = 1.0f, Action completeCallback = null)
    {
        LeanTween.scale(go.GetComponent<RectTransform>(), Vector3.zero, 0.3f / speed).setEaseInBack();

        LeanTween.alphaCanvas(goCanvasGroup, 0, 0.125f / speed).setOnComplete(delegate ()
        {
            // invisible
            go.SetActive(false);

            if (completeCallback != null)
                completeCallback.Invoke();

        }).delay = 0.175f / speed;
    }

    #endregion

    #region Game Center
    public static void AnimateGameCenterShow(MonoBehaviour Root, Image background, GameObject Popup, CanvasGroup PopupCanvas, float from, float to, float speed = 1.0f, Action completeCallback = null)
    {
        ///
        /// Setup
        ///
        GameStatics.IsAnimating = true;

        Popup.SetActive(true);
        Popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, from);
        PopupCanvas.alpha = 0;

        ///
        /// Animate
        ///

        // Background
        background.gameObject.SetActive(true);
        background.color = new Color(0, 0, 0, 0);

        LeanTween.alpha(background.GetComponent<RectTransform>(), 0.8f, 0.125f / speed).setOnComplete(delegate ()
        {
            // Popup
            D2mTween.MoveAnchorY(Popup.GetComponent<RectTransform>(), to, 1 / speed, () =>
            {
                GameStatics.IsAnimating = false;
                if (completeCallback != null)
                    completeCallback.Invoke();
            });
            LeanTween.alphaCanvas(PopupCanvas, 1, 0.3f / speed);
        });
    }

    public static void AnimateGameCenterClose(MonoBehaviour Root, Image background, GameObject Popup, CanvasGroup PopupCanvas, float to, float speed = 1.0f, Action completeCallback = null)
    {
        ///
        /// Popup
        ///
        GameStatics.IsAnimating = true;
        D2mTween.MoveAnchorY(Popup.GetComponent<RectTransform>(), to, 1 / speed, () =>
            {
                LeanTween.alphaCanvas(PopupCanvas, 0, 0.125f / speed).setOnComplete(delegate ()
                {
                    // invisible popup
                    Popup.SetActive(false);

                    ///
                    /// Background
                    ///
                    LeanTween.alpha(background.GetComponent<RectTransform>(), 0, 0.125f / speed).setOnComplete(delegate ()
                    {
                        GameStatics.IsAnimating = false;

                        background.gameObject.SetActive(false);

                        if (completeCallback != null)
                            completeCallback.Invoke();
                        Popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, to);
                    });

                }).delay = 0.175f / speed;
            });
    }
    #endregion
}