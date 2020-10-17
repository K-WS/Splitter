using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthStatusData",menuName = "StatusObjects/Health", order = 1)]
public class CharacterStatus : ScriptableObject
{
    public GameObject characterGO; //Should contain battle animations
    public string  charName = "name";
    public float[] position = new float[3];

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
    /// Optional stats that can be affected during battle
    /// </summary>
    
    //public int currATK = 20;
    //public int currDEF = 10;
    public int currSPD = 25;


    /// <summary>
    /// Buffs and debuffs, currently FLAT, but beware going negative.
    /// </summary>
    public int boostATK = 0; //More or less attack
    public int boostDEF = 0; //More or less defense
    public int boostSPD = 0; //Haste effects (higher turn rate/speed)


    public void resetStats()
    {
        //currATK = attack;
        //currDEF = defense;
        currSPD = speed;

        boostATK = 0;
        boostDEF = 0;
        boostSPD = 0;
    }

    public void BufferUP()
    {
        currSPD += speed + boostSPD;
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
