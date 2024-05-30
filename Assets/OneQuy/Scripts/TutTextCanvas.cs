using SteveRogers;
using UnityEngine;
using UnityEngine.UI;

public class TutTextCanvas : SingletonPersistentStatic<TutTextCanvas>
{
    public Text mainText = null;
    public RectTransform mainTextRect = null;
    public float timeShow = 1;
    public float timeHide = 1;

    void Start()
    {
        Reset();
    }  

    private static void Reset()
    {
        LeanTween.cancel(Instance.mainTextRect);
        Instance.mainText.gameObject.SetActive(false);
    }

    private static bool IsShowing
    {
        get
        {
            return Instance.mainText.gameObject.activeSelf;
        }
    }

    public static void Show(string text, Vector3 localPosition)
    {
        if (IsShowing)
            Reset();

        TutMan.tutDisableTime = Time.time + 3f;

        Instance.mainText.text = text;
        Instance.mainText.SetAlpha(0);
        Instance.mainTextRect.localPosition = localPosition;
        Instance.mainTextRect.gameObject.SetActive(true);

        LeanTween.alphaText(Instance.mainTextRect, 1f, Instance.timeShow);
    }

    public static void Hide()
    {
        if (!IsShowing)
            return;

        LeanTween.cancel(Instance.mainTextRect);

        LeanTween.alphaText(Instance.mainTextRect, 0, Instance.timeHide);
    }
}
