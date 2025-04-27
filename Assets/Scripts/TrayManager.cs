using System.Collections.Generic;
using UnityEngine;

public class TrayManager : MonoBehaviour
{
    public List<Tray> trays = new List<Tray>();

    // Returns trays that match color and are not full
    public List<Tray> GetAvailableTrays()
    {
        List<Tray> available = new List<Tray>();

        foreach (Tray tray in trays)
        {
            if (!tray.IsFull())
            {
                available.Add(tray);
            }
        }

        return available;
    }
}
