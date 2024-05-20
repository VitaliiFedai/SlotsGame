using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Scheme_", menuName = "CheckingSchemeSO", order =0)]
public class CheckingSchemeSO : ScriptableObject
{
    [SerializeField, Range(0, 2)] public int[] _hitRows = new int[5];

    private void OnValidate()
    {
        if (_hitRows.Length != 5)
        {
            _hitRows = new int[5];
        }
    }
}
