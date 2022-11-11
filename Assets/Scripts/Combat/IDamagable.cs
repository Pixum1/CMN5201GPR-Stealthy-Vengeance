using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public abstract void GetDamage(int _value, Vector3 _knockbackDir);
}
