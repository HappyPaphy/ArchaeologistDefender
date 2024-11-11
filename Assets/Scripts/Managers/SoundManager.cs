using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource[] enemySoldierSounds;
    [SerializeField] private AudioSource[] enemyMonsterSounds;
    [SerializeField] private AudioSource[] playerSounds;
    [SerializeField] private AudioSource[] BGMSounds;

    [SerializeField] private AudioSource[] towerASounds;
    [SerializeField] private AudioSource[] towerBSounds;
    [SerializeField] private AudioSource[] towerCSounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void PlaySound(AudioSource audioSource, Vector3 soundPosition, float maxDistance)
    {
        float distance = Vector3.Distance(PlayerController.instance.transform.position, soundPosition);
        float volume = Mathf.Clamp01(1 - (distance / maxDistance));
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void PlayEnemySoldierSounds(int index, Vector3 soundPosition, float maxDistance = 30f)
    {
        PlaySound(enemySoldierSounds[index], soundPosition, maxDistance);
    }

    public void PlayTowerASounds(int index, Vector3 soundPosition, float maxDistance = 30f)
    {
        PlaySound(towerASounds[index], soundPosition, maxDistance);
    }
}
