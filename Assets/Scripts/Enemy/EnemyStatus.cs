﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    public CharacterStatus enemyStatus;

   
    private void Awake()
    {
        enemyStatus.characterGO = gameObject;
        //DontDestroyOnLoad(this.gameObject);
    }
}
