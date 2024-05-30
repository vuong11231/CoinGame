using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SteveUnityAds

[CreateAssetMenu(fileName = "UnityAdvertisementConfigs", menuName = "Steve Rogers/Create Unity Advertisement Configs")]
public class UnityAdvertisementConfigs : ScriptableObject
{
    [Header("video reward")]

    public string videoRewardPlacement = "rewardedVideo";


    [Header("inter")]

    public string interPlacement = "inter";
}

#endif // SteveUnityAds
