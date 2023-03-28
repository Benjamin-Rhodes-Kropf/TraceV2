using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    public Image image;
    public Sprite onSprite;
    public Sprite offSprite;
    public bool toggleValue;
    
    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }
    
    public void toggled()
    {
        toggleValue = toggleValue ? false : true;
        if (toggleValue)
        {
            image.sprite = onSprite;
        }
        else
        {
            image.sprite = offSprite;
        }
    }
}
