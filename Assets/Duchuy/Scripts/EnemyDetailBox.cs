using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hellmade.Sound;
using SteveRogers;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

public class EnemyDetailBox : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtId;
    public TextMeshProUGUI txtMaterial;
    public Image avatar;
    public GameObject bg1, bg2;

    [HideInInspector]
    public DataGameServer data;

    public void LoadData(DataGameServer data,int index)
    {
        this.data = data;
        CheckBackground(index);
        if (data.Name == "guest")
            txtName.text = DataGameSave.GetGuestName(data.userid);
        else
            txtName.text = data.Name;

        txtLevel.text = "Lv." + data.level.ToString();
        txtId.text = "ID: " + FullLengthNumber(data.userid, 6);

        //Minh.ho: use UpdateAvatar()
        //int avatarId = (data.userid % 3) + 1;
        //avatar.sprite = Resources.Load<Sprite>("avatar_" + avatarId) as Sprite;
        //Minh.ho end
        UpdateAvatar(data.rankChartId);

        var value = (int)data.GetAllMaterialCollect();

        if (value >= 1000)
            txtMaterial.text = SteveRogers.Utilities.MoneyShorter(value, 1);
        else
            txtMaterial.text = FullLengthNumber(value, 6);
    }

    private void CheckBackground(int index)
    {
        if (index % 2 == 0)
        {
            bg1.SetActive(true);
            bg2.SetActive(false);
        }
        else
        {
            bg1.SetActive(false);
            bg2.SetActive(true);
        }
    }
    public string FullLengthNumber(int number, int length)
    {
        string result = number.ToString();
        if (result.Length >= length)
            return result;

        while (result.Length < length)
        {
            result = "0" + result;
        }
        return result;
    }

    public void Attack()
    {
        if (GameStatics.IsAnimating)
            return;

        if (DataGameSave.IsAllPlanetDestroyed()) {
            PopupConfirm.ShowOK(TextConstants.NO_PLANET, TextConstants.NO_PLANET_MESSAGE);
            return;
        }

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;

        if (BtnDailyMission.Instance)
        {
            BtnDailyMission.Instance.CheckDoneQuest();
        }

        DataGameSave.dataEnemy = data;

        Scenes.LastScene = SceneName.Gameplay;
        Scenes.ChangeScene(SceneName.Battle);
    }

    public void AddFriend()
    {
        if (GameStatics.IsAnimating)
            return;

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataLogin.userid);
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("friendid", data.userid);

        ServerSystem.Instance.SendRequest(ServerConstants.ADD_FRIEND, form, () =>
        {
            if (!ServerSystem.Instance.IsResponseOK())
            {
                if (ServerSystem.Instance.ResultDetail() == "friend exist")
                    PopupConfirm.ShowOK(TextConstants.FRIEND, data.Name + TextConstants.FRIEND_EXIST, "OK", null);
                else
                    PopupConfirm.ShowOK(TextMan.Get("Error"), TextConstants.FRIEND_ERROR, "OK", null);
            }
            else
            {
                PopupConfirm.ShowOK(TextConstants.FRIEND, data.Name + " is added to your friend list", "OK", null);
            }
        });
    }

    public void DeleteFriend() {
        if (GameStatics.IsAnimating)
            return;

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

        string question = string.Format(TextMan.Get("Are you sure you want to delete {0}?"), data.Name);

        PopupConfirm.ShowYesNo(TextMan.Get("Delete"), question, DeleteFriend);

        void DeleteFriend() {
            WWWForm form = new WWWForm();
            form.AddField("userid", DataGameSave.dataLogin.userid);
            form.AddField("token", DataGameSave.dataLogin.token.ToString());
            form.AddField("friendid", data.userid);

            ServerSystem.Instance.SendRequest(ServerConstants.DELETE_FRIEND, form, () => { 
                PopupConfirm.ShowOK(
                    TextMan.Get("Information"),
                    string.Format(TextMan.Get("{0} is removed from your friend list"),data.Name));
            });

            FriendSystem.Instance.Refresh();
        }
    }

    public void UpdateAvatar(int indexAvatar)
    {
        avatar.sprite = GameManager.Instance.listAvatar[indexAvatar];
    }
}
