using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SliderDragging : MonoBehaviour, IPointerUpHandler, IPointerDownHandler 
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Sliding start");
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Sliding finished");
    }
}