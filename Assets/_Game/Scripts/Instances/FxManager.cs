using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PathologicalGames;

public enum SFX
{
    Star_Burst_Yellow,
    Heart_Burst_Red
}

public class FxManager : Singleton<FxManager>
{
    [Header("SFX")]
    public GameObject sfx_root;
    public ParticleSystem pre_sfx_star_burst_yellow;
    public ParticleSystem pre_sfx_heart_burst_red;

    public static void ShowEffect(SFX sfx, Vector3 position, float scaleFactor = 1f)
    {
        switch (sfx)
        {
            case SFX.Star_Burst_Yellow: PoolManager.Pools["SFX"].Spawn(Instance.pre_sfx_star_burst_yellow, position, Quaternion.identity, Instance.sfx_root.transform).transform.localScale = Vector3.one * scaleFactor; break;
            case SFX.Heart_Burst_Red: PoolManager.Pools["SFX"].Spawn(Instance.pre_sfx_heart_burst_red, position, Quaternion.identity, Instance.sfx_root.transform).transform.localScale = Vector3.one * scaleFactor; break;
        }
    }
}
