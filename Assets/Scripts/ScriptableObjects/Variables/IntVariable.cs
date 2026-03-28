using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Int")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int value;

    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public event Action<int> OnValueChanged;

    public void SetValue(int newValue) => Value = newValue;
    public void ApplyChange(int amount) => Value += amount;
}
