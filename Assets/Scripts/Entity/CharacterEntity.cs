using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public HealthComponent CharacterHealthComponent;

    protected virtual void Awake()
    {
        CharacterHealthComponent.SetHP(CharacterHealthComponent.MaxHP);
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        UpdateEntity();
    }

    protected virtual void UpdateEntity()
    {
        if (CharacterHealthComponent.CurrentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Implement what happens when the character dies
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
}

[Serializable]
public class HealthComponent
{
    public float MaxHP => _maxHP;
    [SerializeField] private float _maxHP;

    public float CurrentHP => _currentHP;
    [SerializeField] private float _currentHP;

    public bool IsInvulnerable => _isInvulnerable;
    [SerializeField] bool _isInvulnerable;

    public void SetHP(float hpValue)
    {
        _currentHP = hpValue;
    }

    public void Heal(float healValue)
    {
        _currentHP = Mathf.Clamp(_currentHP += healValue, 0, _maxHP);
    }

    public void TakeDamage(float damageValue)
    {
        if (_isInvulnerable)
        {
            return;
        }

        _currentHP = Mathf.Clamp(_currentHP -= damageValue, 0, _maxHP);

    }

    public void SetMaxHP(int value)
    {
        _maxHP = value;
    }
}


