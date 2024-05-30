using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    static Indicator _Instance;

    static void CheckInstance()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<Indicator>("Prefabs/Pop-ups/Indicator/Indicator"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show()
    {
        CheckInstance();
        GameStatics.IsAnimating = true;
        _Instance.gameObject.SetActive(true);
        _Instance.Init();
    }

    public static void Hide()
    {
        if(_Instance != null)
        {
            GameStatics.IsAnimating = false;
            _Instance.gameObject.SetActive(false);
        }
    }

    void Init()
    {
        transform.SetAsLastSibling();
    }
}
