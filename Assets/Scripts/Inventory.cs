using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Inventory : MonoBehaviour 
{
    [Serializable]
    public struct SaveData
    {
        public ItemInfo[] ItemInfos;

        public SaveData(Inventory inventory)
        {
            ItemInfos = inventory._itemsInfo.ToArray();
        }

        public void SetSavedDataTo(Inventory inventory)
        {
            if (ItemInfos != null)
            {
                inventory._itemsInfo = ItemInfos.ToList();

                foreach (ItemInfo info in inventory._itemsInfo)
                {
                    Debug.Log(info);
                }
            }
        }
    }

    private List<ItemInfo> _itemsInfo = new List<ItemInfo>();

    public event Action<Inventory, ItemSO> OnItemLost;
    public event Action<Inventory, ItemSO> OnItemGet;

    public event Action<ItemSO, int> OnChangedPrice;
    public event Action<ItemSO, int> OnChangedRequiredLevel;

    public IEnumerable<ItemSO> ItemsSO => _itemsInfo.Select(itemInfo => itemInfo.ItemSO);
    public IEnumerable<ItemInfo> ItemsInfo => _itemsInfo;

    public void AddItems(IEnumerable<ItemInfo> itemsInfo)
    {
        foreach (ItemInfo itemInfo in itemsInfo)
        {
            AddItem(itemInfo.ItemSO, itemInfo.Level, itemInfo.Price, itemInfo.Count);
        }
    }

    public int GetCount(ItemSO itemSO)
    {
        int index = GetIndexOf(itemSO);
        return index >= 0 ? _itemsInfo[index].Count : 0;
    }

    public int GetRequiredLevel(ItemSO itemSO)
    {
        int index = GetIndexOf(itemSO);
        return index >= 0 ? _itemsInfo[index].Level : 0;
    }

    public int GetPrice(ItemSO itemSO)
    {
        int index = GetIndexOf(itemSO);
        return index >= 0 ? _itemsInfo[index].Price : 0;
    }

    public void SetPrice(ItemSO itemSO, int price)
    {
        int index = GetIndexOf(itemSO);
        if (index >= 0)
        {
            ItemInfo itemInfo = _itemsInfo[index];
            itemInfo.SetPrice(price);
            _itemsInfo[index] = itemInfo;
            OnChangedPrice?.Invoke(itemSO, price);
        }
        else
        {
            throw new Exception($"Can''t {nameof(SetPrice)}. Item {itemSO} not found!");
        }
    }

    public void SetRequiredLevel(ItemSO itemSO, int level)
    {
        int index = GetIndexOf(itemSO);
        if (index >= 0)
        {
            ItemInfo itemInfo = _itemsInfo[index];
            itemInfo.SetLevel(level);
            _itemsInfo[index] = itemInfo;
            OnChangedRequiredLevel?.Invoke(itemSO, level);
        }
        else
        {
            throw new Exception($"Can''t {nameof(SetRequiredLevel)}. Item {itemSO} not found!");
        }
    }

    public void IncCount(ItemSO itemSO)
    {
        int index = GetIndexOf(itemSO);
        if (index >= 0)
        {
            ItemInfo itemInfo = _itemsInfo[index];
            itemInfo.IncCount();
            _itemsInfo[index] = itemInfo;
            OnItemGet?.Invoke(this, itemSO);
        }
        else
        {
            throw new Exception($"Can''t {nameof(IncCount)}. Item {itemSO} not found!");
        }
    }

    public void DecCount(ItemSO itemSO)
    {
        int index = GetIndexOf(itemSO);
        if (index >= 0)
        {
            if (_itemsInfo[index].Count > 0)
            {
                ItemInfo itemInfo = _itemsInfo[index];
                itemInfo.DecCount();
                _itemsInfo[index] = itemInfo;
                OnItemLost?.Invoke(this, itemSO);
            }
            else
            {
                throw new Exception($"Can''t {nameof(DecCount)}. Count = 0!");
            }
        }
        else
        {
            throw new Exception($"Can''t {nameof(DecCount)}. Item {itemSO} not found!");
        }
    }

    private int GetIndexOf(ItemSO itemSO)
    { 
        for (int i = 0; i < _itemsInfo.Count; i++) 
        {
            if (_itemsInfo[i].ItemSO == itemSO)
            {
                return i;
            }
        }
        return -1;
    }

    //public void SetExpirienceHandler(ExperienceHandler experienceHandler)
    //{
    //    if (experienceHandler == null)
    //    {
    //        throw new ArgumentNullException(nameof(experienceHandler));
    //    }
    //    _experienceHandler = experienceHandler;
    //}

    //public void SetWallet(Wallet wallet)
    //{
    //    if (wallet == null)
    //    {
    //        throw new ArgumentNullException(nameof(wallet));
    //    }
    //    _wallet = wallet;
    //}

    public bool HasItem(ItemSO itemSO)
    {
        return GetIndexOf(itemSO) >= 0;
    }

    public void GiveItemTo(ItemSO itemSO, Inventory destination)
    {
        DecCount(itemSO);
        if (destination.HasItem(itemSO))
        {
            destination.IncCount(itemSO);
        }
        else
        { 
            destination.AddItem(itemSO, level: GetRequiredLevel(itemSO), price: GetPrice(itemSO), 1);
        }
    }

    private void AddItem(ItemSO itemSO, int level, int price, int count)
    {
        _itemsInfo.Add(new ItemInfo(itemSO, level, price, count));
        OnItemGet?.Invoke(this, itemSO);
    }
}
