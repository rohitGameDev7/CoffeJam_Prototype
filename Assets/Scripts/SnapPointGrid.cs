using UnityEngine;
using UnityEditor;

public class SnapPointGenerator : MonoBehaviour
{
    public int gridRows = 6;
    public int gridColumns = 6;
    public float spacing = 2f;
    public Vector3 origin = Vector3.zero;

    [ContextMenu("Generate Snap Points")]
    public void GenerateSnapPoints()
    {
        GameObject parent = new GameObject("SnapPoints");

        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                Vector3 pos = origin + new Vector3(col * spacing, 0f, row * spacing);
                GameObject snap = new GameObject($"Snap_{row}_{col}");
                snap.transform.position = pos;
                snap.transform.parent = parent.transform;
            }
        }

        Debug.Log("Snap points generated!");
    }
}
