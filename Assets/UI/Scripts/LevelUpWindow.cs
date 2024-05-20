using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class LevelUpWindow : PanelUI
{
    private const string OK_BUTTON_NAME = "ok-button";
    private const string LEVEL_LABEL_NAME = "level-label";

    public event Action OnOkButtonClicked;

    private int _level;
    private bool _okButtonClicked;

    public async Task Show(int level, int betPerLine)
    {
        _level = level;
        _okButtonClicked = false;
        enabled = true;
        Refresh();
        while (!_okButtonClicked)
        {
            await Task.Yield();
        }
        enabled = false;
    }

    protected override void OnBind()
    {
        Refresh();
        RegisterOnClickCallback(OK_BUTTON_NAME, OnOkButtonClickPerformed);
    }

    protected override void OnUnbind()
    {
    }

    private void Refresh()
    {
        if (GetRoot() != null)
        {
            GetElement<Label>(LEVEL_LABEL_NAME).text = _level.ToString();
        }
    }

    private void OnOkButtonClickPerformed()
    {
        OnOkButtonClicked?.Invoke();
        _okButtonClicked = true;
    }
}
