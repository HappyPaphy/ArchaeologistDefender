using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Soldier : Bullet
{
    [SerializeField] private Animator anim;
    Vector2 direction;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        rb.AddForce(direction * moveSpeed, ForceMode2D.Impulse);
    }

    private void Update()
    {

    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
