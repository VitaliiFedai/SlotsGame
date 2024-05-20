using System;

[Serializable]
public struct RecordSaveData
{
    public string recordName;
    public int amount;

    public RecordSaveData(string recordName, int amount)
    { 
        this.recordName = recordName;
        this.amount = amount;
    }
}
