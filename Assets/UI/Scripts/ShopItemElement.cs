using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomSlotsElements
{
    public class ShopItemElement : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<ShopItemElement> { }

        private const string STYLE_SHEET_NAME = "ShopStyles";
        private const string MAIN_STYLE_NAME = "shop-item";
        private const string IMAGE_STYLE_NAME = "shop-item-image";
        private const string RESTRICTED_LABEL_STYLE_NAME = "shop-item-restricted-label";
        private const string PRICE_LABEL_STYLE_NAME = "shop-item-price-label";
        private const string COINS_ICON_STYLE_NAME = "coins-icon";

        private const string SOLD_STYLE_NAME = "sold";
        private const string RESTRICTED_STYLE_NAME = "restricted";

        private const string PRICE_PANEL_STYLE_NAME = "price-panel";

        private const string LEVEL_LABEL_NAME = "level-label";
        private const string PRICE_LABEL_NAME = "price-label";
        private const string ICON_NAME = "icon-image";

        private const string SOLD_TEXT = "SOLD";
        private const string LVL_TEXT = " Lvl";


        public event Action<ShopItemElement> OnClick;

        public event Func<int> GetCurrentLevel;
        public event Func<int> GetMoneyAmount;
        public event Func<ItemSO, int> GetCount;
        public event Func<ItemSO, int> GetPrice;
        public event Func<ItemSO, int> GetRequiredLevel;


        public ItemSO ItemSO { get; private set; }
        public int Price => _price;
        public int RequiredLevel => _requiredLevel;

        private int _price;
        private int _requiredLevel;


        VisualElement _image;
        Label _restrictedLabel;
        Label _priceLabel;

        public ShopItemElement(): base()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(STYLE_SHEET_NAME);
            styleSheets.Add(styleSheet);
            AddToClassList(MAIN_STYLE_NAME);

            _image = AddElement(this, IMAGE_STYLE_NAME, ICON_NAME);
            _image.RegisterCallback<ClickEvent>(OnClickPerformed);
            
            _restrictedLabel = AddLabel(_image, RESTRICTED_LABEL_STYLE_NAME, LEVEL_LABEL_NAME);
            _restrictedLabel.AddToClassList(RESTRICTED_STYLE_NAME);

            VisualElement pricePanel = AddElement(this, PRICE_PANEL_STYLE_NAME);
            
            AddElement(pricePanel, COINS_ICON_STYLE_NAME);
            _priceLabel = AddLabel(pricePanel, PRICE_LABEL_STYLE_NAME, PRICE_LABEL_NAME);
        }

        ~ShopItemElement()
        {
            _image.UnregisterCallback<ClickEvent>(OnClickPerformed);
        }

        public void Init(ItemInfo itemInfo)
        { 
            ItemSO = itemInfo.ItemSO;
            _price = itemInfo.Price;
            _requiredLevel = itemInfo.Level;
            _image.style.backgroundImage = ItemSO != null ? new StyleBackground(ItemSO._sprite) : null;

            _priceLabel.text = $"{_price}";
            _restrictedLabel.text = $"{_requiredLevel} Lvl";
            Refresh();
        }

        public void Refresh()
        {
            _price = GetPrice != null ? GetPrice(ItemSO) : 0;
            _requiredLevel = GetRequiredLevel != null ? GetRequiredLevel(ItemSO) : 0;
            _priceLabel.text = $"{_price}";
            _restrictedLabel.text = GetRestrictedText();

            bool checkLevel = CheckLevel();
            bool checkCount = CheckCount();
            _restrictedLabel.style.visibility = checkLevel && checkCount ? Visibility.Hidden : Visibility.Visible;

            bool checkMoney = CheckMoney();
            bool classContainsSold = _restrictedLabel.ClassListContains(SOLD_STYLE_NAME);
            bool classContainsRestrictedInRestrictedLabel = _restrictedLabel.ClassListContains(RESTRICTED_STYLE_NAME);
            bool classContainsRestrictedInPriceLabel = _priceLabel.ClassListContains(RESTRICTED_STYLE_NAME);

            if (checkCount == classContainsSold)
            {
                _restrictedLabel.ToggleInClassList(SOLD_STYLE_NAME);
            }

            if (checkCount != classContainsRestrictedInRestrictedLabel)
            {
                _restrictedLabel.ToggleInClassList(RESTRICTED_STYLE_NAME);
            }

            if ((checkMoney || !checkCount) == classContainsRestrictedInPriceLabel)
            {
                _priceLabel.ToggleInClassList(RESTRICTED_STYLE_NAME);
            }
        }

        private bool CheckLevel()
        {
            return GetCurrentLevel != null && GetCurrentLevel() >= _requiredLevel;
        }

        private bool CheckMoney()
        {
            return GetMoneyAmount != null && GetMoneyAmount() >= _price;
        }

        private bool CheckCount()
        {
            return GetCount != null && GetCount(ItemSO) > 0;
        }

        private VisualElement AddElement(VisualElement parent, string styleName, string name = "")
        {
            return InitializeElement(new VisualElement(), parent, styleName, name);
        }

        private Label AddLabel(VisualElement parent, string styleName, string name = "") 
        {
            return InitializeElement(new Label(), parent, styleName, name);
        }

        private string GetRestrictedText()
        {
            return CheckCount() ? _requiredLevel.ToString() + LVL_TEXT : SOLD_TEXT;
        }

        private void OnClickPerformed(ClickEvent evt)
        {
            OnClick?.Invoke(this);
        }

        private T InitializeElement<T>(T element, VisualElement parent, string styleName, string name) where T : VisualElement 
        {
            element.name = name;
            parent.Add(element);
            element.AddToClassList(styleName);
            return element;
        }
    }
}
