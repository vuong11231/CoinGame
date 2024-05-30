using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using JetBrains.Annotations;

public class DItemHolder : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private float POS_X_FIX = 100f;
    private float POS_Y_FIX = 100f;

    public BlackHole blackHole;
    public Image vitualItem;
    public GameObject spawnObj;
    public TextMeshProUGUI quantityText;
    public BlackHole.Kind_Of_Stone kind_Of_Stone;
    RectTransform rect;


    private void Start()
    {
        vitualItem.enabled = false;
        rect = vitualItem.GetComponent<RectTransform>();

        int i = (int)kind_Of_Stone;

        if (i > 5 && blackHole.GetAmount(i) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        blackHole.current = (int)kind_Of_Stone;
        rect.anchoredPosition = new Vector2(eventData.position.x - POS_X_FIX, eventData.position.y - POS_Y_FIX);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        blackHole.current = (int)kind_Of_Stone;
        vitualItem.enabled = true;
        vitualItem.sprite = GetComponent<Image>().sprite;
        rect.anchoredPosition = new Vector2(eventData.position.x - POS_X_FIX, eventData.position.y - POS_Y_FIX);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        blackHole.current = (int)kind_Of_Stone; 
        vitualItem.enabled = false;
        
        if (blackHole.CurrentAmount == 0)
        {
            PopupConfirm.ShowOK("Oops!",TextConstants.NO_STONE);
            return;
        }    

        Vector3 position = Camera.main.ScreenToWorldPoint(eventData.position);
        position.z = 0;

        GameObject go = Instantiate(spawnObj, position, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = blackHole.stoneImages[blackHole.current];
        var spinEffect = go.GetComponent<BlackHoleSpinEffect>();
        spinEffect.enabled = true;
        spinEffect.blackHole = blackHole;
        spinEffect.StartSpinning();

        quantityText.text = blackHole.CurrentAmount.ToString();
    }
}
