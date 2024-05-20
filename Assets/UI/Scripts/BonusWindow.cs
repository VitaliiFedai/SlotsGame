using System;

public class BonusWindow : PanelUI
{
    private const string BACK_BUTTON_NAME = "back-button";

    public event Action OnBackButtonClicked;

    protected override void OnBind()
    {
        RegisterCallbacks();
    }

    protected override void OnUnbind()
    {
    }

    private void RegisterCallbacks()
    {
        RegisterOnClickCallback(BACK_BUTTON_NAME, OnBackButtonClicked);
    }
}
