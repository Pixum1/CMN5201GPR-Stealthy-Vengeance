using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLife : Item
{
    public override void OnCollect()
    {
        PlayerController.Instance.Health.HP += 1;
    }
}
