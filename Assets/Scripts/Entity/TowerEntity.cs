using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEntity : CharacterEntity
{
    public enum TowerType { Null };
    public TowerType towertType;

    public virtual void TakeDamage(float damageValue)
    {
        CharacterHealthComponent.TakeDamage(damageValue);
    }

    protected override void Die()
    {

        /*if (GameData.Instance != null)
        {
            for (int i = 0; i < orbsToSpawn; i++)
            {
                GameData.Instance.CreateOrbAtTransform(transform);
            }
        }
        QuestManager.Instance.TrackKill(enemyType);*/

        base.Die(); // Call the base Die method to handle other death-related behaviors
    }
}
