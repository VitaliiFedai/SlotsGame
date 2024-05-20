using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SlotItem : MonoBehaviour
{
    private const int BLINK_DELAY = 500;
    private const int BLINK_COUNT = 1;

    [SerializeField] private SlotItemSO _itemSO;
    [SerializeField] private Image _image;

    public string Name => _itemSO != null ? _itemSO.name : "";
    public Color Color => _itemSO != null ? _itemSO._color : Color.black;

    public SlotItemSO ItemSO { get => _itemSO; }


    private void Awake()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
    }

    public async Task Blink()
    { 
        Color color = _image.color;
        for (int i = 0; i < BLINK_COUNT; i++) 
        {
            _image.color = Color.cyan;
            await Task.Delay(BLINK_DELAY);
            _image.color = color;
            await Task.Delay(BLINK_DELAY);
            if (i < BLINK_COUNT - 1)
            {
                await Task.Delay(BLINK_DELAY);
            }
        }
    }

    public void SetItemSO(SlotItemSO itemSO)
    { 
        _itemSO = itemSO;
        _image.sprite = _itemSO._sprite;
        _image.color = _itemSO._color;
    }

    public bool SameItems(SlotItem other)
    { 
        if (other == null || _itemSO == null)
        {
            return false;
        }
        return _itemSO == other._itemSO || _itemSO._isWild || other.ItemSO._isWild;
    }

    public void AssignFrom(SlotItem other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        SetItemSO(other._itemSO);
    }
}
