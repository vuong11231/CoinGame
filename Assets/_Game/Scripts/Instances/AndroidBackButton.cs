using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AndroidBackButton : Singleton<AndroidBackButton>
{

    //public GameObject spin;
    public bool Banclick = false;

    int _count;
    Popups _currentPopup;

//#if UNITY_ANDROID || UNITY_EDITOR
//    void Update()
//    {
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.Space))
//#elif UNITY_ANDROID
//        if (Input.GetKeyDown(KeyCode.Escape))
//#endif
//            if (!Banclick && 
//                !GameStatics.IsAnimating && 
//                ((Scenes.Current == SceneName.Home) ||  (Scenes.Current == SceneName.Play)))
//            {
//                _count = Popups.Stacks.Count;
//                if(_count > 0)
//                {
//                    _currentPopup = Popups.Stacks[_count - 1];

//                    // Is Yes No
//                    if(_currentPopup is PopupYesNo)
//                    {
//                        var popup = _currentPopup as PopupYesNo;
//                        if(popup.GetConfirmType() == ConfirmType.Ok)
//                            popup.OnYes();
//                        else
//                            popup.OnNo();
//                    }
//                    // Star Chest
//                    else if(_currentPopup is PopupOpenStarChest)
//                    {
//                        // Do nothing
//                    }
//                    // Play Setting
//                    else if(_currentPopup is PopupQuit)
//                    {
//                        var popup = _currentPopup as PopupQuit;
//                        popup.OnClose();
//                    }
//                    // Play Exit
//                    else if(_currentPopup is PopupPlayExit)
//                    {
//                        var popup = _currentPopup as PopupPlayExit;
//                        popup.OnContinue();
//                    }
//                    // Tutorial
//                    else if(_currentPopup is PopupTutorial)
//                    {
//                        // Do nothing
//                    }
//                    // Play Lose
//                    else if(_currentPopup is PopupLose)
//                    {
//                        // Do nothing
//                    }
//                    // Play Win
//                    else if(_currentPopup is PopupWin)
//                    {
//                        // Do nothing
//                    }
//                    // Play Out Of Moves
//                    else if(_currentPopup is PopupOutOfMoves)
//                    {
//                        // Do nothing
//                    }
//                    // Others
//                    else
//                        Popups.Stacks[_count - 1].Disappear();
//                }
//                else
//                {
//                    // Show Quit popup when at Home
//                    if(Scenes.Current == SceneName.Home)
//                    {
//                        PopupYesNo.ShowQuit(
//                            TextConstants.QuitGameTitle,
//                            TextConstants.QuitGameContent,
//                            Application.Quit);

//                    }
//                    // Show Setting when at Play
//                    else if(Scenes.Current == SceneName.Play)
//                    {
//                        PopupQuit.Instance.Open();
//                    }
//                }
//            }
//    }
//#endif
}