public class MaxWinLines : BaseRecord, ISpinExecutedListener, ISpinFinishedListener, IWinLineListener
{
    private const string DESCRIPTION = "Max win lines in a bet";
    private const float BIG_CHANGE_THRESHOLD = 0.3f;
    private const float HUGE_CHANGE_THRESHOLD = 0.5f;
    private const int LINES_COUNT = 25;
    private const int MIN_AMOUNT_FOR_BIG_CHANGE = 8;

    private int _winLinesCount;
    private int _bet;

    public override string GetDescription()
    {
        return DESCRIPTION;
    }

    public override string GetName()
    {
        return nameof(MaxWinLines);
    }

    public void OnSpinExecuted(int bet)
    {
        _winLinesCount = 0;
        _bet = bet;
    }

    public void OnSpinFinished(int gain)
    {
        RefreshAmount(_winLinesCount);
    }

    public void OnWinLine(SlotBox.CheckingSchemeResult result, int lineGain)
    {
        _winLinesCount++;
    }

    protected override bool IsBigChange(int oldValue, int newValue)
    {
        float scale = (float)(newValue - oldValue) / LINES_COUNT;
        return oldValue >= MIN_AMOUNT_FOR_BIG_CHANGE && scale >= BIG_CHANGE_THRESHOLD;
    }

    protected override bool IsHugeChange(int oldValue, int newValue)
    {
        float scale = (float)(newValue - oldValue) / LINES_COUNT;
        return oldValue >= MIN_AMOUNT_FOR_BIG_CHANGE && scale >= HUGE_CHANGE_THRESHOLD;
    }
}
