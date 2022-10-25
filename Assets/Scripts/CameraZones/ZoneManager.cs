using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] private Vector2 standardSize = new Vector2(40,22);

    [Header("Visualization")]
    private ushort id;

    /// <summary>
    /// Creates a new CameraZone and returns it.
    /// </summary>
    /// <returns>Instance of CameraZone</returns>
    public GameObject CreateZone()
    {
        GameObject zone = new GameObject("CameraZone");
        zone.transform.SetParent(transform);
        zone.transform.localScale = standardSize;
        zone.AddComponent<BoxCollider>().isTrigger = true;
        CameraZone z = zone.AddComponent<CameraZone>();
        z.playerLayer = playerLayer;
        z.ID = id;
        id++;
        return zone;
    }
}
