using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTableManager : MonoBehaviour
{
    [SerializeField] private GameObject _comboPanelTemplate;

    private SpawnerManager _spawnerManager;

    private DropDownMenu _dropDownMenu;
    private List<ComboSlider> _comboSliders;
    private List<string> _names;


    void Start()
    {
        _dropDownMenu = GetComponentInChildren<DropDownMenu>();
        _spawnerManager = FindObjectOfType<SpawnerManager>();
        _comboSliders = new List<ComboSlider>();

        StartCoroutine(StartOnNextFrame());
    }

    private IEnumerator StartOnNextFrame()
    {
        yield return 0;

        _names = _spawnerManager.GetParticleNames();

        foreach (string name in _names)
        {
            _dropDownMenu.AddOption(name);
        }
        OnDropDownChange(_dropDownMenu.GetCurrentValue());
        _dropDownMenu.OnValueChangeAction += OnDropDownChange;
        _dropDownMenu.OnNextFrame();
    }

    private void CreateComboSlider(Vector2 pMinMax, float pValue, (int,int) pName)
    {
        ComboSlider newSlider = Instantiate(_comboPanelTemplate, transform).GetComponent<ComboSlider>();
        newSlider.SetMinMaxValue(pMinMax);
        newSlider.SetValue(pValue);
        newSlider.ChangeValueField(pValue);
        string name = $"{_names[pName.Item1].Substring(0, 1)} x {_names[pName.Item2].Substring(0, 1)}";
        newSlider.ChangeNameField(name);
        newSlider.SetIdentificationCode(pName);
        newSlider.OnValueChangeEvent += OnSliderValueChange;
        _comboSliders.Add(newSlider);
    }

    private void OnDropDownChange(int pNewType)
    {
        for (int i = 0; i < _comboSliders.Count; i++)
        {
            Destroy(_comboSliders[i].gameObject);
        }
        _comboSliders.Clear();

        SortedList<(int, int), float> particleCombos = _spawnerManager.GetGlobalComboTableOfType(pNewType);
        for (int i = 0; i < particleCombos.Count; i++)
        {
            CreateComboSlider(_spawnerManager._minMaxAttractionForce, particleCombos[(pNewType, i)], (pNewType, i));
        }
    }

    private void OnSliderValueChange((int,int) pName, float pValue)
    {
        _spawnerManager.AdjustGlobalComboTable(pName, pValue);
    }
}
