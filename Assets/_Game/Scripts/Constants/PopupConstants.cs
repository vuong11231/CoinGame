using System.Collections;
using System.Collections.Generic;

public enum PlayOpenShopFromPopup
{
    None,
    PowerUp,
    RestoreHeart,
    OutOfMoves
}

public static class PopupConstants
{
    public const float TIME_MULTIPLY_APPEAR = 3.0f;
    public const float TIME_MULTIPLY_DISAPPEAR = 2.0f;

    public static PlayOpenShopFromPopup PlayOpenShopFromPopup = PlayOpenShopFromPopup.None;

    //Popup Shop
    public const int SHOP_BONUS_5050 = 15;
    public const int SHOP_BONUS_SAVE = 6;
    public const int GIFT_BONUS_5050 = 10;
    public const int GIFT_BONUS_SAVE = 3;

    public const int SHOP_BUY_SAVE_LIFE_1 = 230;
    public const int SHOP_BUY_SAVE_LIFE_2 = 550;
    public const int SHOP_BUY_SAVE_LIFE_3 = 900;
    public const int SHOP_BUY_SAVE_LIFE_4 = 1380;

    public const int SHOP_BUY_5050_1 = 460;
    public const int SHOP_BUY_5050_2 = 1105;
    public const int SHOP_BUY_5050_3 = 1800;
    public const int SHOP_BUY_5050_4 = 2760;

    public const int SHOP_WATCH_AD = 10;

    public const int TRANSFORM_PLANET = 100;
}
