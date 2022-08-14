using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Sprite baseImage;
    public Sprite selectedImage;
    public Image image;

    void Awake()
    {
        if(image == null)
        {
            image = GetComponent<Image>();
        }
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = selectedImage;
        SoundManager.instance.SfxSound("TitleButtonEnter");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.instance.SfxSound("TitleButtonClick");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = baseImage;
    }

    

}
