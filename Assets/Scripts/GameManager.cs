using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Box Settings")]
    public GameObject boxPrefab;
    public Transform boxSpawnPoint;
    public Transform[] boxSlots; // 4 empty slots inside box

    private GameObject currentBox;
    private int traysInCurrentBox = 0;

    [Header("Timer & Score Settings")]
    public float gameDuration = 60f; // seconds
    private float timer;
    private int score = 0;

    [Header("UI References (Optional)")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    public GameObject gameOverPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timer = gameDuration;
        UpdateScoreUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        // Timer countdown
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            if (timer <= 0)
            {
                timer = 0;
                GameOver();
            }
        }
    }

    public void PackTrayIntoBox(Tray tray)
    {
        if (currentBox == null)
        {
            currentBox = Instantiate(boxPrefab, boxSpawnPoint.position, Quaternion.identity);
            traysInCurrentBox = 0;
        }

        Vector3 slotPosition = GetNextBoxSlotPosition();

        Sequence packSequence = DOTween.Sequence();

        // 🔥 Fancier movement + rotation
        packSequence.Append(tray.transform.DOMove(slotPosition, 0.5f).SetEase(Ease.InOutBack));
        packSequence.Join(tray.transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360));

        tray.transform.SetParent(currentBox.transform);

        traysInCurrentBox++;

        score += 10; // 🎯 +10 points per tray packed
        UpdateScoreUI();

        if (traysInCurrentBox >= 4)
        {
            StartCoroutine(DestroyBoxWithTrays());
        }
    }

    private Vector3 GetNextBoxSlotPosition()
    {
        if (traysInCurrentBox < boxSlots.Length)
        {
            return boxSlots[traysInCurrentBox].position;
        }
        else
        {
            Debug.LogWarning("No more slots in box!");
            return boxSpawnPoint.position;
        }
    }

    private IEnumerator DestroyBoxWithTrays()
    {
        yield return new WaitForSeconds(0.5f);

        if (currentBox != null)
        {
            //  Truck Animation 

            Sequence truckSequence = DOTween.Sequence();

            // 1. Small jump upwards
            truckSequence.Append(currentBox.transform.DOMoveY(currentBox.transform.position.y + 1f, 0.4f).SetEase(Ease.OutQuad));

            // 2. Small playful rotation
            truckSequence.Join(currentBox.transform.DORotate(new Vector3(0, 0, Random.Range(-15f, 15f)), 0.4f));

            // 3. Quick move sideways (simulate truck pickup)
            float direction = Random.value > 0.5f ? 1f : -1f; // Random left or right
            Vector3 targetPos = currentBox.transform.position + new Vector3(direction * 10f, 2f, 0); // fly off

            truckSequence.Append(currentBox.transform.DOMove(targetPos, 0.8f).SetEase(Ease.InBack));

            truckSequence.OnComplete(() =>
            {
                Destroy(currentBox);
            });
        }

        traysInCurrentBox = 0;
        currentBox = null;
    }


    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // You could add more logic here, like Stop Spawning Glasses
    }
}
