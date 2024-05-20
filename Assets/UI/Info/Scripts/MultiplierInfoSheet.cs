using CustomSlotsElements;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class MultiplierInfoSheet : PanelUI
{
    [SerializeField] private SlotItemSO[] _slotItemsSO;
    [SerializeField] private SlotItemSO _wildItemSO;
    [SerializeField] private SlotItemSO _wildDoubleItemSO;

    private List<MultiplierInfoPanel> _panels = new();

    private const string WILD_ICON_NAME = "wild-icon";
    private const string WILD_DOUBLE_ICON_NAME = "wild-double-icon";
    private const string BACK_BUTTON_NAME = "back-button";
    private const string NEXT_TAB_BUTTON_NAME = "next-tab-button";

    public event Action OnBackButtonClicked;
    public event Action OnNextTabButtonClicked;

    protected override void OnBind()
    {
        _panels = Query<MultiplierInfoPanel>();

        AssignItemSOsToPanels();

        SetWildIconImage(WILD_ICON_NAME, _wildItemSO);
        SetWildIconImage(WILD_DOUBLE_ICON_NAME, _wildDoubleItemSO);

        RegisterCallbacks();
    }

    protected override void OnUnbind()
    {
        _panels.Clear();
    }

    private void AssignItemSOsToPanels()
    {
        if (_panels.Count != _slotItemsSO.Length)
        {
            Debug.Log($"_panels.Length != _slotItemsSO.Length  ({_panels.Count} != {_slotItemsSO.Length})!");
            return;
        }

        for (int i = 0; i < _slotItemsSO.Length; i++)
        {
            SlotItemSO itemSO = _slotItemsSO[i];
            MultiplierInfoPanel panel = _panels[i];
            panel.SetSlotItemSO(itemSO);
        }
    }

    private void SetWildIconImage(string iconName, SlotItemSO wildItemSO)
    {
        VisualElement icon = GetElement<VisualElement>(iconName);
        icon.style.backgroundImage = new StyleBackground(wildItemSO._sprite);        
        icon.style.unityBackgroundImageTintColor = wildItemSO._color;
    }

    private void RegisterCallbacks()
    {
        RegisterOnClickCallback(BACK_BUTTON_NAME, OnBackButtonClicked);
        RegisterOnClickCallback(NEXT_TAB_BUTTON_NAME, OnNextTabButtonClicked);
    }
}
