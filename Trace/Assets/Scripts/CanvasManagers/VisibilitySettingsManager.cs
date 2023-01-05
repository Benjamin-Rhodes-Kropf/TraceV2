using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilitySettingsManager : MonoBehaviour
{
    [SerializeField] private List<ChangeImageColor> _visabilityOptions = new List<ChangeImageColor>();
    [SerializeField] private List<ChangeImageColor> _viewRangeOptions = new List<ChangeImageColor>(); 
    
    private void OnEnable()
    {
        foreach (var button in _viewRangeOptions)
        {
            button.SetImageColorinActive();
        }
        foreach (var button in _visabilityOptions)
        {
            button.SetImageColorinActive();
        }
        
        Debug.Log("VisibilitySettingsManager: VisibilitySettingsManager Enabled");
        int rangeValue = PlayerPrefs.GetInt("TraceVisRange");
        _viewRangeOptions[4-rangeValue].SetImageColorActive();
        int visability = PlayerPrefs.GetInt("TraceViewable");
        _visabilityOptions[1-visability].SetImageColorActive();
        
        //ScreenManager.instance.Invoke("CountTanks", 2);
        StartCoroutine(PullUpFriendSelect());
    }
    
    private void OnDisable()
    {
        Debug.Log("VisibilitySettingsManager: VisibilitySettingsManager Disabled");
    }

    public void SetVisabiltyRange(int range)
    {
        PlayerPrefs.SetInt("TraceVisRange", range-1);

        foreach (var button in _viewRangeOptions)
        {
            button.SetImageColorinActive();
        }

        //_viewRangeOptions[range-1].SetImageColorActive();
        Debug.Log("VisibilitySettingsManager: visability range set to:" + range.ToString());
    }
    public void SetVisability(int viewable)
    {
        PlayerPrefs.SetInt("TraceViewable", viewable);
        Debug.Log("VisibilitySettingsManager: trace viewability is set to:" + viewable.ToString());
    }

    IEnumerator PullUpFriendSelect()
    {
        yield return new WaitForSeconds(0.5f);
        ScreenManager.instance.OpenPopup("Friend Select");
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
