using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampHealth : Health
{
    public override void GetDamage(int _value, Vector3 _knockbackDir)
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.HitObjectSound);
        base.GetDamage(_value, _knockbackDir);
    }
}
