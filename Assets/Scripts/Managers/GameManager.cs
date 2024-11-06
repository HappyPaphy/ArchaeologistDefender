using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    public bool IsEnemyAllDead()
    {
        return false;
    }

    public bool IsArtifactBeingStole()
    {
        return false;
    }

    public bool IsArtifactDestroyed()
    {
        return false;
    }
}
