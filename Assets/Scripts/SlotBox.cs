using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SlotBox : MonoBehaviour
{
    [Serializable]
    public struct SaveData
    {
        public SlotColumn.SaveData[] SlotColumnsSaveData;

//        public int Value;

        public SaveData(SlotBox slotBox)
        {
            SlotColumnsSaveData = new SlotColumn.SaveData[slotBox._slotColumns.Length];
            int index = 0; ;
            foreach (SlotColumn column in slotBox._slotColumns)
            {
                SlotColumnsSaveData[index++] = new SlotColumn.SaveData(column);
            }
            //Value = 666;
        }

        public void SetSavedDataTo(SlotBox slotBox)
        {
            if (SlotColumnsSaveData != null)
            {
                int index = 0;
                foreach (SlotColumn.SaveData saveData in SlotColumnsSaveData)
                {
                    saveData.SetSavedDataTo(slotBox._slotColumns[index++]);
                }
            }
        }
    }

    public struct CheckingSchemeResult
    {
        public int WinMultiplier;

        public string SchemeName { get; private set; }
        public int MatchedItemsCount => _matchedItems.Count();
        public string MatchedItemName => _matchedItems[0]?.Name;
        public IEnumerable<SlotItem> MatchedItems => _matchedItems;

        private SlotItem[] _matchedItems;

        public CheckingSchemeResult(IEnumerable<SlotItem> matchedItems, string schemeName, int multiplier)
        {
            _matchedItems = matchedItems.ToArray();
            SchemeName = schemeName;
            WinMultiplier = multiplier;
        }

        public int BetMultiplier
        {
            get
            {
                SlotItemSO itemSO = GetItemSO();
                return itemSO != null ? itemSO._betMultiplier[MatchedItemsCount - MIN_MATCHED_COUNT_TO_WIN] : 0;
            }
        }

        public SlotItemSO GetItemSO()
        {
            foreach (SlotItem item in _matchedItems)
            {
                if (!item.ItemSO._isWild)
                {
                    return item.ItemSO;
                }
            }
            return _matchedItems.Length > 0 ? _matchedItems[0].ItemSO : null;
        }
    }

    public const int MIN_MATCHED_COUNT_TO_WIN = 3;

    private const int DELAY_AFTER_CHECK_SCHEMES = 100;
    private const int DELAY_BEFORECHECK_SCHEMES = 300;

    public event Action<CheckingSchemeResult> OnWinLine;
    public event Action OnSpinExecuted;
    public event Action OnSpinFinished;

    [Header("Debug")]
    [SerializeField] private bool _showMatchedItemsEnabled;

    [Header("Spin")]
    [SerializeField] private int _minSpinItemsCount = 7;
    [SerializeField] private int _maxSpinItemsCount = 30;
    [SerializeField] private AnimationCurve _animationCurve;

    [Header("Links")]
    [SerializeField] private SlotColumn[] _slotColumns;
    [SerializeField] private SlotItemSO[] _initialSlotItemsSO;
    [SerializeField] private CheckingSchemeSO[] _checkingSchemes;

    public int LinesCount => _checkingSchemes.Length;

    public bool IsReady { get; private set; }

    public IEnumerable<SlotItemSO> ItemsSO => _slotItemsSO;

    private List<SlotItemSO> _slotItemsSO;


    private void Awake()
    {
        IsReady = true;
        _slotItemsSO = new List<SlotItemSO>(_initialSlotItemsSO);
    }

    public async Task CheckAllSchemes()
    {
        List<CheckingSchemeResult> results = new();

        foreach (CheckingSchemeSO scheme in _checkingSchemes)
        {
            CheckingSchemeResult resultScheme = CheckScheme(scheme);
            if (resultScheme.MatchedItemsCount >= MIN_MATCHED_COUNT_TO_WIN)
            {
                results.Add(resultScheme);
            }
        }

        if (_showMatchedItemsEnabled)
        {
            await ShowAllMatchedLines(results);
        }
        else
        {
            CallMatchedLinesEvents(results);
        }
    }

    public async Task Spin()
    {
        if (IsReady)
        {
            OnSpinExecuted?.Invoke();
            IsReady = false;
            List<Task> tasks = new List<Task>();

            float delay = 0f;
            int spinItemsCount = _minSpinItemsCount;
            foreach (SlotColumn column in _slotColumns)
            {
                spinItemsCount = UnityEngine.Random.Range(spinItemsCount, _maxSpinItemsCount);
                delay += UnityEngine.Random.Range(0.03f, 0.1f);
                tasks.Add(column.Spin(spinItemsCount, delay));
            }

            await Task.WhenAll(tasks);
            await Task.Delay(DELAY_BEFORECHECK_SCHEMES);
            await CheckAllSchemes();
            IsReady = true;
            OnSpinFinished?.Invoke();
        }
    }

    public float GetProgressValue(float time)
    {
        return _animationCurve.Evaluate(time);
    }

    public void AddSlotItemSO(SlotItemSO slotItemSO)
    {
        _slotItemsSO.Add(slotItemSO);
        foreach (SlotColumn column in _slotColumns)
        {
            column.AddSlotItemSO(slotItemSO);
        }
    }

    public void AddSlotItemSOToColumnAt(int cloumnIndex, SlotItemSO slotItemSO)
    {
        _slotItemsSO.Add(slotItemSO);
        SlotColumn column = _slotColumns[cloumnIndex];        
        column.AddSlotItemSO(slotItemSO);
    }

    private CheckingSchemeResult CheckScheme(CheckingSchemeSO checkingScheme)
    {
        List<SlotItem> maxMatchedGroup = new List<SlotItem>();
        List<SlotItem> currentGroup = new List<SlotItem>();
        SlotItem groupBaseItem = null;
        int currentMultiplier = 1;
        int maxMatchedGroupMultiplier = 1;

        for (int columnIndex = 0; columnIndex < _slotColumns.Length; columnIndex++)
        {
            SlotColumn column = _slotColumns[columnIndex];
            int hitRow = checkingScheme._hitRows[columnIndex];
            SlotItem item = column.GetItemAt(hitRow);

            if (groupBaseItem == null)
            {
                currentGroup.Add(item);
                if (item.ItemSO._isWild)
                {
                    currentMultiplier *= item.ItemSO._betMultiplier[0];
                }
                else
                {
                    groupBaseItem = item;
                }
            }
            else if (item.SameItems(groupBaseItem))
            {
                currentGroup.Add(item);
                if (item.ItemSO._isWild)
                {
                    currentMultiplier *= item.ItemSO._betMultiplier[0];
                }
            }
            else
            {
                if (currentGroup.Count > maxMatchedGroup.Count)
                {
                    maxMatchedGroup.Clear();
                    maxMatchedGroup.AddRange(currentGroup);
                    maxMatchedGroupMultiplier = currentMultiplier;
                }

                List<SlotItem> wildItems = GetWildItemsRow(currentGroup);
                currentGroup.Clear();
                currentGroup.AddRange(wildItems);
                currentGroup.Add(item);
                groupBaseItem = item.ItemSO._isWild ? null : item;
                currentMultiplier = item.ItemSO._isWild ? item.ItemSO._betMultiplier[0] : 1;
            }
        }

        if (maxMatchedGroup.Count > currentGroup.Count)
        {
            return new CheckingSchemeResult(maxMatchedGroup, checkingScheme.name, maxMatchedGroupMultiplier);
        }
        else
        {
            return new CheckingSchemeResult(currentGroup, checkingScheme.name, currentMultiplier);
        }
    }

    private List<SlotItem> GetWildItemsRow(List<SlotItem> items)
    {
        List<SlotItem> wildItems = new List<SlotItem>();
        for (int i = items.Count - 1; i >= 0; i--)
        {
            SlotItem currentItem = items[i];
            if (currentItem.ItemSO._isWild)
            {
                wildItems.Add(items[i]);
            }
            else
            {
                break;
            }
        }
        return wildItems;
    }

    private async Task ShowAllMatchedLines(IEnumerable<CheckingSchemeResult> results)
    {
        foreach (CheckingSchemeResult result in results)
        {
            OnWinLine?.Invoke(result);
            await ShowMatchedItems(result.MatchedItems);
            await Task.Delay(DELAY_AFTER_CHECK_SCHEMES);
        }
    }

    private void CallMatchedLinesEvents(IEnumerable<CheckingSchemeResult> results)
    {
        foreach (CheckingSchemeResult result in results)
        {
            OnWinLine?.Invoke(result);
        }
    }

    private async Task ShowMatchedItems(IEnumerable<SlotItem> matchedItems)
    {
        List<Task> tasks = new List<Task>();
        foreach (SlotItem item in matchedItems)
        {
            tasks.Add(item.Blink());
        }
        await Task.WhenAll(tasks);
    }
}
