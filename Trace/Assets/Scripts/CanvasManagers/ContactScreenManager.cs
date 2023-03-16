using UnityEngine;
using UnityEngine.UI;

public class ContactScreenManager : MonoBehaviour
{
    [Header("Array of Scroll Panels Detail")]
    [SerializeField]
    private ContactPanel[] contactPanels;

    [Header("Scroll Panel which should be enabled in start")]
    [SerializeField]
    private string startingScreenName;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color notActiveColor;

  
    [SerializeField]
    private RectTransform Search_Scroll;

    //Opening the default contact panel when screen is enbaled
    private void OnEnable()
    {
        ResetPanels();
    }
    //Reseting the panels on disbale
    private void OnDisable()
    {
        ResetPanels();
    }
    //For enabling the search scroller
    public void OnSearchBarClicked() {
        CloseOpenedScrollerWhenClickOnSearchAndOpenedSearchScroller();
    }
    //This will close the opened scroller panel when search scroller is enabled
    void CloseOpenedScrollerWhenClickOnSearchAndOpenedSearchScroller() {
        Search_Scroll.gameObject.SetActive(true);
        foreach (var item in contactPanels)
        {
            item.ScreenObject.gameObject.SetActive(false);
            
        }
    }
    //For opening specific panel with panel name
    public void OpenThisScrollerPanel(string panelName) {
        foreach (var item in contactPanels)
        {
            if (item.Name.Equals(panelName))
            {
                item.ScreenObject.gameObject.SetActive(true);
                item.screenButton.image.color = selectedColor;
            }
            else
            {
                item.ScreenObject.gameObject.SetActive(false);
                item.screenButton.image.color = notActiveColor;
            }
        }
        Search_Scroll.gameObject.SetActive(false);
    }
    void ResetPanels()
    {
        foreach (var item in contactPanels)
        {
            if (item.Name.Equals(startingScreenName))
            {
                item.ScreenObject.gameObject.SetActive(true);
                item.screenButton.image.color = selectedColor;
            }
            else
            {
                item.ScreenObject.gameObject.SetActive(false);
                item.screenButton.image.color = notActiveColor;
            }
        }
        Search_Scroll.gameObject.SetActive(false);
    }

}
[System.Serializable]
public class ContactPanel
{
    public string Name;
    public Transform ScreenObject;
    public Button screenButton;
}
