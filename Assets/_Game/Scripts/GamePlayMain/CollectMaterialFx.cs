using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lean.Pool;
using UnityEngine.UI;

public class CollectMaterialFx : MonoBehaviour
{
    public TextMeshPro TxtAmount;

    public TextMeshProUGUI txtAmountBlackHole;
    public SpriteRenderer imgBlackHole;

    public void SetData(float Amount, Vector3 pos)
    {
        Amount *= DataGameSave.dataLocal.itemX2.multiplyNumber;
        DataGameSave.dataLocal.M_Material += (int)Amount;
        TxtAmount.text = Amount + " <sprite=\"meteorite\" index=5>";
        transform.LeanScale(Vector3.one,2);
        LeanTween.cancel(gameObject);
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + 1, 1f).setOnComplete(() =>
        {
            LeanPool.Despawn(gameObject);
        });
    }

    public void SetFx(float Amount, TypePlanet Type)
    {
        Amount *= DataGameSave.dataLocal.itemX2.multiplyNumber;
        switch (Type)
        {
            case TypePlanet.Air:
                {
                    TxtAmount.text = Amount + " " + TextConstants.M_Air;
                    break;
                }
            case TypePlanet.Antimatter:
                {
                    TxtAmount.text = Amount + " " + TextConstants.M_Antimatter;
                    break;
                }
            case TypePlanet.Default:
                {
                    TxtAmount.text = Amount + " " + TextConstants.M_Mater;
                    break;
                }
            case TypePlanet.Ice:
                {
                    TxtAmount.text = Amount + " " + TextConstants.M_Ice;
                    break;
                }
            case TypePlanet.Fire:
                {
                    TxtAmount.text = Amount + " " + TextConstants.M_Fire;
                    break;
                }
            case TypePlanet.Gravity:
                {
                    TxtAmount.text = Amount + " " + TextConstants.M_Gravity;
                    break;
                }
        }
        transform.LeanScale(Vector3.one, 0);
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, MoneyManager.Instance.DesMoney.transform.position, 3f).setEaseInCubic().setOnComplete(() =>
        {
            LeanPool.Despawn(gameObject);
        });

    }

    public void SetFxMeteor(string type)
    {
        if (type == "material")
            TxtAmount.text = TextConstants.M_Mater;
        else if (type == "air")
            TxtAmount.text = TextConstants.M_Air;
        else if(type == "antimater")
            TxtAmount.text = TextConstants.M_Antimatter;
        else if(type == "fire")
            TxtAmount.text = TextConstants.M_Fire;
        else if(type == "gravity")
            TxtAmount.text = TextConstants.M_Gravity;
        else if(type == "ice")
            TxtAmount.text = TextConstants.M_Ice;
        else if(type == "diamond")
            TxtAmount.text = TextConstants.M_Mater;
        else
            TxtAmount.text = TextConstants.M_Mater;

        transform.LeanScale(Vector3.one, 0);
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, MoneyManager.Instance.DesMoney.transform.position, 3f).setEaseInCubic().setOnComplete(() =>
        {
            LeanPool.Despawn(gameObject);
        });
    }

    public void SetFxBlackHole(Sprite img, int amount)
    {
        txtAmountBlackHole.text = amount.ToString();
        imgBlackHole.sprite = img;

        transform.LeanScale(Vector3.one, 0);
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, MoneyManager.Instance.DesMoney.transform.position, 3f).setEaseInCubic().setOnComplete(() =>
        {
            LeanPool.Despawn(gameObject);
        });
    }
}
