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

    public abstract void OnCollect();
    public virtual void OnDelete()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            OnCollect();
            OnDelete();
        }
    }
}
