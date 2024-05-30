using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SteveAdmob

using GoogleMobileAds.Api;

[CreateAssetMenu(fileName = "AdmobConfigs", menuName = "Steve Rogers/Create Admob Configs")]
public class AdmobConfigs : ScriptableObject
{
    [Header("general")]

    [SerializeField]
    public bool simulate = true;


    [Header("banner")]

    [SerializeField]
    public string bannerID_Android = null;

    [SerializeField]
    public string bannerID_iOS = null;

    [SerializeField]
    public AdPosition bannerInitPosition = AdPosition.Bottom;

    [SerializeField]
    public bool bannerShowWhenStart = true;


    [Header("inter")]

    [SerializeField]
    public string interID_Android = null;

    [SerializeField]
    public string interID_iOS = null;


    [Header("video reward")]

    [SerializeField]
    public string videoRewardId_Android = null;

    [SerializeField]
    public string videoRewardId_iOS = null;
}

#endif // SteveAdmob
