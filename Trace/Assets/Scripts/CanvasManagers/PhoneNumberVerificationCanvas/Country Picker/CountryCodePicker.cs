using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class CountryCodePicker : MonoBehaviour
{
    [SerializeField] private TextAsset _textAsset;
    [SerializeField] private TMP_Dropdown _dropdown;
    private CountryCodes[] _codes;
    private void OnEnable()
    {
        GetCodes();
        // _dropdown.options = new List<TMP_Dropdown.OptionData>();
        _dropdown.onValueChanged.AddListener(OnValueChangeDropDown);
        OnValueChangeDropDown(0);
    }

    private void GetCodes()
    {
       _codes= JsonConvert.DeserializeObject<CountryCodes[]>(_textAsset.text);
       _dropdown.options = new List<TMP_Dropdown.OptionData>();
       foreach (var code in _codes)
       {
           TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
           optionData.text = code.countryCode + "," + code.phoneCode;
           _dropdown.options.Add(optionData);
       }
    }


    private void OnValueChangeDropDown(int index)
    {
        _dropdown.captionText.text = _codes[index].phoneCode;
    }
    
}



[System.Serializable]
public class CountryCodes
{
    public string countryCode;
    public string countryName;
    public string phoneCode;
}
