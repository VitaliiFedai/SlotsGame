using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class BetTooltip : PanelUI
{
    private const string TEXT_LABEL_NAME = "text-label";

    private const string BACKGROUND_NAME = "background";
    private const float SHOW_DURATION_SEC = 4.0f;
    private const string START_STYLE_NAME = "bet-tooltip-before-start-pos";

    private float _elapsedTime;
    private int _betToLine;

    private void Update()
    {
        _elapsedTime -= Time.deltaTime;

        if (_elapsedTime <= 0)
        {
            VisualElement background = GetElement(BACKGROUND_NAME);
            background.AddToClassList(START_STYLE_NAME);
            background.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);
        }
    }

    private void OnTransitionEnd(TransitionEndEvent evt)
    {
        VisualElement background = GetElement(BACKGROUND_NAME);
        background.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
        enabled = false;
    }

    public void SetBetToLine(int value)
    {
        _betToLine = value;
    }

    public async void ShowWithAnim()
    {
        if (enabled)
        {
            if (_elapsedTime <= 0f)
            {
                VisualElement panel = GetElement(BACKGROUND_NAME);
                panel.RemoveFromClassList(START_STYLE_NAME);
                panel.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
            }
            _elapsedTime = SHOW_DURATION_SEC;
            return;
        }

        _elapsedTime = SHOW_DURATION_SEC;

        enabled = true;

        VisualElement background = GetElement(BACKGROUND_NAME);
        if (!background.ClassListContains(START_STYLE_NAME))
        {
            background.AddToClassList(START_STYLE_NAME);
        }
        await Task.Yield();
        await Task.Yield();
        background.RemoveFromClassList(START_STYLE_NAME);
    }

    protected override void OnBind()
    {
        GetElement<Label>(TEXT_LABEL_NAME).text = $"Bet to Line = {_betToLine}\n\r\n\rLines count = 25\n\r\n\rTotal Bet: {_betToLine} * 25 = {25 * _betToLine}";
    }

    protected override void OnUnbind()
    {
    }
}
