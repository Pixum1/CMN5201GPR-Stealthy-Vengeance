using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Collectible : MonoBehaviour, ICollectable
{
    protected bool Collected = false;
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

    }

    private void Start()
    {
        SaveSystem.Instance.e_LoadGame += OnRespawn;

        OnRespawn();
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
            GameManager.Instance.CollectedCollectibles.Add(this);

            OnCollect();
            OnDelete();
        }
    }
}
