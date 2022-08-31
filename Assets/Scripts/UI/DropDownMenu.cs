using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropDownMenu : MonoBehaviour
{
    public UnityAction<int> OnValueChangeAction;

    private TMP_Dropdown _dropDownComponent;
    private List<string> _options;

    void Start()
    {
        _options = new List<string>();

        _dropDownComponent = GetComponent<TMP_Dropdown>();
        _dropDownComponent.ClearOptions();
    }

    public void OnNextFrame()
    {
        _dropDownComponent.onValueChanged.AddListener(OnValueChangeAction);
    }

    public void AddOption(string pOptionName)
    {
        _dropDownComponent.ClearOptions();

        _options.Add(pOptionName);
        _dropDownComponent.AddOptions(_options);
    }

    public int GetCurrentValue()
    {
        return _dropDownComponent.value;
    }
}
