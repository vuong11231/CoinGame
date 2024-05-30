using System;
using UnityEngine;
using System.Collections.Generic;
//using Hellmade.Sound;
//using EazyTools.SoundManager;

public abstract class   Popups : MonoBehaviour
{
    public CanvasGroup Root;
    public GameObject Background;
    public CanvasGroup Panel;

    #region Canvas Get
    static GameObject _canvasPopup;
    public static GameObject CanvasPopup
    {
        get
        {
            if (_canvasPopup == null)
                _canvasPopup = GameObject.FindWithTag("Canvas Popup");

            return _canvasPopup;
        }
    }

    static GameObject _canvasPopup2;
    public static GameObject CanvasPopup2
    {
        get
        {
            if (_canvasPopup2 == null)
                _canvasPopup2 = GameObject.FindWithTag("Canvas Popup 2");

            return _canvasPopup2;
        }
    }

    static GameObject _canvasPopup3;
    public static GameObject CanvasPopup3
    {
        get
        {
            if (_canvasPopup3 == null)
                _canvasPopup3 = GameObject.FindWithTag("Canvas Popup 3");

            return _canvasPopup3;
        }
    }

    static GameObject _canvasToast;
    public static GameObject CanvasToast
    {
        get
        {
            if (_canvasToast == null)
                _canvasToast = GameObject.FindWithTag("Canvas Toast");

            return _canvasToast;
        }
    }

    static GameObject _canvasFX;
    public static GameObject CanvasFX
    {
        get
        {
            if (_canvasFX == null)
                _canvasFX = GameObject.FindWithTag("Canvas FX");

            return _canvasFX;
        }
    }
    #endregion

    /// <summary>
    /// Check if another popup is showed, ignore the appearance
    /// </summary>
    public static bool IsShowed = false;
    public static bool IgnoreSoundOneTime = false;

    public static List<Popups> Stacks = new List<Popups>();
    public static bool DestroyIgnore = false;
    public static bool StackIgnore = false;
    public static bool DisappearIgnore = false;
    public static bool DisappearIgnoreNextAppear = false;
    static bool _isSetStacks = false;

    // *** onequy rem to remove warning logs ***
    //private void OnLevelWasLoaded(int level)
    //{

    //    if (_isSetStacks == false)
    //    {
    //        Stacks.Clear();

    //        _isSetStacks = true;

    //        StackIgnore = false;
    //        DisappearIgnore = false;
    //        DisappearIgnoreNextAppear = false;
    //    }
    //}

    private void OnDestroy()
    {
        if(_isSetStacks && !DestroyIgnore)
        {
            //
            GameStatics.IsAnimating = false;

            //
            Stacks.Clear();

            _isSetStacks = false;

            StackIgnore = false;
            IsShowed = false;
            DisappearIgnore = false;
            DisappearIgnoreNextAppear = false;
        }
        else if(DestroyIgnore)
        {
            DestroyIgnore = false;
        }
    }

    public static void Reset()
    {
        Stacks.Clear();
    }

    /// <summary>
    /// Abstract functions
    /// 
    public abstract void NextStep (object value = null);

    /// <summary>
    /// Virtual functions
    /// </summary>
    /// 
    public virtual void Appear () {

        Root.alpha = 1;
        Root.interactable = true;
        Root.blocksRaycasts = true;
        //Sound
       // EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
        // sound
        //if (IgnoreSoundOneTime == false)
        //{
        //    SoundManager.PlaySound(Sounds.Instance.Popup_Open);
        //}
        //else
        //IgnoreSoundOneTime = true;

        IsShowed = true;

        //Debug.LogError("StackIgnore: " + StackIgnore);

        if (!StackIgnore)
            Stacks.Add(this);
        else
             StackIgnore = false;

        //Debug.LogError("Appear: " + Stacks.Count);
    }

    public virtual void Disappear ()
    {

        Root.alpha = 0;
        Root.interactable = false;
        Root.blocksRaycasts = false;

        // sound
        //if (IgnoreSoundOneTime == false)
        //    SoundManager.PlaySound(Sounds.Instance.Popup_Exit);
        //else
        //IgnoreSoundOneTime = true;

        IsShowed = false;

        //Debug.LogError("test STACKIGNORE: " + StackIgnore);
        if (!DisappearIgnore)
        {
            if (!StackIgnore)
            {
                Stacks.Remove(this);

                if (DisappearIgnoreNextAppear)
                {
                    DisappearIgnoreNextAppear = false;
                }
                else
                {
                    if (Stacks.Count > 0)
                    {
                        StackIgnore = true;
                        Stacks[Stacks.Count - 1].Appear();
                    }
                }
            }
            else
            {

                StackIgnore = false;
            }
        }
        else
        {
            DisappearIgnore = false;
            Stacks.Clear();
        }

        //Debug.LogError("Stack Count: " + Stacks.Count);
    }

    public virtual void Disable ()
    {
        Root.alpha = 0;
        Root.interactable = false;
        Root.blocksRaycasts = false;

        IsShowed = false;

        if (!DisappearIgnore)
        {
            if (!StackIgnore)
            {
                Stacks.Remove(this);

                if (Stacks.Count > 0)
                {
                    StackIgnore = true;
                    Stacks[Stacks.Count - 1].Appear();
                }
            }
            else
            {
                StackIgnore = false;
            }
        }
        else
        {
            DisappearIgnore = false;
            Stacks.Clear();
        }
    }
}

