using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChar : MonoBehaviour
{
    [SerializeField]
    private float movSpeed = 1;
    //private bool isAttacked = false;

    public CharacterStatus playerStatus;
    public CharacterStatus enemyStatus;

    public void Move(Vector3 dir)
    {
        transform.position += dir * Time.deltaTime * movSpeed;
    }

    //Note, reacts to grounds and walls as well, keep this in mind
    /*private void OnCollisionEnter(Collision collision)
    {
       //Debug.Log($"Collided with {collision.gameObject}");
       if(collision.gameObject.tag == "Enemy" && !isAttacked)
        {
            isAttacked = true;
            setBattleData(collision);
            //LevelLoader.instance.LoadLevel("Battle");
            
            Debug.Log("Trigger Battle Mode");
        }
    }*/

    /*private void setBattleData(Collision collision)
    {
        // Player Data 
        playerStatus.position[0] = this.transform.position.x;
        playerStatus.position[1] = this.transform.position.y;
        playerStatus.position[2] = this.transform.position.z;

        // Enemy Data
        CharacterStatus status = collision.gameObject.GetComponent<EnemyStatus>().enemyStatus;
        enemyStatus.charName = status.charName;
        enemyStatus.characterGameObject = status.characterGameObject.transform.GetChild(0).gameObject;
        enemyStatus.health = status.health;
        enemyStatus.maxHealth = status.maxHealth;
        enemyStatus.mana = status.mana;
        enemyStatus.maxMana = status.maxMana;
    }*/
}
