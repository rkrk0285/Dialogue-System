using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TouchInputCanvas : MonoBehaviour, IPointerClickHandler
{
    float touchInterval = 0.05f;
    float time = 0;
    
    public bool canClick = true;
    public bool clicked = false;

    void Update()
    {
        time += Time.deltaTime;
    }

    public void OnPointerClick(PointerEventData eventData)
    {       
        if (!canClick) return;
        if (time < touchInterval) return;

        time = 0;
        DialogueManager.Instance.isClicked = true;
        clicked = true;
    }
}