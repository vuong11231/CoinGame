using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToRotate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    Vector3 startDrag;
    Vector3 pivot;

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 current = eventData.position;
        //Debug.Log("event data position: " + current);
        //Debug.Log("curent transform position: " + transform.GetComponent<RectTransform>().anchoredPosition);
        float angle = Vector3.Angle( current - pivot, startDrag - pivot);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pivot = GetComponent<RectTransform>().anchoredPosition;
        startDrag = eventData.position;
        Debug.Log(startDrag);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
