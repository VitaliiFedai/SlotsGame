using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class PanelUI : MonoBehaviour
{
    private UIDocument _document;
    private Dictionary<string, Action> _onClickCallbacks = new Dictionary<string, Action>();
    private Dictionary<string, EventCallback<ChangeEvent<float>>> _onChangeCallbacks = new Dictionary<string, EventCallback<ChangeEvent<float>>>();

    public event Action OnClick;

    public void Show()
    {
        enabled = true;
    }

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _document.enabled = true;
        BindRegisteredCallbacks();
        OnBind();
    }

    private void OnDisable()
    {
        if (GetRoot() != null)
        {
            UnbindRegisteredCallbacks();
            OnUnbind();
        }
        _document.enabled = false;
    }

    protected abstract void OnBind();

    protected abstract void OnUnbind();

    protected T GetElement<T>(string elementName) where T : VisualElement
    {
        return _document.rootVisualElement?.Q<T>(elementName);
    }

    protected VisualElement GetElement(string elementName) 
    {
        return GetElement<VisualElement>(elementName);
    }

    protected List<T> Query<T>() where T: VisualElement
    {
        VisualElement root = GetRoot();
        return root != null? root.Query<T>().ToList() : new List<T>();
    }

    private void BindEvent<T>(string buttonName, EventCallback<T> callback) where T: EventBase<T>, new()
    {
        GetRoot().Q(buttonName).RegisterCallback(callback);
    }

    private void UnbindEvent<T>(string buttonName, EventCallback<T> callback) where T : EventBase<T>, new()
    {
        if (_document == null)
        {
            Debug.Log("_document == null");
        }

        VisualElement root = _document.rootVisualElement;

        if (root == null)
        {
            Debug.Log("root == null");
        }
        else
            root.Q(buttonName).UnregisterCallback(callback);
    }

    protected void SetElementEnabled(string elementName, bool enabled)
    {
        GetRoot().Q(elementName).SetEnabled(enabled);
    }

    protected void CallOnClickEvent()
    {
        OnClick?.Invoke();
    }

    //************************************************
    protected VisualElement GetRoot()
    {
        return _document != null && _document.enabled ? _document?.rootVisualElement : null;
    }

    protected void RegisterOnClickCallback(string elementName, Action callback)
    {
        if (callback == null)
        {
            Debug.Log($"Can''t {nameof(RegisterOnClickCallback)} for Element {elementName} because {nameof(callback)} is null. Skipped.");
            return;
        }

        if (GetElement(elementName) == null)
        { 
            throw new ArgumentException($"Element {elementName} not found in {_document}");
        }

        if (!_onClickCallbacks.ContainsKey(elementName))
        {
            _onClickCallbacks[elementName] = null;
            if (GetRoot() != null)
            {
                BindEvent<ClickEvent>(elementName, OnClickPerformed);
            }
        }

        _onClickCallbacks[elementName] += callback;
    }

    protected void RegisterOnChangeCallback(string elementName, EventCallback<ChangeEvent<float>> callback)
    {
        if (callback == null)
        {
            Debug.Log($"Can''t {nameof(RegisterOnChangeCallback)} for Element {elementName} because {nameof(callback)} is null. Skipped.");
            return;
        }

        if (!_onChangeCallbacks.ContainsKey(elementName))
        {
            _onChangeCallbacks[elementName] = null;
            if (GetRoot() != null)
            { 
                BindEvent<ChangeEvent<float>>(elementName, OnChangePerformed);
            }
        }

        _onChangeCallbacks[elementName] += callback;
    }

    protected void UnregisterOnClickCallback(string elementName, Action callback)
    {
        if (callback == null)
        {
            Debug.Log($"Can''t {nameof(UnregisterOnClickCallback)} for Element {elementName} because {nameof(callback)} is null. Skipped.");
            return;
        }

        if (_onClickCallbacks.ContainsKey(elementName))
        {
            _onClickCallbacks[elementName] -= callback;

            if (_onClickCallbacks[elementName] == null)
            {
                _onClickCallbacks.Remove(elementName);
            
                if (GetRoot() != null)
                {
                    UnbindEvent<ClickEvent>(elementName, OnClickPerformed);
                }
            }
        }
    }

    protected void UnregisterOnClickCallback(string elementName, EventCallback<ChangeEvent<float>> callback)
    {
        if (callback == null)
        {
            Debug.Log($"Can''t {nameof(UnregisterOnClickCallback)} for Element {elementName} because {nameof(callback)} is null. Skipped.");
            return;
        }

        if (_onChangeCallbacks.ContainsKey(elementName))
        {
            _onChangeCallbacks[elementName] -= callback;

            if (_onChangeCallbacks[elementName] == null)
            {
                _onChangeCallbacks.Remove(elementName);
            
                if (GetRoot() != null)
                {
                    UnbindEvent<ChangeEvent<float>>(elementName, OnChangePerformed);
                }
            }
        }
    }

    private void BindRegisteredCallbacks()
    {
        foreach (string elementName in _onClickCallbacks.Keys)
        {
            BindEvent<ClickEvent>(elementName, OnClickPerformed);
        }
        foreach (string elementName in _onChangeCallbacks.Keys)
        {
            BindEvent<ChangeEvent<float>>(elementName, OnChangePerformed);
        }
    }

    private void UnbindRegisteredCallbacks()
    {
        foreach (string elementName in _onClickCallbacks.Keys)
        {
            UnbindEvent<ClickEvent>(elementName, OnClickPerformed);
        }
        foreach (string elementName in _onChangeCallbacks.Keys)
        {
            UnbindEvent<ChangeEvent<float>>(elementName, OnChangePerformed);
        }
    }

    private void OnClickPerformed(ClickEvent evt)
    {
        if (evt.currentTarget is VisualElement element)
        {
            if (_onClickCallbacks.ContainsKey(element.name))
            {
                if (_onClickCallbacks[element.name] == null)
                {
                    throw new InvalidOperationException($"{this}  _onClickCallbacks[{element.name}] == null");
                }

                _onClickCallbacks[element.name]?.Invoke();
                OnClick?.Invoke();
            }
        }
    }

    private void OnChangePerformed(ChangeEvent<float> evt)
    {
        if (evt.currentTarget is VisualElement element)
        {
            if (_onChangeCallbacks.ContainsKey(element.name))
            {
                _onChangeCallbacks[element.name](evt);
            }
        }
    }
}
