using UnityEngine;

public class Tray : MonoBehaviour
{
    public string trayColor;
    public int capacity = 4;
    private int currentCount = 0;
    public Transform[] glassSlots;
    public bool isOnAssemblyDesk = false;
    public bool isPacked = false; //  New

    public bool IsFull()
    {
        return currentCount >= capacity;
    }

    public Vector3 GetNextGlassSlot()
    {
        if (currentCount >= glassSlots.Length)
        {
            Debug.LogWarning("Tray is full!");
            return transform.position;
        }
        return glassSlots[currentCount].position;
    }

    public void FillOneGlass(GameObject glass)
    {
        if (currentCount >= capacity)
        {
            Destroy(glass); // Safety
            return;
        }

        Destroy(glass); // ❗ Important: Destroy glass when it reaches tray (no parenting headache)
        currentCount++;

        if (IsFull())
        {
            TryPack();
        }
    }

    private void TryPack()
    {
        if (isOnAssemblyDesk)
        {
            isPacked = true;
            GameManager.Instance.PackTrayIntoBox(this);
        }
    }

    public void MoveToAssemblyDesk(Vector3 deskPosition)
    {
        transform.position = deskPosition;
        isOnAssemblyDesk = true;
    }
}
