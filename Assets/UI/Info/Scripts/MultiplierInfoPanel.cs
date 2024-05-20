using UnityEngine.UIElements;
using UnityEngine;

namespace CustomSlotsElements
{
    public class MultiplierInfoPanel : CustomVisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<MultiplierInfoPanel> { }

        public const string CONTAINER_NAME = "multiplier-info-container";

        private const string STYLE_SHEET_NAME = "MultiplierInfoPanelStyle";
        private const int BET_MULTIPLIERS_COUNT = 3;
        private const int MIN_MATCHED_COUNT_TO_WIN = 3;
        private const string MAIN_STYLE_NAME = "multiplier-info";
        private const string CONTAINER_STYLE_NAME = "multiplier-info-container";
        private const string IMAGE_NAME = "multiplier-info-item-icon";
        private const string IMAGE_STYLE_NAME = "multiplier-info-item-icon";
        private const string TEXT_PANEL_STYLE_NAME = "multiplier-info-text-panel";
        private const string TEXT_PANEL_LEFT_STYLE_NAME = "multiplier-info-text-panel-left";
        private const string TEXT_PANEL_RIGHT_STYLE_NAME = "multiplier-info-text-panel-right";

        private VisualElement _itemIcon;
        private SlotItemSO _slotItemSO;

        private Label[] _matchCountLabels = new Label[BET_MULTIPLIERS_COUNT];
        private Label[] _multiplierValueLabels = new Label[BET_MULTIPLIERS_COUNT];

        public MultiplierInfoPanel() : base() 
        {
            VisualElement container = CreateElement(this, CONTAINER_STYLE_NAME, CONTAINER_NAME);
            container.pickingMode = PickingMode.Ignore;
            _itemIcon = CreateElement(container, IMAGE_STYLE_NAME, IMAGE_NAME);
            VisualElement textPanel = CreateElement(container, TEXT_PANEL_STYLE_NAME);
            VisualElement textPanelLeft = CreateElement(textPanel, TEXT_PANEL_LEFT_STYLE_NAME);
            VisualElement textPanelRight = CreateElement(textPanel, TEXT_PANEL_RIGHT_STYLE_NAME);

            for (int i = BET_MULTIPLIERS_COUNT - 1; i >= 0; i--)
            {
                string text = $"{i + MIN_MATCHED_COUNT_TO_WIN}x";
                _matchCountLabels[i] = CreateLabel(textPanelLeft, "", text);
                _multiplierValueLabels[i] = CreateLabel(textPanelRight, "", "0");
            }
        }

        public void SetSlotItemSO(SlotItemSO itemSO)
        {
            _slotItemSO = itemSO;
            _itemIcon.style.backgroundImage = new StyleBackground(_slotItemSO._sprite);
            _itemIcon.style.unityBackgroundImageTintColor = _slotItemSO._color;

            for (int i = 0; i < BET_MULTIPLIERS_COUNT; i++)
            {
                _multiplierValueLabels[i].text = _slotItemSO._betMultiplier[i].ToString();
            }
        }

        protected override string GetMainStyleName()
        {
            return MAIN_STYLE_NAME;
        }

        protected override string GetStyleSheetName()
        {
            return STYLE_SHEET_NAME;
        }
    }
}