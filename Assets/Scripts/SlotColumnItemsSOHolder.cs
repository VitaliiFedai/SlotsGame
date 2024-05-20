using System;
using System.Collections.Generic;
using System.Linq;

public class SlotColumnItemsSOHolder
{

    [Serializable]
    public struct SaveData
    {
        public SlotItemSO[] SlotItemsSO;
        public float SumWeight;
        public int RowsCount;

        public SaveData(SlotColumnItemsSOHolder itemsHolder)
        {
            List<SlotItemSO> slotItemSOs = new List<SlotItemSO>(itemsHolder._awailableItems);
            slotItemSOs.AddRange(itemsHolder._unawailableItems);

            SlotItemsSO = slotItemSOs.ToArray();

            SumWeight = slotItemSOs.Sum((itemSO) => itemSO._spawnWeight);
            RowsCount = itemsHolder._rowsCount;
        }

        public void SetSavedDataTo(SlotColumnItemsSOHolder itemsHolder)
        {
            itemsHolder._awailableItems.Clear();
            foreach (SlotItemSO itemSO in SlotItemsSO)
            {
                itemsHolder._awailableItems.AddLast(itemSO);
            }
            itemsHolder._sumWeight = SumWeight;
            itemsHolder._rowsCount = RowsCount;
        }
    }


    private LinkedList<SlotItemSO> _awailableItems;
    private Queue<SlotItemSO> _unawailableItems;
    private float _sumWeight;
    private int _rowsCount;

    public SlotColumnItemsSOHolder(IEnumerable<SlotItemSO> awailableItemsSO, int rowsCount)
    {
        _awailableItems = new LinkedList<SlotItemSO>(awailableItemsSO);
        _unawailableItems = new Queue<SlotItemSO>();
        _rowsCount = rowsCount;

        _sumWeight = _awailableItems.Sum(item => item._spawnWeight);
    }

    public SlotItemSO GetRandomItem()
    {
        float randomValue = UnityEngine.Random.Range(0, _sumWeight);

        foreach (SlotItemSO item in _awailableItems)
        {
            randomValue -= item._spawnWeight;
            if (randomValue <= 0)
            {
                PickItem(item);
                return item;
            }
        }
        return _awailableItems.Last();
    }

    public void AddSlotItemSOToAwailable(SlotItemSO item) 
    {
        _awailableItems.AddLast(item);
        _sumWeight += item._spawnWeight;
    }

    public bool HasSlotItemSO(SlotItemSO item) 
    {
        return _awailableItems.Contains(item) || _unawailableItems.Contains(item);
    }

    private void PickItem(SlotItemSO item)
    {
        _awailableItems.Remove(item);
        _unawailableItems.Enqueue(item);
        _sumWeight -= item._spawnWeight;
        if (_unawailableItems.Count == _rowsCount)
        {
            SlotItemSO itemToBringBack = _unawailableItems.Dequeue();
            AddSlotItemSOToAwailable(itemToBringBack);
        }
    }
}
