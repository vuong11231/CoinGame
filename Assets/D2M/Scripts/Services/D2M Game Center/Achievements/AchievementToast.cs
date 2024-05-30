using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;

public struct AchievementData
{
    public string content;
    public Sprite image;
    public Transform parent;
    public float Duration;
}

public class AchievementToast : MonoBehaviour
{
    [HideInInspector]
    public static AchievementToast toast;

    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Text achievementTitle;

    static List<AchievementData> stacks = new List<AchievementData>();
    static bool IsRunning = false;

    private void Awake()
    {
        toast = this;
    }

    private void Init(string content, Sprite image)
    {
        achievementTitle.text = content;
    }

    private void Close()
    {
        Destroy(toast.gameObject);
        toast = null;
        IsRunning = false;
    }

    static void Show(string _content/*, Sprite _image*/, Transform _parent, float _Duration)
    {
        stacks.Add(new AchievementData()
        {
            content = _content,
            //image = _image,
            parent = _parent,
            Duration = _Duration
        });

        if (!IsRunning)
        {
            IsRunning = true;
            Animate();
            //EazySoundManager.PlayUISound(Sounds.Instance.Achievement_Achieved);
        }
    }

    static void Animate()
    {
        if (stacks.Count > 0)
        {
            var data = stacks[0];

            AchievementToast NewToast = Instantiate(Resources.Load<AchievementToast>("Prefabs/Pop-ups/Achievement/Achievement Toast"), data.parent, false);
            NewToast.Init(data.content, data.image);

            // Set mac dinh
            NewToast.canvasGroup.alpha = 0;
            NewToast.rect.anchoredPosition = new Vector2(0, 130);

            //
            if (data.Duration > 0)
            {
                LeanTween.alphaCanvas(NewToast.canvasGroup, 1, 0.3f);
                D2mTween.MoveAnchorY(NewToast.rect, FitReso.IS_IPHONE_X ? -100f : -200f, 0.3f, () =>
                {
                    NewToast.StartCoroutine(DelayAction(() =>
                    {
                        LeanTween.alphaCanvas(NewToast.canvasGroup, 0, 0.15f).delay = 0.15f;
                        D2mTween.MoveAnchorY(NewToast.rect, 130, 0.3f, () =>
                        {
                            NewToast.Close();
                            stacks.RemoveAt(0);
                            Animate();
                        });
                    },
                        data.Duration)
                    );
                });
            }
        }
    }

    public static void ShowShort(string content/*, Sprite image*/, Transform parent, bool ignoreSound = false)
    {
        // Sound
        //if (!ignoreSound)
        //    EazySoundManager.PlaySound(Sounds.Instance.FX_Bell);

        Show(content/*, image*/, parent, 3f);
    }

    public static void ShowLong(string content/*, Sprite image*/, Transform parent, bool ignoreSound = false)
    {
        // Sound
        //if (!ignoreSound)
        //    EazySoundManager.PlaySound(Sounds.Instance.FX_Bell);

        Show(content/*, image*/, parent, 7f);
    }

    public static void ShowForever(string content/*, Sprite image*/, Transform parent, bool ignoreSound = false)
    {
        // Sound
        //if (!ignoreSound)
        //    EazySoundManager.PlaySound(Sounds.Instance.FX_Bell);

        Show(content/*, image*/, parent, -1);
    }

    static IEnumerator DelayAction(Action action, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        action.Invoke();
    }
}