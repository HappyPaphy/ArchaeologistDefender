using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_TowerB : Bullet
{
    [SerializeField] private Animator anim;

    void Start()
    {

    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
