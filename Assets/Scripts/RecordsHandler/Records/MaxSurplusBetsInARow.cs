public class MaxSurplusBetsInARow : BaseRecord, ISpinExecutedListener, ISpinFinishedListener
{
    private const string DESCRIPTION = "Max surplus bets in a row";
    private const float BIG_CHANGE_THRESHOLD = 1.2f;
    private const float HUGE_CHANGE_THRESHOLD = 2f;
    private const int MIN_AMOUNT_FOR_BIG_CHANGE = 10;

    private int _winBetsInARow;
    private int _bet;

    public override string GetDescription()
    {
        return DESCRIPTION;
    }

    public override string GetName()
    {
        return nameof(MaxSurplusBetsInARow);
    }

    public void OnSpinExecuted(int bet)
    {
        _bet = bet;
    }

    public void OnSpinFinished(int gain)
    {
        if (gain > _bet)
        {
            _winBetsInARow++;
        }
        else
        {
            RefreshAmount(_winBetsInARow);
            _winBetsInARow = 0;
        }
    }

    protected override bool IsBigChange(int oldValue, int newValue)
    {
        float scale = (float)newValue / oldValue;
        return oldValue >= MIN_AMOUNT_FOR_BIG_CHANGE && scale >= BIG_CHANGE_THRESHOLD;
    }

    protected override bool IsHugeChange(int oldValue, int newValue)
    {
        float scale = (float)newValue / oldValue;
        return oldValue >= MIN_AMOUNT_FOR_BIG_CHANGE && scale >= HUGE_CHANGE_THRESHOLD;
    }
}
