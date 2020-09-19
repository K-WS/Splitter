using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    private bool isAttacked = false;

    CharacterStatus playerStatus;
    CharacterStatus enemyStatus; //Only updated when an enemy touches the player.

    //Note, reacts to grounds and walls as well, keep this in mind
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log($"Collided with {collision.gameObject}");
        if (other.gameObject.tag == "Enemy" && !isAttacked)
        {
            isAttacked = true;
            setBattleData(other);
            //LevelLoader.instance.LoadLevel("Battle");

            Debug.Log("Trigger Battle Mode");
        }
    }

    private void setBattleData(Collision collision)
    {
        // Player Data 
        playerStatus.position[0] = this.transform.position.x;
        playerStatus.position[1] = this.transform.position.y;
        playerStatus.position[2] = this.transform.position.z;

        // Enemy Data
        CharacterStatus status = collision.gameObject.GetComponent<EnemyStatus>().enemyStatus;
        enemyStatus.charName = status.charName;
        enemyStatus.characterGO = status.characterGO.transform.GetChild(0).gameObject;
        enemyStatus.currHealth = status.currHealth;
        enemyStatus.maxHealth = status.maxHealth;
        enemyStatus.currEnergy = status.currEnergy;
        enemyStatus.maxEnergy = status.maxEnergy;
    }
}
