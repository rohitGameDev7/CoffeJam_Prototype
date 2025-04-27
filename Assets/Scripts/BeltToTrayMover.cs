using UnityEngine;
using DG.Tweening;

public class BeltToTrayMover : MonoBehaviour
{
    private Vector3 beltTarget;
    private float moveSpeed;
    private Tray targetTray;
    private bool movingToBelt = true;
    private bool landedInTray = false;

    public void Init(Vector3 beltEndPoint, float speed, Tray tray)
    {
        beltTarget = beltEndPoint;
        moveSpeed = speed;
        targetTray = tray;
    }

    private void Update()
    {
        if (landedInTray) return;

        if (movingToBelt)
        {
            transform.position = Vector3.MoveTowards(transform.position, beltTarget, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, beltTarget) < 0.1f)
            {
                MoveToTray();
            }
        }
    }

    private void MoveToTray()
    {
        if (targetTray == null || targetTray.IsFull())
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetSlot = targetTray.GetNextGlassSlot();
        movingToBelt = false;
        landedInTray = true;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        transform.DOMove(targetSlot, 0.4f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                transform.position = targetSlot;

                Sequence juicySequence = DOTween.Sequence();
                juicySequence.Append(transform.DOScale(new Vector3(1.3f, 0.7f, 1.3f), 0.1f).SetEase(Ease.OutQuad));
                juicySequence.Append(transform.DOScale(new Vector3(0.8f, 1.4f, 0.8f), 0.08f).SetEase(Ease.OutQuad));
                juicySequence.Append(transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutElastic));

                juicySequence.OnComplete(() =>
                {
                    if (targetTray != null)
                    {
                        targetTray.FillOneGlass(this.gameObject);
                    }
                });
            });
    }
}
