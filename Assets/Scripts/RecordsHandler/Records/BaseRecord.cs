using System;

public abstract class BaseRecord : IRecord
{
    public event Action<IRecord> OnChange;
    public event Action<IRecord> OnBigChange;
    public event Action<IRecord> OnHugeChange;

    private int _amount;

    public abstract string GetName();

    public int GetAmount()
    {
        return _amount;
    }

    public abstract string GetDescription();

    public RecordSaveData GetSaveData()
    {
        return new RecordSaveData(GetName(), _amount);
    }

    public void Load(RecordSaveData saveData)
    {
        if (saveData.recordName != GetName())
        {
            throw new Exception($"WrongSaveData for {GetName()}! It is a {saveData.recordName} instead.");
        }
        _amount = saveData.amount;
    }

    protected void RefreshAmount(int amount)
    {
        if (amount > _amount)
        {
            int oldValue = _amount;
            _amount = amount;

            OnChange?.Invoke(this);

            if (IsHugeChange(oldValue, _amount)) 
            {
                OnHugeChange?.Invoke(this);
            }
            else if (IsBigChange(oldValue, _amount))
            {
                OnBigChange?.Invoke(this);
            }
        }
    }

    protected abstract bool IsBigChange(int oldValue, int newValue);
    protected abstract bool IsHugeChange(int oldValue, int newValue);
}
