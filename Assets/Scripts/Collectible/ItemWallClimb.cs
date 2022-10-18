using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWallClimb : Item
{
    public override void OnCollect()
    {
        PlayerController.Instance.AllowWallClimb = true;
    }
}
