using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;
   
    [SerializeField] private bool isArtfactBeingStole = false;
    [SerializeField] private bool isWaveAllCleared = false;
    [SerializeField] private Transform[] enemySpawnPositions;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWinUI;

    [SerializeField] public int enemyCount = 0;
    [SerializeField] private int enemyCountWin = 212;

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

    private void Start()
    {
        SetWaveCleared(false);
        gameOverUI.SetActive(false);
        gameWinUI.SetActive(false);
    }

    private void Update()
    {
        GameWinOver();

        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
            else if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
        }

        if (enemyCount >= enemyCountWin)
        {
            SetWaveCleared(true);
        }
    }

    public void SetWaveCleared(bool isTrue)
    {
        isWaveAllCleared = isTrue;
    }

    public void SetArtifactStoleStatus(bool isTrue)
    {
        isArtfactBeingStole = isTrue;
    }

    public void GameWinOver()
    {
        if (isArtfactBeingStole)
        {
            foreach (Transform enemySpawn in enemySpawnPositions)
            {
                if (Vector2.Distance(Artifact.instance.transform.position, enemySpawn.transform.position) < 0.03f)
                {
                    gameOverUI.SetActive(true);
                    gameWinUI.SetActive(false);
                    Invoke("RestartScene", 5f);
                }
            }
        }

        if (isWaveAllCleared)
        {
            gameWinUI.SetActive(true);
            gameOverUI.SetActive(false);
            Invoke("RestartScene", 5f);
        }

        if(PlayerController.instance.CharacterHealthComponent.CurrentHP <= 0)
        {
            gameOverUI.SetActive(true);
            gameWinUI.SetActive(false);
            Invoke("RestartScene", 5f);
        }

        if (Artifact.instance.CharacterHealthComponent.CurrentHP <= 0)
        {
            gameOverUI.SetActive(true);
            gameWinUI.SetActive(false);
            Invoke("RestartScene", 5f);
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public bool IsArtifactBeingStole()
    {
        return isArtfactBeingStole;
    }

    public bool IsArtifactDestroyed()
    {
        return false;
    }

    
}
