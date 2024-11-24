using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : CharacterEntity
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public virtual void TakeDamage(float damageValue)
    {
        CharacterHealthComponent.TakeDamage(damageValue);
    }

    protected override void Die()
    {
        base.Die();
    }
}
