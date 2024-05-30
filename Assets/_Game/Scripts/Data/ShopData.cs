using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopData : Singleton<ShopData>
{
    public List<int> DiamondValue;
    public List<ShopMaterial> Material;
    public int AutoRestoreDiamondPrice;
}

[Serializable]
public class ShopMaterial
{
    public int NumberItem;
    public int Cost;
}
