using UnityEngine;

public class AssemblySlotManager : MonoBehaviour
{
    public Transform[] slots;
    private int currentIndex = 0;

    public Transform GetNextAvailableSlot()
    {
        if (currentIndex >= slots.Length)
        {
            Debug.LogWarning("No more assembly slots available.");
            return null;
        }

        Transform slot = slots[currentIndex];
        currentIndex++;
        return slot;
    }
}
