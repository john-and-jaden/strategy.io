using UnityEngine;
using UnityEngine.Events;

public class XpSystem : MonoBehaviour
{
    private int xp;
    [System.Serializable] public class XpChangedEvent : UnityEvent<int> { }
    private XpChangedEvent onXpChanged = new XpChangedEvent();
    public void IncrementXp(int amount)
    {
        xp += amount;
        onXpChanged.Invoke(xp);
    }

    public void AddXpChangedListener(UnityAction<int> listener)
    {
        onXpChanged.AddListener(listener);
    }

    public void RemoveXpChangedListener(UnityAction<int> listener)
    {
        onXpChanged.RemoveListener(listener);
    }
}
