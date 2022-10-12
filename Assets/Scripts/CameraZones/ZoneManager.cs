using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] public LayerMask playerLayer;

    [Header("Visualization")]
    [SerializeField, Range(.1f, 1f)] private float m_Scale = .5f;
    [SerializeField] private Vector2 m_Position = Vector2.zero;

    /// <summary>
    /// Creates a new CameraZone and returns it.
    /// </summary>
    /// <returns>Instance of CameraZone</returns>
    public GameObject CreateZone()
    {
        GameObject zone = new GameObject("CameraZone");
        zone.transform.SetParent(transform);
        zone.transform.localScale = new Vector3(40, 22f, 1);
        zone.AddComponent<BoxCollider>().isTrigger = true;
        CameraZone z = zone.AddComponent<CameraZone>();
        z.playerLayer = playerLayer;
        return zone;
    }
}
