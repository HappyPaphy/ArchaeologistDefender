using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private GameObject[] transparentTowers;
    [SerializeField] private SpriteRenderer spriteTower_Create;
    [SerializeField] private GameObject player;

    public static TowerPlacer instance;

    void Awake()
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

    void Update()
    {

    }

    private Vector3 UpdateMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void CreateTower(int towerIndex)
    {
        GameObject tower = Instantiate(transparentTowers[towerIndex]);
        tower.GetComponent<Tower>().currentState = Tower.TowerState.Creating;
        //tower.GetComponent<SpriteRenderer>().sprite = towerSprites[towerIndex];
        tower.transform.position = UpdateMousePosition();
    }
}
