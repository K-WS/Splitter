﻿using UnityEngine;

[CreateAssetMenu(fileName = "HealthStatusData",menuName = "StatusObjects/Health", order = 1)]
public class CharacterStatus : ScriptableObject
{
    public GameObject characterGO; //Should contain battle animations
    public string  charName = "name";
    public float[] position = new float[3];
    public int level = 1;
    public int maxHealth = 100;
    public int currHealth = 100;
    public int maxEnergy = 100;
    public int currEnergy = 100;

    
}