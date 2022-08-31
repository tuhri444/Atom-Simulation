using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Slider))]
public class ComboSlider : MonoBehaviour
{
    public UnityAction<float> OnValueChangeAction;
    public UnityAction<(int,int),float> OnValueChangeEvent;

    private Slider _sliderComponent;
    private TMP_Text _nameField;
    private TMP_Text _valueField;
    private (int, int) _identifactionCode;

    void Awake()
    {
        _sliderComponent = GetComponent<Slider>();
        _nameField = GetComponentsInChildren<TMP_Text>()[0];
        _valueField = GetComponentsInChildren<TMP_Text>()[1];
    }

    void Start()
    {
        OnValueChangeAction += ChangeValueField;
        OnValueChangeAction += (a) => { OnValueChangeEvent?.Invoke(_identifactionCode, a); };
        _sliderComponent.onValueChanged.AddListener(OnValueChangeAction);
    }

    public void SetValue(float pValue)
    {
        _sliderComponent.value = pValue;
    }

    public void SetMinMaxValue(Vector2 pMinMax)
    {
        _sliderComponent.minValue = pMinMax.x;
        _sliderComponent.maxValue = pMinMax.y;
    }

    public void ChangeNameField(string pName)
    {
        _nameField.text = pName;
    }

    public void ChangeValueField(float pValue)
    {
        _valueField.text = pValue.ToString("0.##");
    }

    public void SetIdentificationCode((int,int) pCode)
    {
        _identifactionCode = pCode;
    }
}
