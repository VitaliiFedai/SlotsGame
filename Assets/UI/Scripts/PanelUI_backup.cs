using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class PanelUI_backup : MonoBehaviour
{
    private UIDocument _document;

    public event Action OnClick;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    public void OnEnable()
    {
        _document.enabled = true;
        Bind();
    }

    public void OnDisable()
    {
        if (GetRoot() != null)
        {
            Unbind();
        }
        _document.enabled = false;
    }

    protected abstract void Bind();
    protected abstract void Unbind();

    protected T GetElement<T>(string elementName) where T: VisualElement
    {
        VisualElement root = _document.rootVisualElement;
        if (root == null)
        {
            Debug.Log("Oops!");
        }

        return root?.Q<T>(elementName);
    }

    protected T GetElement<T>() where T: VisualElement
    {
        VisualElement root = _document.rootVisualElement;
        return root?.Q<T>();
    }

    protected void BindClickEvent(string buttonName, EventCallback<ClickEvent> callback)
    {
        BindEvent(buttonName, callback);
    }

    protected void UnbindClickEvent(string buttonName, EventCallback<ClickEvent> callback)
    {
        UnbindEvent(buttonName, callback);
    }

    protected void BindChangeEvent<T>(string buttonName, EventCallback<ChangeEvent<T>> callback)
    {
        BindEvent(buttonName, callback);
    }

    protected void UnbindChangeEvent<T>(string buttonName, EventCallback<ChangeEvent<T>> callback)
    {
        UnbindEvent(buttonName, callback);
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

    protected VisualElement GetRoot()
    { 
        return _document.rootVisualElement;
    }

    protected void SetElementEnabled(string elementName, bool enabled)
    {
        GetRoot().Q(elementName).SetEnabled(enabled);
    }

    protected bool GetElementEnabledSelf(string elementName)
    {
        return GetRoot().Q(elementName).enabledSelf;
    }

    protected bool GetElementEnabledInHierarchy(string elementName)
    {
        return GetRoot().Q(elementName).enabledInHierarchy;
    }

    protected void CallOnClickEvent()
    {
        OnClick?.Invoke();
    }
}
