﻿using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthStatusData",menuName = "StatusObjects/Health", order = 1)]
public class CharacterStatus : ScriptableObject
{
    //TODO: A lot of code here is meant for the main character, but everyone has access to it...
    public GameObject characterGO; //Should contain battle animations
    public string   charName = "name";
    public string   fuseName = "name";
    public bool     isAlive = true;
    public float[]  position = new float[3];
    public int      placement = 0;

    /// <summary>
    /// Main stats that the player should see
    /// </summary>
    public int level = 1;
    public int maxHealth = 100;
    public int maxEnergy = 100;
    public int attack = 20; //How about a magic and magic defense too? At least, later...
    public int defense = 10;
    public int speed = 25;

    public float currHealth = 100;
    public float currEnergy = 100;

    /// <summary>
    /// Background stats that affect things during battle
    /// </summary>
    
    public int currSPD = 25; // Turn buffer
    public int HPSplit = 0; // How much HP main character can provide, ~30%


    /// <summary>
    /// Buffs and debuffs, currently FLAT, but beware going negative.
    /// </summary>
    public int boostATK = 0; //More/less attack
    public int boostDEF = 0; //More/less defense
    public int boostSPD = 0; //Haste effects (higher turn rate/speed)

    public int DoT = 0;
    public int DoT_Turns = 0;
    public int Regen = 0;
    public int Regen_Turns = 0;


    public void resetStats()
    {
        currSPD = speed;

        boostATK = 0;
        boostDEF = 0;
        boostSPD = 0;
        DoT = 0;
        Regen = 0;
        DoT_Turns = 0;
        Regen_Turns = 0;
        isAlive = true;

    HPSplit = Mathf.FloorToInt(maxHealth / 100f * 30f);
    }

    public void BufferUP()
    {
        currSPD += speed + boostSPD;
    }

    public void SplitFromMain(CharacterStatus status)
    {
        maxHealth -= HPSplit;
        currHealth -= HPSplit;
        status.maxHealth = HPSplit;
        status.currHealth = HPSplit;
    }

    public void ReturnToMain(CharacterStatus status)
    {
        maxHealth += status.maxHealth;
        currHealth += status.currHealth;
    }

    public void ResetHP()
    {
        //Warning: This is intended for enemies in battle, MC outside of it.
        currHealth = maxHealth;
    }
    public void ResetEN()
    {
        currEnergy = maxEnergy;
    }

    public int updateStatuses()
    {
        int totalDamage = 0;
        //Damge damage and heal DoT's, update timers
        if(DoT_Turns > 0)
        {
            totalDamage += DoT;
            DoT_Turns -= 1;
        }
        else
            DoT = 0;
        if (Regen_Turns > 0)
        {
            totalDamage -= Regen;
            Regen_Turns -= 1;
        }
        else
            Regen = 0;

        //Update Boosts
        if (boostATK > 0)
            boostATK = Mathf.Max(0, boostATK - 5);
        else if (boostATK < 0)
            boostATK = Mathf.Min(boostATK + 5, 0);

        if (boostDEF > 0)
            boostDEF = Mathf.Max(0, boostDEF - 5);
        else if (boostDEF < 0)
            boostDEF = Mathf.Min(boostDEF + 5, 0);

        if (boostSPD > 0)
            boostSPD = Mathf.Max(0, boostSPD - 5);
        else if (boostSPD < 0)
            boostSPD = Mathf.Min(boostSPD + 5, 0);

        //Return total tally of DoT's for statusHUD to update
        return totalDamage;
    }


    /*
    Buffs and harms?
    Regen/Poison - Get/Lose small amount of health at start or end of each turn?
                   Needs special check in case of death

    DMG/DEF buffs- Simple enough, already integrated, just change the boost values (BUT NOT TOO MUCH)



    Self-guide on how to perform...
    Attack/Defense

    Defense shouldn't fully be able to nullify attacks, so lets say that if ATK==DEF, then damage is halved.
    For now I can't think clearly enough to make this properly, so...
    
    damage = Math.Ceil( accuracy * ( (ATK*Random(0.8f,1)) / Math.Ceil(0.2f*DEF) ) )


    Speed
    The classic method is that speed stat also becomes higher, like others, but I can't directly
    use this to see who goes next though.

    uhh... some kind of buffer value...
    OH, THE FIRST ONE BUT COMBO IT LIKE THIS, WHERE SMALLEST STILL SUBTRACTS

    So, take the LOWEST ELIGIBLE SPEED, turns go one full round, they all get subtracted by that.
    If no-one is eligible, then add their buffer their speed+boostSPD

    I should make some kind of list that stores enums in that case to know who goes next

    25, 20, 15
     1,  2,  3
    10,  5,  0

    35, 25, 15
     1,  2,  3
    20, 10,  0

    45, 30, 15
     1,  2,  3
    30, 15,  0
     1,  -,  -
     5, 15,  0


     */


}
