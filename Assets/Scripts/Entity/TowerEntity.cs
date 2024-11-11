using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEntity : CharacterEntity
{
    public enum TowerType { Null, A, B, C };
    public TowerType towerType;

    [SerializeField] protected List<Sprite> towerBaseASprite;
    [SerializeField] protected List<Sprite> towerBaseBSprite;
    [SerializeField] protected List<Sprite> towerBaseCSprite;

    [SerializeField] protected List<Sprite> towerWeaponASprite;
    [SerializeField] protected List<Sprite> towerWeaponBSprite;
    [SerializeField] protected List<Sprite> towerWeaponCSprite;

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
