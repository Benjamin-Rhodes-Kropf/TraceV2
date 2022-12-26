using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectFriendButton : MonoBehaviour
{
    [SerializeField] private Image myImageBG;
    [SerializeField] private Image myImageCircle;
    [SerializeField]private Sprite unselectedBG;
    [SerializeField]private Sprite selectedBG;
    [SerializeField]private Sprite selectedCircle;
    [SerializeField]private Sprite unselectedCircle;

    public bool isSelected;

    public void Select()
    {
        isSelected = true;
        myImageBG.sprite = selectedBG;
        myImageCircle.sprite = selectedCircle;
    }
    public void DeSelect()
    {
        isSelected = false;
        myImageBG.sprite = unselectedBG;
        myImageCircle.sprite = unselectedCircle;
    }

    public void ToggleSelection()
    {
        Debug.Log("SelectFriendButton: Toggling Selection");
        if (isSelected)
        {
            DeSelect();
        }
        else
        {
            Select();   
        }
    }
}
