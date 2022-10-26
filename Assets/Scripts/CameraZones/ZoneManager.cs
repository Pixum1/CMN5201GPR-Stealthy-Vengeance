using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] private Vector2 standardSize = new Vector2(40,22);
    [SerializeField] private GameObject m_MapVisuals;

    /// <summary>
    /// Creates a new CameraZone and returns it.
    /// </summary>
    /// <returns>Instance of CameraZone</returns>
    public GameObject CreateZone()
    {
        // Get new Zone ID
        ushort id = 0;
        CameraZone[] zones = FindObjectsOfType<CameraZone>();

        for (int i = 0; i < zones.Length; i++)
        {
            if (id <= zones[i].ID)
                id = zones[i].ID++;
        }

        // Create a new zone
        GameObject zone = new GameObject("CameraZone");
        zone.transform.SetParent(transform);
        zone.transform.localScale = standardSize;
        zone.AddComponent<BoxCollider>().isTrigger = true;
        CameraZone z = zone.AddComponent<CameraZone>();
        z.playerLayer = playerLayer;
        z.ID = _id;

        // Create visuals for the minimap
        GameObject visual = Instantiate(m_MapVisuals);
        visual.transform.SetParent(zone.transform);
        visual.name = "MapVisual";
        z.MapVisual = visual.GetComponent<SpriteRenderer>();
        z.MapVisual.drawMode = SpriteDrawMode.Sliced;
        z.MapVisual.size = zone.transform.localScale;

        return zone;
    }
}
