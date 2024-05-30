using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using SteveRogers;

public class AvatarScrollCellView : EnhancedScrollerCellView
{
    public Image imagetop = null;
    public Image imagebottom = null;

    public Image imageticktop = null;
    public Image imagetickbottom = null;

    public Button buttontop = null;
    public Button buttonbottom = null;

    public override void SetData<T>(ref T[] list, int startIndex)
    {
        var arr = list as AvatarScroller.AvatarScrollerItem[];

        var topItem = arr[startIndex];
        var bottomItem = arr[startIndex + 1];

        // main image

        imagetop.sprite = topItem.sprite;
        imagebottom.sprite = bottomItem.sprite;

        // button

        buttontop.interactable = (int)topItem.name < DataGameSave.dataServer.level;
        buttonbottom.interactable = (int)bottomItem.name < DataGameSave.dataServer.level;

        // tick

        var cur = SpaceManager.Instance.PlanetSelect?.DataPlanet;

        if (cur != null)
        {
            imageticktop.gameObject.SetActive(cur.skin == topItem.name);
            imagetickbottom.gameObject.SetActive(cur.skin == bottomItem.name);
        }
    }
}