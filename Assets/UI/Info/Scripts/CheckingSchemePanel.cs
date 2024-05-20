using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomSlotsElements
{
    [Serializable]
    public class CheckingSchemePanel : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<CheckingSchemePanel> { }

        private const int ROWS_COUNT = 3;
        private const int COLUMNS_COUNT = 5;
        private const string STYLE_SHEET_NAME = "CheckingSchemeStyle";
        private const string MAIN_STYLE_NAME = "checking-scheme";
        private const string CONTAINER_NAME = "checking-scheme-container";
        private const string CONTAINER_STYLE_NAME = "checking-scheme-container";
        private const string ROW_STYLE_NAME = "checking-scheme-row";
        private const string ITEM_STYLE_NAME = "checking-scheme-item";
        private const string ITEM_NAME = "item";
        private const string LABEL_STYLE_NAME = "checking-scheme-index-label";



        private VisualElement[,] _items = new VisualElement[ROWS_COUNT, COLUMNS_COUNT];
        private CheckingSchemeSO _scheme;

        private Color _defaultColor = new Color(0.38f, 0.22f, 0.33f);
//        private Color _defaultColor = new Color(0.52f, 0.38f, 0.35f);
//        private Color _matchColor = new Color(0.96f, 0.55f, 0.20f);
        private Color _matchColor = new Color(1f, 0.69f, 0.34f);
        private int _index;
        private Label _label;

        public CheckingSchemePanel()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(STYLE_SHEET_NAME);
            styleSheets.Add(styleSheet);
            AddToClassList(MAIN_STYLE_NAME);
            pickingMode = PickingMode.Ignore;

            _label = AddLabel();

            VisualElement container = new VisualElement();
            container.name = CONTAINER_NAME;
            container.AddToClassList(CONTAINER_STYLE_NAME);
            hierarchy.Add(container);

            for (int i = 0; i < ROWS_COUNT; i++) 
            {
                VisualElement row = AddElement(container, Length.Percent(100f), Length.Auto());
                row.AddToClassList(ROW_STYLE_NAME);

                for (int j = 0; j < COLUMNS_COUNT; j++) 
                {
                    VisualElement item = AddElement(row, Length.Percent(20f), Length.Auto());
                    item.name = ITEM_NAME;
                    item.AddToClassList(ITEM_STYLE_NAME);
                    _items[i, j] = item;
                }
            }

            RefreshItemColors();
        }

        public void SetScheme(CheckingSchemeSO scheme)
        { 
            _scheme = scheme;
            RefreshItemColors();
        }

        public void SetItemColors(Color defaultColor, Color matchColor)
        {
            _defaultColor = defaultColor;
            _matchColor = matchColor;
            RefreshItemColors();
        }

        public void SetIndex(int index)
        {
            _index = index;
            _label.text = index.ToString();
        }

        private void RefreshItemColors()
        {
            if (_scheme != null)
            {
                for (int i = 0; i < ROWS_COUNT; i++)
                {    for (int j = 0; j < COLUMNS_COUNT; j++)
                    {
                        _items[i, j].style.backgroundColor = (i == GetInvertedRow(_scheme._hitRows[j])) ? _matchColor : _defaultColor;
                    }
                }
            }
        }

        private int GetInvertedRow(int row)
        {
            return ROWS_COUNT - row - 1;
        }

        private VisualElement AddElement(VisualElement parent, StyleLength width, StyleLength height)
        {
            VisualElement item = new VisualElement();
            item.style.width = width;
            item.style.height = height;
            parent.Add(item);
            return item;
        }

        private Label AddLabel()
        {
            Label label = new Label();
            label.AddToClassList(LABEL_STYLE_NAME);
            label.text = "0";
            hierarchy.Add(label);
            return label;
        }
    }
}