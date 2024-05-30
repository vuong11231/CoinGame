using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRewardManager : MonoBehaviour
{
    public GameObject content;
    public Sprite diamond;
    public Sprite rockRandom;
    public Sprite rockEffect;
    public Sprite recoverTime;
    public Sprite planet;
    public Sprite asteroidBelt;

    private void Start()
    {
        var childCount = content.transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var child = content.transform.GetChild(i);
            LoginRewardItem item = child.GetComponent<LoginRewardItem>();
            if (item)
            {
                item.Setup(RewardLogin.ListReards[i]);
                item.setIcon(this.getSpriteFromRewardInfo(RewardLogin.ListReards[i]));
            }
        }
    }

    Sprite getSpriteFromRewardInfo(RewardLogin info)
    {
        if (info.type == RewardLogin.Type.AsteroidBelt)
        {
            return asteroidBelt;
        }
        else if (info.type == RewardLogin.Type.Diamond)
        {
            return diamond;
        }
        else if (info.type == RewardLogin.Type.Planet)
        {
            return planet;
        }
        else if (info.type == RewardLogin.Type.RecoverTime)
        {
            return recoverTime;
        }
        else if (info.type == RewardLogin.Type.RockEffect)
        {
            return rockEffect;
        }
        else if (info.type == RewardLogin.Type.RockRandom)
        {
            return rockRandom;
        }
        else
        {
            return null;
        }
    }
}
