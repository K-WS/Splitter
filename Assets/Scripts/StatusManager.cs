using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    private bool isAttacked = false;

    public CharacterStatus playerStatus;
    public CharacterStatus enemyStatus; //Only updated when an enemy touches the player.

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        playerStatus.characterGO = gameObject.transform.GetChild(0).gameObject;
    }

    //Note, reacts to grounds and walls as well, keep this in mind
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Touched");
        //Debug.Log($"Collided with {collision.gameObject}");
        if (other.gameObject.tag == "Enemy" && !isAttacked)
        {
            isAttacked = true;
            //Debug.Log("Trigger Battle Mode");
            setBattleData(other);
            LevelLoader.instance.LoadLevel("Battle");

            
        }
    }

    private void setBattleData(Collision collision)
    {
        // Player Data 
        playerStatus.position[0] = gameObject.transform.position.x;
        playerStatus.position[1] = gameObject.transform.position.y;
        playerStatus.position[2] = gameObject.transform.position.z;

        // Enemy Data
        CharacterStatus status = collision.gameObject.GetComponent<EnemyStatus>().enemyStatus;
        enemyStatus.charName = status.charName;
        enemyStatus.characterGO = status.characterGO.transform.GetChild(0).gameObject;
        enemyStatus.currHealth = status.currHealth;
        enemyStatus.maxHealth = status.maxHealth;
        enemyStatus.currEnergy = status.currEnergy;
        enemyStatus.maxEnergy = status.maxEnergy;
        enemyStatus.speed = status.speed;
    }
}
