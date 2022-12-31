using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ChangeImageColor : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    private bool currentImageColor = false;
    
    public void SetImageColorActive()
    {
        _image.color = _activeColor;
    }
    public void SetImageColorinActive()
    {
        _image.color = _inactiveColor;
    }

    public void SwitchCurrentImageColor()
    {
        if (currentImageColor)
        {
            currentImageColor = false;
            SetImageColorinActive();
        }
        else
        {
            currentImageColor = true;
            SetImageColorActive();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
