using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomSlotsElements
{
    public class TabIndicator : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<TabIndicator> { }

        private const string STYLE_SHEET_NAME = "CheckingSchemeStyle";

        private const string MAIN_STYLE_NAME = "tab-indicator";
        private const string CONTAINER_STYLE_NAME = "tab-indicator-container";
        private const string CONTAINER_NAME = "tab-indicator-container";
        private const string TAB_POINT_STYLE_NAME = "tab-point";
        private const string INDICATOR_POINT_STYLE_NAME = "indicator-point";
        private const string INDICATOR_POINT_NAME = "indicator-point";


        private const string POINT_NAME = "point";

        public int CurrentTab
        {
            get => _tabIndex;
            set 
            {
                if (value > TabCount)
                { 
                    throw new ArgumentOutOfRangeException(nameof(CurrentTab));
                }
                _tabIndex = value;
                RefreshIndicatorPosition();
            }
        }

        public int TabCount => _tabPoints.Count;

        private int _tabIndex;

        private List<VisualElement> _tabPoints = new();
        private VisualElement _container;
        private VisualElement _indicatorPoint;

        public TabIndicator()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(STYLE_SHEET_NAME);
            styleSheets.Add(styleSheet);
            AddToClassList(MAIN_STYLE_NAME);
            pickingMode = PickingMode.Ignore;

            _container = new VisualElement();
            hierarchy.Add(_container);

            _container.name = CONTAINER_NAME;
            _container.AddToClassList(CONTAINER_STYLE_NAME);

            SetTabCount(5);

            AddIndicatorPoint();

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            CurrentTab = 3;
        }

        ~TabIndicator()
        { 
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void SetTabCount(int count)
        { 
            int delta = count - TabCount;
            if (delta > 0)
            {
                CallOperation(delta, AddTabPoint);
            }
            else if (delta < 0)
            {
                CallOperation(-delta, RemoveTabPoint);
            }

            if (_tabIndex >= TabCount)
            {
                _tabIndex = TabCount - 1;
            }
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        { 
            RefreshIndicatorPosition();
        }


        private void CallOperation(int callCount, Action operation)
        {
            for (int i = 0; i < callCount; i++)
            {
                operation();
            }
        }

        private void AddTabPoint()
        { 
            VisualElement point = new VisualElement();
            point.AddToClassList(TAB_POINT_STYLE_NAME);

            point.name = POINT_NAME;
            point.pickingMode = PickingMode.Ignore;
            _container.Add(point);
            _tabPoints.Add(point);
        }

        private void RemoveTabPoint()
        {
            VisualElement point = _tabPoints[_tabPoints.Count - 1];
            _container.Remove(point);
            _tabPoints.Remove(point);
        }

        private void AddIndicatorPoint()
        {
            _indicatorPoint = new VisualElement();
            _container.Add(_indicatorPoint);
            _indicatorPoint.name = INDICATOR_POINT_NAME;
            _indicatorPoint.AddToClassList(INDICATOR_POINT_STYLE_NAME);
            _indicatorPoint.pickingMode = PickingMode.Ignore;
        }

        private void RefreshIndicatorPosition()
        {
            VisualElement tabPoint = _tabPoints[_tabIndex];
            Vector2 offset = tabPoint.layout.center - _indicatorPoint.layout.center;
            Vector2 position = tabPoint.LocalToWorld(Vector2.zero);
            _indicatorPoint.transform.position = _indicatorPoint.parent.WorldToLocal(position);
        }
    }
}
