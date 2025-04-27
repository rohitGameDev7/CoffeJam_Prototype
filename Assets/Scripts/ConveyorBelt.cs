using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform endPoint;
    public float glassMoveSpeed = 2f;
    public float spawnInterval = 2f;

    public List<Tray> trays = new List<Tray>();
    public List<GlassColorPrefab> glassPrefabs = new List<GlassColorPrefab>();

    private void Start()
    {
        StartCoroutine(SpawnGlasses());
    }

    IEnumerator SpawnGlasses()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            TrySpawnGlass();
        }
    }

    void TrySpawnGlass()
    {
        List<Tray> availableTrays = GetAvailableTrays();

        if (availableTrays.Count == 0)
        {
            Debug.LogWarning("No trays available on assembly desk! No glass will spawn.");
            return; //  Don't spawn anything
        }

        Tray targetTray = availableTrays[Random.Range(0, availableTrays.Count)];
        string targetColor = targetTray.trayColor;

        GlassColorPrefab prefabEntry = glassPrefabs.Find(p => p.colorName == targetColor);

        if (prefabEntry == null)
        {
            Debug.LogError($"No glass prefab found for color: {targetColor}");
            return;
        }

        GameObject glassObj = Instantiate(prefabEntry.prefab, spawnPoint.position, Quaternion.identity);
        BeltToTrayMover mover = glassObj.GetComponent<BeltToTrayMover>();
        if (mover != null)
        {
            mover.Init(endPoint.position, glassMoveSpeed, targetTray);
        }
    }

    List<Tray> GetAvailableTrays()
    {
        List<Tray> available = new List<Tray>();

        foreach (var tray in trays)
        {
            if (tray != null)
            {
                Debug.Log($"Checking Tray {tray.trayColor}, OnDesk: {tray.isOnAssemblyDesk}, IsFull: {tray.IsFull()}");
            }

            if (tray != null && tray.isOnAssemblyDesk && !tray.IsFull())
            {
                available.Add(tray);
            }
        }

        Debug.Log($"Available trays count: {available.Count}");
        return available;
    }
}

[System.Serializable]
public class GlassColorPrefab
{
    public string colorName;
    public GameObject prefab;
}
