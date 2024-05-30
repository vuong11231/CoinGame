using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using SteveRogers;
using System;

public class PlanetCellView : EnhancedScrollerCellView
{
    public SpriteAnim spriteAnim = null;
    public Image image = null;    
    public Transform fatherEffect = null;

    private PlanetController controller = null;
    private TypePlanet currentEffect = TypePlanet.Default;

    public override void SetData<T>(ref T[] list, int startIndex)
    {
        var l = list as PlanetCellViewData[];
        var data = l[startIndex];
        
        if (data.controller)
        {            
            spriteAnim.Set(data.controller.DataPlanet.skin.ToString());
            PopupUpgradePlanet._Instance.planetEffectTabSpriteAnim.Set(data.controller.DataPlanet.skin.ToString());
            image.color = Color.white;
        }
        else
        {
            spriteAnim.Pause();
            image.color = Utilities.HexToColor("3D3D3D");
            spriteAnim.transform.localScale = Vector3.one * 0.7f;
        }

        controller = data.controller;

        // reset effect 

        foreach (Transform t in fatherEffect)
            t.gameObject.SetActive(false);

        currentEffect = TypePlanet.Default;
    }

    private void UpdateEffect()
    {
        if (!controller || currentEffect == controller.Type)
            return;

        currentEffect = controller.Type;

        foreach (Transform t in fatherEffect)
            t.gameObject.SetActive(false);

        if (controller.Type < TypePlanet.Default)
            fatherEffect.GetChild((int)controller.Type).gameObject.SetActive(true);
    }

    private void Update()
    {
        if (controller)
        {            
            float scale = controller.Manager.IndexSpace == PlanetScroller.currentPosIndex ? 1f : 0.7f;

            if (spriteAnim.transform.localScale.x != scale)
                spriteAnim.transform.localScale = Vector3.one * scale;

            if (controller.DataPlanet != null)
                spriteAnim.Set(controller.DataPlanet.skin.ToString());

            UpdateEffect();
        }
    }
}