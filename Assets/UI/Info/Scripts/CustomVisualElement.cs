using UnityEngine;
using UnityEngine.UIElements;

namespace CustomSlotsElements
{
    public abstract class CustomVisualElement : VisualElement
    {
        public CustomVisualElement()
        {
            LoadStyleSheet(GetStyleSheetName());
            AddToClassList(GetMainStyleName());
        }

        ~CustomVisualElement()
        {
            OnDestroy();
        }

        protected abstract string GetStyleSheetName();
        protected abstract string GetMainStyleName();

        protected virtual void OnDestroy()
        { }

        protected VisualElement CreateElement(VisualElement parent, string styleName, string elementName = "")
        {
            return InitializeElement(new VisualElement(), parent, styleName, elementName);
        }

        protected Label CreateLabel(VisualElement parent, string styleName, string text, string elementName = "")
        {
            Label label = new Label();
            label.text = text;
            return InitializeElement(label, parent, styleName, elementName);
        }

        protected Button CreateButton(VisualElement parent, string elementName, string text, string styleName = "")
        {
            Button button = new Button();
            button.text = text;
            return InitializeElement(button, parent, styleName, elementName);
        }

        protected T InitializeElement<T>(T element, VisualElement parent, string styleName, string elementName) where T : VisualElement 
        {
            element.name = elementName;
            element.AddToClassList(styleName);
            parent.Add(element);
            return element;
        }

        private void LoadStyleSheet(string name)
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(name);
            styleSheets.Add(styleSheet);
        }
    }
}