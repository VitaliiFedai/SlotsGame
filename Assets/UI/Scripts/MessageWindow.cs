using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class MessageWindow : PanelUI
{
    private const string OK_BUTTON_NAME = "ok-button";
    private const string TEXT_LABEL_NAME = "text-label";

    public event Action OnOkButtonClicked;

    private string _message;
    private bool _okButtonClicked;

    public async Task Show(string message)
    {
        _message = message;
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
            GetElement<Label>(TEXT_LABEL_NAME).text = _message;
        }
    }

    private void OnOkButtonClickPerformed()
    {
        OnOkButtonClicked?.Invoke();
        _okButtonClicked = true;
    }
}
