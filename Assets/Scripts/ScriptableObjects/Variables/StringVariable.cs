using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/String")]
public class StringVariable : ScriptableObject
{
    [SerializeField] private string value;

    public string Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public event Action<string> OnValueChanged;

    public void SetValue(string newValue) => Value = newValue;
}
