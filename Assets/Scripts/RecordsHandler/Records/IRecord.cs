using System;

public interface IRecord
{
    public string GetName();
    public string GetDescription();
    public int GetAmount();
    public event Action<IRecord> OnChange;
    public event Action<IRecord> OnBigChange;
    public event Action<IRecord> OnHugeChange;
    public RecordSaveData GetSaveData();
    public void Load(RecordSaveData saveData);
}
