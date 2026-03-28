using UnityEngine;

public class RuntimeSetRegistrar : MonoBehaviour
{
    [SerializeField] private PlayerSeatRuntimeSet set;

    private void OnEnable()
    {
        if (set != null)
        {
            set.Add(transform);
        }
    }

    private void OnDisable()
    {
        if (set != null)
        {
            set.Remove(transform);
        }
    }
}
