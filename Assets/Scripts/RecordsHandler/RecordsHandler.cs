using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecordsHandler : MonoBehaviour
{
    private List<IRecord> _records = new();

    [SerializeField] private Game _game;

    public event Action<IRecord> OnChange;
    public event Action<IRecord> OnBigChange;
    public event Action<IRecord> OnHugeChange;

    public IEnumerable<IRecord> Records => _records;

    public IEnumerable<RecordSaveData> GetSaveData()
    {
        foreach (IRecord record in _records)
        { 
            yield return record.GetSaveData();
        }
    }

    public void LoadFromSavedData(IEnumerable<RecordSaveData> saveData)
    {
        foreach (RecordSaveData data in saveData)
        {
            IRecord record = _records.First(record => record.GetName() == data.recordName);
            record?.Load(data);
        }
    }

    public void PrintRecords()
    {
        Debug.Log(" ======  RECORDS  ======");
        foreach (IRecord record in _records)
        {
            Debug.Log(record.GetDescription());
            Debug.Log("    - " + record.GetAmount().ToString());
        }
        Debug.Log(" =======================");
    }


    private void Awake()
    {
        AddRecord(new MaxCoinsCount());
        AddRecord(new MaxGainRecord());
        AddRecord(new MaxLineWinRecord());
        AddRecord(new MaxSurplusBetsInARow());
        AddRecord(new MaxScarceBetsInARow());
        AddRecord(new MaxWinLines());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        { 
            PrintRecords();
        }
    }

    private void OnEnable()
    {
        foreach (IRecord record in _records)
        {
            SurscribeRecord(record);
        }
    }

    private void OnDisable()
    {
        foreach (IRecord record in _records)
        {
            UnsurscribeRecord(record);
        }
    }

    private void SurscribeRecord(IRecord record)
    {
        if (record is IGainChangedListener gainListener)
        {
            _game.OnGainChanged += gainListener.OnGainChanged;
        }
        if (record is ICoinsChangedListener coinsListener)
        {
            _game.OnCoinsChanged += coinsListener.OnCoinsChanged;
        }
        if (record is ISpinExecutedListener spinExecutedListener)
        {
            _game.OnSpinExecuted += spinExecutedListener.OnSpinExecuted;
        }
        if (record is ISpinFinishedListener spinFinishedListener)
        {
            _game.OnSpinFinished += spinFinishedListener.OnSpinFinished;
        }
        if (record is IWinLineListener winLineListener)
        {
            _game.OnWinLine += winLineListener.OnWinLine;
        }
    }

    private void UnsurscribeRecord(IRecord record)
    {
        if (record is IGainChangedListener gainListener)
        {
            _game.OnGainChanged -= gainListener.OnGainChanged;
        }
        if (record is ICoinsChangedListener coinsListener)
        {
            _game.OnCoinsChanged -= coinsListener.OnCoinsChanged;
        }
        if (record is ISpinExecutedListener spinExecutedListener)
        {
            _game.OnSpinExecuted -= spinExecutedListener.OnSpinExecuted;
        }
        if (record is ISpinFinishedListener spinFinishedListener)
        {
            _game.OnSpinFinished -= spinFinishedListener.OnSpinFinished;
        }
        if (record is IWinLineListener winLineListener)
        {
            _game.OnWinLine -= winLineListener.OnWinLine;
        }
    }

    private void AddRecord(IRecord record)
    {
        _records.Add(record);
        record.OnChange += OnRecordChangePerformed;
        record.OnBigChange += OnBigRecordChangePerformed;
        record.OnHugeChange += OnHugeRecordChangePerformed;
    }

    private void OnRecordChangePerformed(IRecord record)
    {
        OnChange?.Invoke(record);

        Debug.Log($"RecordChanged {record.GetName()} NewValue = {record.GetAmount()}");
    }

    private void OnBigRecordChangePerformed(IRecord record)
    {
        OnBigChange?.Invoke(record);
        Debug.Log($"BIG CHANGE! {record.GetName()} NewValue = {record.GetAmount()}");
    }

    private void OnHugeRecordChangePerformed(IRecord record)
    {
        OnHugeChange?.Invoke(record);
        Debug.Log($"HUGE CHANGE! {record.GetName()} NewValue = {record.GetAmount()}");
    }
}
