using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraZone : MonoBehaviour
{
    public ushort ID;
    public bool WasVisited = false;
    private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            if (value == isActive) return;

            isActive = value;

            if (value == true)
            {
                ZoneManager.Instance.CurrentActiveZone = this;
                // if this is the first time visiting this room
                if (!WasVisited)
                {
                    if (GameManager.Instance != null)
                        for (int i = 0; i < GameManager.Instance.ZoneSaves.Count; i++)
                        {
                            // Find this zone in the GameManagers zone list
                            if (GameManager.Instance.ZoneSaves[i].ZoneID != ID) continue;

                            // Create a new data object at the location in the list
                            GameManager.Instance.ZoneSaves[i] = new CameraZoneSaveData(ID, true);
                            Debug.Log("Set new Values for room " + ID);
                        }
                }
            }

            TriggerRoomAction();

            WasVisited = true;
        }
    }

    [HideInInspector] public Collider col;
    [SerializeField] public float cameraOrthographicSize = 11;

    private enum ERoomType
    {
        normal,
        boss,
    }
    [SerializeField] private ERoomType m_RoomType;

    [Header("Normal Room")]
    [SerializeField] private EnemyData[] m_Enemies;

    [Header("Boss Room")]
    [SerializeField] private EnemyWaves[] m_Waves;
    [SerializeField] private GameObject[] m_EntranceBarriers;
    private enum EUnlockAbility
    {
        none,
        wallHang,
        wallClimb,
        wallHop,
        doubleJump,
        dash,
    }
    [SerializeField] private EUnlockAbility m_UnlockAbility;

    private List<GameObject> enemiesInRoom = new List<GameObject>();
    [HideInInspector] public SpriteRenderer MapVisual;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }
    private void TriggerRoomAction()
    {
        switch (m_RoomType)
        {
            case ERoomType.normal:
                if (isActive)
                    SpawnAll(enemiesInRoom);
                else
                    DestroyAll(enemiesInRoom);
                break;
            case ERoomType.boss:
                if (isActive && !WasVisited)
                {
                    StartCoroutine(StartEvent());
                }
                break;
        }
    }

    private void Start()
    {
        // Register this room to the GameManager
        GameManager.Instance.ZoneSaves.Add(new CameraZoneSaveData(ID, WasVisited));
    }

    public void UpdateRoom()
    {
        CheckForPlayer();

        for (int i = 0; i < enemiesInRoom.Count; i++)
        {
            if (enemiesInRoom[i] == null)
                enemiesInRoom.Remove(enemiesInRoom[i]);
        }
    }
    /// <summary>
    /// Checks if the player is within the bounds of the zone.
    /// </summary>
    private void CheckForPlayer()
    {
        // dont check for the player if this is already the current zone
        if (IsActive && ZoneManager.Instance.CurrentActiveZone == this) return;

        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0f, ZoneManager.Instance.PlayerLayer);

        if (cols.Length != 0)
            IsActive = true;

        else
            IsActive = false;
    }
    private void SpawnAll(List<GameObject> _enemyList)
    {
        for (int i = 0; i < m_Enemies.Length; i++)
        {
            m_Enemies[i].Spawn(_enemyList);
        }
    }
    private void DestroyAll(List<GameObject> _enemyList)
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            if (_enemyList[i] == null) continue;

            GameObject.Destroy(_enemyList[i]);
        }
    }

    private IEnumerator StartEvent()
    {
        Debug.Log("Event started");
        ZoneManager.Instance.StartEncounterEvent.RaiseEvent();

        // Player moves to middle of the screen
        yield return MovePlayerToPos(new Vector2(transform.position.x, PlayerController.Instance.transform.position.y));
        // Close entrances
        CloseEntrance();

        // Start Wave 1 & wait untill all enemies of that wave spawned
        // Then spawn next Wave
        for (int i = 0; i < m_Waves.Length; i++)
            yield return m_Waves[i].StartWave(enemiesInRoom);

        // Wait until all enemies are dead
        yield return new WaitUntil(() => enemiesInRoom.Count <= 0);
        Debug.Log("All enemies dead");

        // Open doors
        OpenEntrance();

        // Unlock Ability
        switch (m_UnlockAbility)
        {
            case EUnlockAbility.none:
                break;
            case EUnlockAbility.wallHang:
                PlayerController.Instance.AllowWallHang = true;
                break;
            case EUnlockAbility.wallClimb:
                PlayerController.Instance.AllowWallClimb = true;
                break;
            case EUnlockAbility.wallHop:
                PlayerController.Instance.AllowWallHops = true;
                break;
            case EUnlockAbility.doubleJump:
                PlayerController.Instance.AmountOfJumps = 2;
                break;
            case EUnlockAbility.dash:
                PlayerController.Instance.AllowDashing = true;
                break;
        }
        // Play animation or particles
        ZoneManager.Instance.EndEncounterEvent.RaiseEvent();
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
        for (int i = 0; i < m_EntranceBarriers.Length; i++)
            m_EntranceBarriers[i].SetActive(true);
    }
    private void OpenEntrance()
    {
        for (int i = 0; i < m_EntranceBarriers.Length; i++)
            m_EntranceBarriers[i].SetActive(false);
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
        if (m_Enemies != null)
        {
            for (int i = 0; i < m_Enemies.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube((Vector2)m_Enemies[i].SpawnPoint, Vector2.one);

                for (int k = 0; k < m_Enemies[i].Waypoints.Length; k++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube((Vector2)m_Enemies[i].Waypoints[k], Vector2.one);
                }
            }
        }
        if (m_Waves != null)
        {
            for (int i = 0; i < m_Waves.Length; i++)
            {
                Gizmos.color = Color.blue;
                if (m_Waves[i].SpawnPoints != null)
                {
                    for (int k = 0; k < m_Waves[i].SpawnPoints.Length; k++)
                    {
                        Gizmos.DrawWireCube((Vector2)m_Waves[i].SpawnPoints[k], Vector2.one);
                    }
                }
            }
        }
        #endregion
    }
    private void OnValidate()
    {
        if (MapVisual != null)
        {
            MapVisual.transform.localScale = new Vector2(1 / transform.localScale.x, 1 / transform.localScale.y);
            MapVisual.size = transform.localScale;
        }
    }
}
