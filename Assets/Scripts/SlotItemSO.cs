using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SlotItemSO", menuName = "SlotItemSO", order = 0)]
public class SlotItemSO : ScriptableObject
{
    public string _name;
    public Sprite _sprite;
    public Color _color;
    public float _spawnWeight = 1f;
    public int[] _betMultiplier = new int[3];
    public bool _isWild;
}
