using UnityEngine;

public class DoorTeleporter : MonoBehaviour
{
    public string acceptedColor = "Red";
    public AssemblySlotManager slotManager;

    private void OnTriggerEnter(Collider other)
    {
        TrayColor tray = other.GetComponent<TrayColor>();
        if (tray != null && !tray.hasTeleported)
        {
            if (tray.colorName == acceptedColor)
            {
                Transform nextSlot = slotManager.GetNextAvailableSlot();
                if (nextSlot != null)
                {
                    tray.transform.position = nextSlot.position;
                    tray.hasTeleported = true;

                    TrayDragHandler drag = tray.GetComponent<TrayDragHandler>();
                    if (drag != null)
                        drag.enabled = false;

                    // 👇 Start animation
                    StartCoroutine(AnimateTrayBounce(tray.transform));
                }
            }
        }
    }

    private System.Collections.IEnumerator AnimateTrayBounce(Transform tray)
    {
        Vector3 originalScale = tray.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        float duration = 0.1f;
        float t = 0f;

        // Scale up
        while (t < duration)
        {
            tray.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        tray.localScale = targetScale;

        // Wait a brief moment
        yield return new WaitForSeconds(0.05f);

        // Scale back
        t = 0f;
        while (t < duration)
        {
            tray.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        tray.localScale = originalScale;
    }
}
