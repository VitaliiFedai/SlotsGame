using System;
using UnityEngine;

[Serializable]
public struct ItemInfo
{
    [SerializeField] private ItemSO _itemSO;
    [SerializeField] private int _price;
    [SerializeField] private int _count;
    [SerializeField] private int _level;

    public ItemSO ItemSO => _itemSO;
    public int Price => _price;
    public int Level => _level;
    public int Count => _count;

    public ItemInfo(ItemSO itemSO, int level, int price, int count)
    {
        _itemSO = itemSO;
        _price = price;
        _level = level;
        _count = count;
    }

    public void SetPrice(int price)
    { 
        _price = price;
    }

    public void SetLevel(int level)
    { 
        _level = level;
    }

    public void IncCount()
    {
        _count++;
    }

    public void DecCount()
    {
        _count--;
    }
}
