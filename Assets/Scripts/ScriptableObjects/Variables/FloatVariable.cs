using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Float")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] private float value;

    public float Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public event Action<float> OnValueChanged;

    public void SetValue(float newValue) => Value = newValue;
    public void ApplyChange(float amount) => Value += amount;
}
