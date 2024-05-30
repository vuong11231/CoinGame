using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SteveRogers;
using TMPro;

public class CheckAvatar : MonoBehaviour
{
    public Image mainAvatar;
    public Text name;
    //public TextMeshProUGUI userid;
   
    void Start()
    {
        if (name && DataGameSave.dataServer != null)
            name.text = DataGameSave.dataServer.Name;

        //userid.text = "ID: " + DataGameSave.dataServer.userid;
        mainAvatar.sprite = GameManager.Instance.listAvatar[0];
        if(DataGameSave.dataLocal.indexAvatar!= DataGameSave.dataServer.rankChartId)
        {
            DataGameSave.dataLocal.indexAvatar = DataGameSave.dataServer.rankChartId;
        }
        UpdateAvatar();
    }
    private void Update()
    {
        UpdateAvatar();
    }
    public void OnClickAvatar()
    {
        PopupSelectAvatar.Show(UpdateAvatar);
    }    
    public void UpdateAvatar()
    {
        if (mainAvatar && GameManager.Instance && GameManager.Instance.listAvatar != null && DataGameSave.dataLocal != null)
            mainAvatar.sprite = GameManager.Instance.listAvatar.GetLastIfOverRange(DataGameSave.dataLocal.indexAvatar);
    }
}
