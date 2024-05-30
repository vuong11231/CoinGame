using Hellmade.Sound;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailboxManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //PlayfabManager.GetUserData(PlayfabManager.PlayFabID,
        //    (result) =>
        //    {
        //        string json = "";

        //        result.Data.TryGetValue(GameConstants.REWARD_RANK_KEY, out UserDataRecord value);

        //        if (value != null)
        //            json = value.Value;

        //        if (string.IsNullOrEmpty(json))
        //        {
        //            Debug.LogError("Don't have data on server");
        //            GameStatics.RewardRankValue = 0;
        //        }
        //        else
        //        {
        //            GameStatics.RewardRankValue = int.Parse(json);
        //        }
        //    },
        //    () => GameStatics.RewardRankValue = 0);
        //Debug.Log("Thong bao from Duchuy: Mail box start!");
    }

    public void OnMail()
    {
        if (GameStatics.IsAnimating)
            return;

        PopupMailbox.Show();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
