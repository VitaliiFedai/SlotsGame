using CustomSlotsElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class ShopWindow : PanelUI
{
    private const string BACK_BUTTON_NAME = "back-button";
    private const string CONTAINER_NAME = "container";

    public event Action OnBackButtonClicked;
    public event Action<ShopItemElement> OnElementClick;

    private List<ShopItemElement> _shopItemElements = new List<ShopItemElement>();

    private IEnumerable<ItemInfo> _items;
    private VisualElement _container;

    private int _currentLevel;
    private int _money;


    public void SetCurrentLevel(int currentLevel)
    { 
        _currentLevel = currentLevel;
        Refresh();
    }

    public void SetMoneyAmount(int amount)
    {
        _money = amount;
        Refresh();
    }

    public void SetItems(IEnumerable<ItemInfo> items)
    {
        _items = items;
        Refresh();
    }

    public void RefreshElement(ItemSO itemSO)
    {
        ShopItemElement element = GetShopItemElement(itemSO);
        element?.Refresh();
    }

    public void Refresh()
    {
        if (GetRoot() == null)
        {  
            return; 
        }

        if (_items.Count() == _shopItemElements.Count)
        {
            foreach (ItemInfo itemInfo in _items)
            {
                ShopItemElement element = GetShopItemElement(itemInfo.ItemSO);
                element?.Refresh();
            }
        }
        else
        {
            ClearElements();
            CreateElements();
        }
    }

    protected override void OnBind()
    {
        RegisterOnClickCallback(BACK_BUTTON_NAME, OnBackButtonClicked);

        _container = GetElement(CONTAINER_NAME);
        CreateElements();
    }

    protected override void OnUnbind()
    {
        ClearElements();
        _container = null;
    }

    private void ClearElements()
    {
        if (GetRoot() != null)
        {
            foreach (ShopItemElement element in _shopItemElements)
            {
                element.OnClick -= OnElementClick;
            }
            _container.Clear();
            _shopItemElements.Clear();
        }
    }

    private void CreateElements()
    {
        if (GetRoot() != null)
        {
            foreach (ItemInfo itemInfo in _items)
            {
                ShopItemElement itemElement = CreateShopItemElement(itemInfo);
                itemElement.OnClick += OnElementClick;
                _shopItemElements.Add(itemElement);
            }
        }
    }

    private ShopItemElement CreateShopItemElement(ItemInfo itemInfo)
    {
        ShopItemElement shopItemElement = new ShopItemElement();
        shopItemElement.Init(itemInfo);
        shopItemElement.GetCount += GetCount;
        shopItemElement.GetCurrentLevel += GetCurrentLevel;
        shopItemElement.GetMoneyAmount += GetMoneyAmount;
        shopItemElement.GetPrice += GetPrice;
        shopItemElement.GetRequiredLevel += GetRequiredLevel;
        shopItemElement.Refresh();
        _container.Add(shopItemElement);
        return shopItemElement;
    }


    private int GetCurrentLevel()
    { 
        return _currentLevel;
    }

    private int GetMoneyAmount()
    {
        return _money;
    }

    private int GetCount(ItemSO itemSO)
    {
        return GetItemInfo(itemSO).Count;
    }

    private int GetRequiredLevel(ItemSO itemSO)
    {
        return GetItemInfo(itemSO).Level;
    }

    private int GetPrice(ItemSO itemSO)
    {
        return GetItemInfo(itemSO).Price;
    }

    private ItemInfo GetItemInfo(ItemSO itemSO)
    {
        return _items.FirstOrDefault((itemInfo) => itemInfo.ItemSO == itemSO);
    }

    private ShopItemElement GetShopItemElement(ItemSO itemSO)
    {
        return _shopItemElements.FirstOrDefault((element) => element.ItemSO == itemSO);
    }
}
