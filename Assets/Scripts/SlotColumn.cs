using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlotColumn : MonoBehaviour
{
    [Serializable]
    public struct SaveData
    {
        public SlotColumnItemsSOHolder.SaveData ItemsSOHolderSaveData;

        public SaveData(SlotColumn slotColumn)
        {
            ItemsSOHolderSaveData = new SlotColumnItemsSOHolder.SaveData(slotColumn._itemsSOHolder);
        }

        public void SetSavedDataTo(SlotColumn slotColumn)
        {
            ItemsSOHolderSaveData.SetSavedDataTo(slotColumn._itemsSOHolder);
        }
    }


    private const int ROWS_COUNT = 3;

    [SerializeField] private SlotBox _slotBox;

    [SerializeField] private SlotItemSO[] _uniqueItemsSO;

    private SlotItem[] _items;
    private float _itemHeight;

    private Vector3 _startPosition;

    private float _spinMaxDistance;
    private float _spinDuration;
    private float _spinTime;

    private float _spinDistance;
    private SlotColumnItemsSOHolder _itemsSOHolder;

    private void Awake()
    {
        SlotItem[] items = GetComponentsInChildren<SlotItem>();
        _items = new SlotItem[items.Length];
        items.CopyTo(_items, 0);

        List<SlotItemSO> slotItemSOs = new List<SlotItemSO>(_slotBox.ItemsSO);
        slotItemSOs.AddRange(_uniqueItemsSO);

        _itemsSOHolder = new SlotColumnItemsSOHolder(slotItemSOs, ROWS_COUNT);
    }

    private void Start()
    {
        _startPosition = transform.localPosition;
        _startPosition = transform.position;

        for (int index = 0; index < _items.Length; index++)
        {
            SetItemProperties(_items[index]);
        }

        if (_items.Length > 1)
        {
            _itemHeight = _items[1].transform.position.y - _items[0].transform.position.y;
        }
    }

    public async Task Spin(int itemsToSpin, float delaySeconds = 0f)
    {
        await Task.Delay((int)(delaySeconds * 1000));

        if (_spinDuration == 0f)
        {
            _spinDuration = 1.5f + 0.1f * itemsToSpin;
            _spinMaxDistance = itemsToSpin * _itemHeight;
        }
        await SpinRefresh();
    }

    public SlotItem GetItemAt(int index)
    {
        return _items[index];
    }

    public void AddSlotItemSO(SlotItemSO slotItemSO)
    {
        if (_itemsSOHolder.HasSlotItemSO(slotItemSO))
        {
            throw new InvalidOperationException($"{slotItemSO} is already in the list!");   
        }
        _itemsSOHolder.AddSlotItemSOToAwailable(slotItemSO);
    }

    private void SetItemProperties(SlotItem item)
    {
        SlotItemSO itemSO = _itemsSOHolder.GetRandomItem();
        item.SetItemSO(itemSO);
    }

    private async Task SpinRefresh()
    {
        while (_spinTime < _spinDuration)
        {
            _spinTime = _spinTime + Time.deltaTime;
            float newDistance = Math.Min(1.0f, _slotBox.GetProgressValue(_spinTime / _spinDuration)) * _spinMaxDistance;
            float deltaDistance = (newDistance - _spinDistance);
            _spinDistance = newDistance;

            float newPosition = transform.position.y - deltaDistance;

            if (Math.Abs(_spinMaxDistance - _spinDistance) <= 0.001f)
            {
                _spinDuration = 0f;
                _spinTime = 0f;
                _spinDistance = 0f;

                if ((_startPosition.y - transform.position.y) / _itemHeight > 0.5f)
                {
                    SwapColumnOnce();
                }
                newPosition = _startPosition.y;
            }
            else
            {
                int swapCount = AlignColumnToBase(newPosition);
                newPosition += _itemHeight * swapCount;
            }
            transform.position = new Vector3(_startPosition.x, newPosition, _startPosition.z);
            await Task.Yield();
        }
    }

    private void SwapColumnOnce()
    {
        for (int index = 0; index < _items.Length - 1; index++)
        {
            SwapItems(fromIndex: index + 1, toIndex: index);
        }
        SetItemProperties(GetTopItem());
    }

    private int AlignColumnToBase(float newPosition)
    {
        int swapCount = (int)((_startPosition.y - newPosition) / _itemHeight);
        for(int i = 0; i < swapCount; i++)
        {
            SwapColumnOnce();
        }
        return swapCount;
    }

    private void SwapItems(int fromIndex, int toIndex)
    {
        _items[toIndex].AssignFrom(_items[fromIndex]);
    }

    private SlotItem GetTopItem()
    {
        return _items[_items.Length - 1];
    }
}
