using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraZone : MonoBehaviour
{
    public bool WasVisited = false;
    private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            if (value == isActive) return;

            isActive = value;

            switch (m_RoomType)
            {
                case ERoomType.normal:
                    if (isActive)
                        m_SpawnAbles.SpawnAll(enemiesInRoom);
                    else
                        m_SpawnAbles.DestroyAll(enemiesInRoom);
                    break;
                case ERoomType.boss:
                    if (isActive && !WasVisited)
                        StartCoroutine(StartEvent());
                    break;
            }

            WasVisited = true;
        }
    }
    [SerializeField]
    public LayerMask playerLayer;
    [HideInInspector]
    public Collider col;
    [SerializeField]
    public float cameraOrthographicSize = 11;

    private enum ERoomType
    {
        normal,
        boss,
    }
    [SerializeField] private ERoomType m_RoomType;

    [SerializeField] private SpawnAbles m_SpawnAbles;
    [SerializeField] private EnemyWaves[] m_Waves;
    [SerializeField] private Vector2[] m_WavePositions;
    [SerializeField] private GameObject[] EntranceBarriers;
    private List<GameObject> enemiesInRoom = new List<GameObject>();

    private void Awake()
    {
        col = GetComponent<Collider>();
        CheckForPlayer();
    }

    private void Update()
    {
        CheckForPlayer();
    }
    /// <summary>
    /// Checks if the player is within the bounds of the zone.
    /// </summary>
    private void CheckForPlayer()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0f, playerLayer);

        if (cols.Length != 0)
            IsActive = true;

        else
            IsActive = false;
    }
    private IEnumerator StartEvent()
    {
        Debug.Log("Event started");

        // Player moves to middle of the screen
        StartCoroutine(MovePlayerToPos(new Vector2(transform.position.x, PlayerController.Instance.transform.position.y)));

        yield return new WaitUntil(() => Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x) <= 1f);
        // Close entrances
        CloseEntrance();

        // Start Wave 1 & wait untill all enemies of that wave spawned
        // Then spawn next Wave
        for (int i = 0; i < m_Waves.Length; i++)
        {
            StartCoroutine(m_Waves[i].StartWave(enemiesInRoom, m_WavePositions));
            yield return new WaitUntil(() => m_Waves[i].IsFinished); // Doesn't work
        }

    }

    private IEnumerator MovePlayerToPos(Vector2 _pos)
    {
        PlayerController p = PlayerController.Instance;
        bool save = p.LookAtMouse;

        p.IsInEvent = true;
        p.LookAtMouse = false;

        Rigidbody2D rb = PlayerController.Instance.RigidBody;
        rb.velocity = Vector2.zero;

        float xDir = Mathf.Sign(_pos.x - p.transform.position.x);
        float acceleration = 70f;
        float maxSpeed = 7f;

        while (((Vector2)p.transform.position - _pos).sqrMagnitude > 1f)
        {
            rb.velocity = Vector2.right * xDir * acceleration;
            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y); //Clamp velocity when max speed is reached!

            yield return null;
        }

        rb.velocity = Vector2.zero;

        p.LookAtMouse = save;
        p.IsInEvent = false;
    }

    private void CloseEntrance()
    {
        for (int i = 0; i < EntranceBarriers.Length; i++)
            EntranceBarriers[i].SetActive(true);
    }
    private void OpenEntrance()
    {
        for (int i = 0; i < EntranceBarriers.Length; i++)
            EntranceBarriers[i].SetActive(false);
    }

    private void OnDrawGizmos()
    {
        #region Zone Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale); //visualize zone bounds

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector2(cameraOrthographicSize * 2 * Camera.main.aspect, cameraOrthographicSize * 2));
        #endregion

        #region Spawnpoints
        if (m_SpawnAbles.Enemies != null)
        {
            for (int i = 0; i < m_SpawnAbles.Enemies.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(m_SpawnAbles.Enemies[i].SpawnPoint, Vector2.one);

                for (int k = 0; k < m_SpawnAbles.Enemies[i].Waypoints.Length; k++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(m_SpawnAbles.Enemies[i].Waypoints[k], Vector2.one);
                }
            }
        }
        if (m_WavePositions != null)
        {
            for (int k = 0; k < m_WavePositions.Length; k++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(m_WavePositions[k], Vector2.one);
            }
        }
        #endregion
    }
}
