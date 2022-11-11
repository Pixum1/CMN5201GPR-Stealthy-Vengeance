using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLife : Item
{
    public override void OnCollect()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.LifeGainSound);
        PlayerController.Instance.Health.AddHP(1);
    }
}
