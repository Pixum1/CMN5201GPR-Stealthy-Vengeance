using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] public LayerMask playerLayer;
    private List<CameraZone> zones = new List<CameraZone>();

    [Header("Visualization")]
    [SerializeField, Range(.1f,1f)] private float m_Scale = .5f;
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
        zones.Add(z);
        return zone;
    }
    public void RemoveZoneFromList(CameraZone _z)
    {
        zones.Remove(_z);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < zones.Count; i++)
        {
            Vector2 newScale = zones[i].transform.localScale * m_Scale;
            Vector2 newPos = (Vector2)zones[i].transform.position + m_Position;

            if (i == 0)
                Gizmos.DrawWireCube(newPos, newScale);

            else
            {
                Vector2 prevPos = zones[i - 1].transform.position * m_Scale;
                Vector2 prevScale = zones[i - 1].transform.localScale * m_Scale;

                newPos = prevPos + (prevScale / 2f) + (newScale / 2f) + m_Position;
                Gizmos.DrawWireCube(newPos, newScale);
            }
        }
    }
}
