using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadGameButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.SfxSound("TitleButtonEnter");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.instance.SfxSound("TitleButtonClick");
    }
}
