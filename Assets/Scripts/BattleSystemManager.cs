using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class BattleSystemManager : MonoBehaviour
{
    public enum BattleState { START, P1TURN, P2TURN, P3TURN, E1TURN, E2TURN, E3TURN, WIN, LOST }

    private GameObject enemy1; //FOR ANIMATION, if the enemy brings more allies, he should have them attached to get
    private GameObject enemy2; //and attach to this
    private GameObject enemy3; //and this
    private GameObject player1; //FOR ANIM; I think the player should hold all other character gameobjects in it too?
    private GameObject player2; //Same idea here
    private GameObject player3; //and here

    //Positions to place characters in when in battle
    public Transform E1BattlePos;
    public Transform E2BattlePos;
    public Transform E3BattlePos;
    public Transform P1BattlePos;
    public Transform P2BattlePos;
    public Transform P3BattlePos;

    public CharacterStatus P1Status;
    private CharacterStatus P2Status;
    private CharacterStatus P3Status;
    //How to fetch statuses for split characters? And changing max hp?
    //public CharacterStatus P2Status;
    //public CharacterStatus P3Status;
    public CharacterStatus E1Status;
    private CharacterStatus E2Status;
    private CharacterStatus E3Status;


    public StatusHUD playerStatusHUD;
    public StatusHUD enemyStatusHUD;
    public RectTransform choices;

    private BattleState battleState;

    private bool hasClicked = true;



    private void Start()
    {
        battleState = BattleState.START;
        StartCoroutine(BeginBattle());
    }

    IEnumerator BeginBattle()
    {
        // spawn main characters on the platforms
        Debug.Log(P1Status);
        Debug.Log(E1Status);
        Debug.Log(P1Status.characterGO);
        Debug.Log(E1Status.characterGO);

        //enemy1 = Instantiate(E1Status.characterGO, E1BattlePos); enemy1.SetActive(true);
        //player1 = Instantiate(P1Status.characterGO.transform.GetChild(0).gameObject, P1BattlePos); player1.SetActive(true);

        // make the characters models invisible in the beginning ?
        //enemy1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);
        //player1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);

        // set the characters stats in HUD displays
        playerStatusHUD.SetStatusHUD(P1Status);
        enemyStatusHUD.SetStatusHUD(E1Status);

        //yield return new WaitForSeconds(0.2f);

        // fade in our characters sprites
        //yield return StartCoroutine(FadeInOpponents());

        yield return new WaitForSeconds(0.4f);

        // player turn!
        //Modify later to be affected by speed status
        battleState = BattleState.P1TURN;

        // let player select his action now!    
        yield return StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        // probably display some message 
        // stating it's player's turn here
        yield return new WaitForSeconds(0.5f);

        choices.gameObject.SetActive(true);


        // release the blockade on clicking 
        // so that player can click on 'attack' button    
        hasClicked = false;
    }

    public void OnAttackButtonPress()
    {
        // don't allow player to click on 'attack'
        // button if it's not his turn!
        if (battleState != BattleState.P1TURN)
            return;

        // allow only a single action per turn
        if (!hasClicked)
        {
            StartCoroutine(PlayerAttack());

            // block user from repeatedly 
            // pressing attack button  
            hasClicked = true;
        }
    }

    IEnumerator PlayerAttack()
    {
        choices.gameObject.SetActive(false);
        // trigger the execution of attack ANIMATION
        // in 'BattlePresence' animator

        //player1.GetComponent<Animator>().SetTrigger("Attack");


        //yield return new WaitForSeconds(1f);

        // decrease enemy health by a fixed
        // amount of 10. You probably want to have some
        // more complex logic here.
        enemyStatusHUD.SetHP(E1Status, 10);

        yield return new WaitForSeconds(1f);

        if (E1Status.currHealth <= 0)
        {
            // if the enemy health drops to 0 
            // we won!
            battleState = BattleState.WIN;
            yield return StartCoroutine(EndBattle());
        }
        else
        {
            // if the enemy health is still
            // above 0 when the turn finishes
            // it's enemy's turn!
            battleState = BattleState.E1TURN;
            yield return StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.5f);
        // as before, decrease playerhealth by a fixed
        // amount of 10. You probably want to have some
        // more complex logic here.
        playerStatusHUD.SetHP(P1Status, 10);

        // play attack ANIMATION by triggering
        // it inside the enemy animator
        
        //enemy1.GetComponent<Animator>().SetTrigger("Attack");

        yield return new WaitForSeconds(1);

        if (P1Status.currHealth <= 0)
        {
            // if the player health drops to 0 
            // we have lost the battle...
            battleState = BattleState.LOST;
            yield return StartCoroutine(EndBattle());
        }
        else
        {
            // if the player health is still
            // above 0 when the turn finishes
            // it's our turn again!
            battleState = BattleState.P1TURN;
            yield return StartCoroutine(PlayerTurn());
        }
    }

    IEnumerator EndBattle()
    {
        // check if we won
        if (battleState == BattleState.WIN)
        {
            // you may wish to display some kind
            // of message or play a victory fanfare
            // here
            yield return new WaitForSeconds(1);
            LevelLoader.instance.LoadLevel("Overworld");
        }
        // otherwise check if we lost
        // You probably want to display some kind of
        // 'Game Over' screen to communicate to the 
        // player that the game is lost
        else if (battleState == BattleState.LOST)
        {
            // you may wish to display some kind
            // of message or play a sad tune here!
            yield return new WaitForSeconds(1);

            LevelLoader.instance.LoadLevel("Overworld");
        }
    }




}
