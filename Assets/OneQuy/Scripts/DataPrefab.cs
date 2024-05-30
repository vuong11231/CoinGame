using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SteveRogers;

public class DataPrefab : MonoBehaviour
{
    // varibles

    [Header("Stone")]

    public Sprite airImage;
    public Sprite antimaterImage;
    public Sprite  fireImage;
    public Sprite gravityImage;
    public Sprite iceImage;

    [Header("Stone Effect")]

    public Sprite airImageEffect;
    public Sprite antimaterEffect;
    public Sprite fireImageEffect;
    public Sprite gravityImageEffect;
    public Sprite iceImageEffect;

    [Header("Skin")]

    public Sprite[] skinSprites = null;

    public static Sprite GetSpriteStone(bool isEffectStone, TypePlanet type)
    {
        switch (type)
        {
            case TypePlanet.Antimatter:
                if (isEffectStone)
                    return Instance.antimaterEffect;
                else
                    return Instance.antimaterImage;
            
            case TypePlanet.Gravity:
                if (isEffectStone)
                    return Instance.gravityImageEffect;
                else
                    return Instance.gravityImage;
            
            case TypePlanet.Ice:
                if (isEffectStone)
                    return Instance.iceImageEffect;
                else
                    return Instance.iceImage;
            
            case TypePlanet.Fire:
                if (isEffectStone)
                    return Instance.fireImageEffect;
                else
                    return Instance.fireImage;
            
            case TypePlanet.Air:
                if (isEffectStone)
                    return Instance.airImageEffect;
                else
                    return Instance.airImage;

            default:
                throw new System.NotImplementedException(type.ToString());
        }
    }

    // 

    public static DataPrefab Instance => GameManager.Data;
}
