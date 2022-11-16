using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Item : MonoBehaviour, ICollectable
{
    private BoxCollider2D col;

    protected bool Collected = false;
    [HideInInspector] public bool Despawn = false;

    [HideInInspector] public float despawnTime = 15f;
    private float despawnTimer;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void Start()
    {
        //SaveSystem.Instance.e_LoadGame += OnRespawn;

        //OnRespawn();

        despawnTimer = despawnTime;
    }

    private void Update()
    {
        if (Despawn)
        {
            despawnTimer -= Time.deltaTime;

            if (despawnTimer <= 0)
                Destroy(this.gameObject);
        }
    }
    public void Spawn()
    {
        this.gameObject.SetActive(true);
    }
    public abstract void OnCollect();
    public virtual void OnDelete()
    {
        this.gameObject.SetActive(false);
    }
    private void OnRespawn()
    {
        for (int i = 0; i < GameManager.Instance.CollectedCollectibles.Count; i++)
        {
            if (GameManager.Instance.CollectedCollectibles[i] == this)
            {
                OnDelete();
                return;
            }
        }

        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            if (!Despawn)
                GameManager.Instance.CollectedCollectibles.Add(this);

            OnCollect();
            OnDelete();
        }
    }
}
