using CustomSlotsElements;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckingSchemeSheet : PanelUI
{
    private const string BACK_BUTTON_NAME = "back-button";
    private const string PREV_TAB_BUTTON_NAME = "prev-tab-button";

    [SerializeField] private CheckingSchemeSO[] _checkingSchemesSO;

    public event Action OnBackButtonClicked;
    public event Action OnPrevTabButtonClicked;

    private List<CheckingSchemePanel> _panels = new();

    protected override void OnBind()
    {
        _panels = Query<CheckingSchemePanel>();

        AssignSchemeSOsToPanels(_panels, _checkingSchemesSO);

        RegisterCallbacks();
    }

    protected override void OnUnbind()
    {
        _panels.Clear();
    }

    private void AssignSchemeSOsToPanels(List<CheckingSchemePanel> panels, CheckingSchemeSO[] schemeSOs)
    {
        if (panels.Count != schemeSOs.Length)
        {
            Debug.Log($"_panels.Length != _checkingSchemesSO.Length  ({panels.Count} != {schemeSOs.Length})!");
            return;
        }

        for (int i = 0; i < panels.Count; i++)
        {
            CheckingSchemePanel panel = panels[i];
            CheckingSchemeSO schemeSO = schemeSOs[i];
            panel.SetScheme(schemeSO);
            panel.SetIndex(i + 1);
        }
    }

    private void RegisterCallbacks()
    {
        RegisterOnClickCallback(BACK_BUTTON_NAME, OnBackButtonClicked);
        RegisterOnClickCallback(PREV_TAB_BUTTON_NAME, OnPrevTabButtonClicked);
    }
}
