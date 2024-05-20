using UnityEngine;


[CreateAssetMenu(fileName = "ShopItemSO", menuName = "ShopItemSO")]
public class ItemSO : ScriptableObject
{
    public string _name;
    public Sprite _sprite;
    public SlotItemSO _slotItemSO;
}
 
