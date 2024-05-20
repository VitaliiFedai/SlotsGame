using System;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ItemInfo[] _itemsInfo;
    [SerializeField] private Inventory _inventory;

    public event Action<ItemSO, int> OnChangedPrice
    {
        add { _inventory.OnChangedPrice += value; }
        remove { _inventory.OnChangedPrice -= value; }
    }

    public event Action<ItemSO, int> OnChangedRequiredLevel
    {
        add { _inventory.OnChangedRequiredLevel += value; }
        remove { _inventory.OnChangedRequiredLevel -= value; }
    }

    private Player _player;
    private bool _sessionStarted;

    private bool SessionStarted => _player != null;

    private void Awake()
    {
        _inventory.AddItems(_itemsInfo);
    }

    public void StartSession(Player player)
    {
        _player = player;    
        if (_player == null)
        {
            throw new ArgumentNullException(nameof(_player));            
        }
    }

    public void StopSession() 
    {
        _player = null;
    }

    public IEnumerable<ItemSO> Items => _inventory.ItemsSO;

    public IEnumerable<ItemInfo> ItemsInfo => _inventory.ItemsInfo;

    public void BuyItem(ItemSO itemSO)
    {
        if (!_inventory.HasItem(itemSO))
        {
            throw new Exception($"Can''t {nameof(BuyItem)}! {itemSO} not found in {nameof(_inventory)}");
        }

        if (_inventory.GetCount(itemSO) == 0)
        {
            throw new Exception($"Can''t {nameof(BuyItem)}! {itemSO} count = 0");
        }

        if (_player.Experience.Level < _inventory.GetRequiredLevel(itemSO))
        {
            throw new Exception($"Can''t {nameof(BuyItem)}! {itemSO} level = {_inventory.GetRequiredLevel(itemSO)} Player Level = {_player.Experience.Level}");
        }

        if (_player.Wallet.TryToGet(_inventory.GetPrice(itemSO)))
        {
            _inventory.GiveItemTo(itemSO, _player.Inventory);
        }
        else
        {
            throw new Exception($"Can''t {nameof(BuyItem)} {itemSO}! Not enough coins! Player Wallet: {_player.Wallet.Amount}  Price: {_inventory.GetPrice(itemSO)}");
        }
    }

    public bool CanBuy(ItemSO itemSO)
    {
        return _inventory.HasItem(itemSO) && _inventory.GetCount(itemSO) > 0 && _player.Experience.Level >= _inventory.GetRequiredLevel(itemSO);
    }

    public int GetPrice(ItemSO itemSO)
    {
        return _inventory.GetPrice(itemSO);
    }

    public void SetPrice(ItemSO itemSO, int price)
    {
        _inventory.SetPrice(itemSO, price);
    }

    public int GetRequiredLevel(ItemSO itemSO)
    {
        return _inventory.GetRequiredLevel(itemSO);
    }

    public void SetRequiredLevel(ItemSO itemSO, int price)
    {
        _inventory.SetRequiredLevel(itemSO, price);
    }
}
