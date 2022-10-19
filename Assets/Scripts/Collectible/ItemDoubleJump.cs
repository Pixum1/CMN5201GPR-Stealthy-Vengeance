using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDoubleJump : Item
{
    public override void OnCollect()
    {
        PlayerController.Instance.AmountOfJumps = 2;
    }
}