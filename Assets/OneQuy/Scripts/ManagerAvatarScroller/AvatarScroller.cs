using UnityEngine;
using EnhancedUI.EnhancedScroller;
using SteveRogers;

public class AvatarScroller : MonoBehaviour
{
    public EnhancedScroller mainScroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public Sprite[] avatars = null;

    private Scroller<AvatarScrollerItem> scroller = null;
    private System.Collections.Generic.List<AvatarScrollerItem> list = new System.Collections.Generic.List<AvatarScrollerItem>();

    private void Start()
    {
        scroller = new Scroller<AvatarScrollerItem>(mainScroller, cellViewPrefab, 200, 2);

        for (PLanetName n = 0; n < PLanetName._Count; n++)
        {
            list.Add(new AvatarScrollerItem
            {
                name = n,
                sprite = avatars[(int)n]
            });
        }
    }

    public void Refresh()
    {
        scroller.Set(list);
    }

    public class AvatarScrollerItem
    {
        public PLanetName name;
        public Sprite sprite = null;
    }
}