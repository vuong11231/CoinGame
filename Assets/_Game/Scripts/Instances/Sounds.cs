using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;

public enum MusicSong
{
    Home,
    Play
}

public class Sounds : Singleton<Sounds>
{
    public static bool IgnorePopupShow = false;
    public static bool IgnorePopupClose = false;

    [Header("Music")]
    public AudioClip Music_Background;

    [Header("Sound")]
    public AudioClip Show_Popup;
    public AudioClip Collect_Element;
   // public AudioClip Collect_Meteorite;
    //public AudioClip SelectPlanet;
    public AudioClip Upgrade;
    public AudioClip ButtonClick;

    public List<AudioClip> ListCollect;
    public List<AudioClip> ListSelect;

    //
    //void OnLevelWasLoaded()
    //{
    //    if (Scenes.Current == SceneName.Gameplay)
    //    {
    //        // Music
    //        EazySoundManager.GlobalMusicVolume = DataGameSave.dataLocal.musicVolume;
    //        EazySoundManager.GlobalSoundsVolume = DataGameSave.dataLocal.soundVolume;
    //        EazySoundManager.GlobalUISoundsVolume = DataGameSave.dataLocal.soundVolume;
    //    }
    //}

    public void PlayCollect()
    {
        int temp = Random.Range(0, ListCollect.Count-1);
        EazySoundManager.PlaySound(ListCollect[temp]);
    }

    public void PlaySelect()
    {
        int temp = Random.Range(0, ListSelect.Count-1 );
       EazySoundManager.PlaySound(ListSelect[temp]);
    }

    public void PlayMusic()
    {
        EazySoundManager.PlayMusic(Music_Background, 1, true, true); 
    }
}