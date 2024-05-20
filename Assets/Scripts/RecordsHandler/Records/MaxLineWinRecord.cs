public class MaxLineWinRecord : BaseRecord, IWinLineListener
{
    private const string DESCRIPTION = "Max gain on single line";
    private const float BIG_CHANGE_THRESHOLD = 1.1f;
    private const float HUGE_CHANGE_THRESHOLD = 1.5f;
    private const int MIN_AMOUNT_FOR_BIG_CHANGE = 1500;

    public override string GetDescription()
    {
        return DESCRIPTION;
    }

    public override string GetName()
    {
        return nameof(MaxLineWinRecord);
    }

    public void OnWinLine(SlotBox.CheckingSchemeResult result, int lineGain)
    {
        RefreshAmount(lineGain);
    }

    protected override bool IsBigChange(int oldValue, int newValue)
    {
        float scale = (float) newValue / oldValue;
        return oldValue >= MIN_AMOUNT_FOR_BIG_CHANGE && scale >= BIG_CHANGE_THRESHOLD;
    }

    protected override bool IsHugeChange(int oldValue, int newValue)
    {
        float scale = (float)newValue / oldValue;
        return oldValue >= MIN_AMOUNT_FOR_BIG_CHANGE && scale >= HUGE_CHANGE_THRESHOLD;
    }
}
