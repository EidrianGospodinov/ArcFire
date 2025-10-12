using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    [SerializeField] int enemies = 5;
    [SerializeField] Text enemiesText;

    private void Awake()
    {
        enemiesText.text = enemies.ToString();
        Enemy.OnEnemyKilled += OnEnemyKilledAction;
    }

    void OnEnemyKilledAction()
    {
        enemies--; 
        enemiesText.text = enemies.ToString();
    }

    private void OnDisable()
    {
        Enemy.OnEnemyKilled -= OnEnemyKilledAction;
    }
}
