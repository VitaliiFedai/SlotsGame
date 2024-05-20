using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUIElements
{

    public class CustomSlider : Slider
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<CustomSlider> { }

        private const string STYLE_RESOURCE = "CustomSlider";
        private const string CUSTOM_SLIDER = "custom-slider";

        private const string CONTAINER_NAME = "unity-drag-container";
        private const string TRACKER_NAME = "unity-tracker";
        private const string DRAGGER_NAME = "unity-dragger";

        private const string CONTAINER_STYLE = "custom-drag-container";
        private const string TRACKER_STYLE = "custom-tracker";
        private const string DRAGGER_STYLE = "custom-dragger";

        private const string BAR_NAME = "bar";
        private const string BAR_STYLE = "bar";

        private const string NEW_DRAGGER_NAME = "custom-dragger";
        private const string NEW_DRAGGER_STYLE = "new-dragger";

        private VisualElement _dragger;
        private VisualElement _newDragger;

        public CustomSlider()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(STYLE_RESOURCE);
            styleSheets.Add(styleSheet);
            AddToClassList(CUSTOM_SLIDER);

            AddStyleToElement(CONTAINER_NAME, CONTAINER_STYLE);
            AddStyleToElement(TRACKER_NAME, TRACKER_STYLE);
            AddStyleToElement(DRAGGER_NAME, DRAGGER_STYLE);

            _dragger = this.Q<VisualElement>(DRAGGER_NAME);

            AddElements();

            RegisterCallback<ChangeEvent<float>>(OnSliderValueChanged);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        ~CustomSlider()
        {
            UnregisterCallback<ChangeEvent<float>>(OnSliderValueChanged);
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void AddStyleToElement(string elementName, string styleName)
        {
            VisualElement element = this.Q<VisualElement>(elementName);
            element.AddToClassList(styleName);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            RefreshNewDraggerPosition();
        }

        private void OnSliderValueChanged(ChangeEvent<float> evt)
        {
            RefreshNewDraggerPosition();
        }

        private void AddElements()
        {
            VisualElement bar = new VisualElement();
            bar.name = BAR_NAME;
            bar.AddToClassList(BAR_STYLE);

            _dragger = this.Q<VisualElement>(DRAGGER_NAME);
            _dragger.Add(bar);

            _newDragger = new VisualElement();
            _newDragger.name = NEW_DRAGGER_NAME;
            _newDragger.AddToClassList(NEW_DRAGGER_STYLE);
            _newDragger.pickingMode = PickingMode.Ignore;

            Add(_newDragger);
        }

        private void RefreshNewDraggerPosition()
        {
            Vector2 offset = _dragger.layout.center - _newDragger.layout.center;
            Vector2 pos = _dragger.LocalToWorld(Vector2.zero) + offset;
            _newDragger.transform.position = _newDragger.parent.WorldToLocal(pos);
        }
    }
}