using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;

public class BattleSystemManager : MonoBehaviour
{
    public enum BattleState { START, P1TURN, P2TURN, P3TURN, E1TURN, E2TURN, E3TURN, WIN, LOST }

    /*private GameObject player1; //FOR ANIM; I think the player should hold all other character gameobjects in it too?
    private GameObject player2; //Same idea here
    private GameObject player3; //and here
    private GameObject enemy1; //FOR ANIMATION, if the enemy brings more allies, he should have them attached to get
    private GameObject enemy2; //and attach to this
    private GameObject enemy3; //and this
    */

    //For now, since my animations don't work, I'll just have references to the debug models to set active
    public GameObject player1Debug;
    public GameObject player2Debug;
    public GameObject player3Debug;
    public GameObject enemy1Debug;
    public GameObject enemy2Debug;
    public GameObject enemy3Debug;

    public Material p1DebugMat;
    public Material p2DebugMat;
    public Material p3DebugMat;

    //Positions to place characters in when in battle
    public Transform P1BattlePos;
    public Transform P2BattlePos;
    public Transform P3BattlePos;
    public Transform E1BattlePos;
    public Transform E2BattlePos;
    public Transform E3BattlePos;
    

    public CharacterStatus P1Status;
    private CharacterStatus P2Status = null;
    private CharacterStatus P3Status = null;
    public CharacterStatus E1Status;
    private CharacterStatus E2Status = null;
    private CharacterStatus E3Status = null;

    //MUST UPDATE LATER TO GET CORRECT FIELDS
    public StatusHUD playerStatusHUD;
    public StatusHUD enemyStatusHUD;
    public RectTransform choicesPanel;

    private BattleState battleState;

    //private bool hasClicked = true;

    private Queue<BattleState> turnQueue;

    //This would be better as a hashset... but how to give it characters?
    public List<CharacterStatus> splittableChars;
    public RectTransform splitPanel;



    private void Start()
    {
        battleState = BattleState.START;
        turnQueue = new Queue<BattleState>();
        StartCoroutine(BeginBattle());
    }

    /*
     ---------------------------------------------------------
     ----------------------Battle Start-----------------------
     ---------------------------------------------------------
     */

    IEnumerator BeginBattle()
    {
        // Spawn main characters on the platforms
        //Debug.Log(P1Status);
        //Debug.Log(E1Status);
        //Debug.Log(P1Status.characterGO);
        //Debug.Log(E1Status.characterGO);
        //enemy1 = Instantiate(E1Status.characterGO, E1BattlePos); enemy1.SetActive(true);
        //player1 = Instantiate(P1Status.characterGO.transform.GetChild(0).gameObject, P1BattlePos); player1.SetActive(true);
        // make the characters models invisible in the beginning ?
        //enemy1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);
        //player1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);

        //Call stat resets 
        P1Status.resetStats();
        E1Status.resetStats();
        E1Status.ResetHP();

        // set the characters stats in HUD displays
        playerStatusHUD.SetStatusHUD(1, P1Status);
        enemyStatusHUD.SetStatusHUD(1, E1Status);

        //In case enemies don't split, start with multiple, I need to make a check and reset them too.

        //This time is essentially like a pause before starting fadein, might be necessary, might not.
        //yield return new WaitForSeconds(0.2f);
        // fade in our characters sprites
        //yield return StartCoroutine(FadeInOpponents());
        yield return new WaitForSeconds(0.4f);

        //Start checking who should go next
        yield return WhoGoesNext();
    }

    /*
     ---------------------------------------------------------
     ---------------------Turn Assignment---------------------
     ---------------------------------------------------------
     */

    IEnumerator P1Turn()
    {
        Debug.Log(battleState);
        yield return new WaitForSeconds(0.5f);

        //Set panel contents
        SetPanel(true, true, false);
    }

    IEnumerator P2Turn()
    {
        Debug.Log(battleState);
        yield return new WaitForSeconds(0.5f);

        SetPanel(false, false, true);
    }

    IEnumerator P3Turn()
    {
        Debug.Log(battleState);
        yield return new WaitForSeconds(0.5f);

        SetPanel(false, false, true);
    }

    private void SetPanel(bool fuse, bool split, bool returner)
    {
        choicesPanel.gameObject.SetActive(true);
        choicesPanel.GetChild(2).gameObject.SetActive(fuse);     //Fuse   Button
        choicesPanel.GetChild(3).gameObject.SetActive(split);    //Split  Button
        choicesPanel.GetChild(4).gameObject.SetActive(returner); //Return Button
        choicesPanel.GetChild(5).gameObject.SetActive(false);    //Split  Panel, always default false
    }

    IEnumerator E1Turn()
    {
        Debug.Log(battleState);
        yield return new WaitForSeconds(0.5f);

        (CharacterStatus, int) toAttack = EChooseTarget();
        yield return StartCoroutine(EnemyDoesAttack(enemy1Debug, E1Status, toAttack.Item1, toAttack.Item2));
    }

    IEnumerator E2Turn()
    {
        //For now I'm not working on multiple enemies, so just default to E1 in case of a mistake
        yield return StartCoroutine(E1Turn());
    }
    IEnumerator E3Turn()
    {
        yield return StartCoroutine(E1Turn());
    }



    private (CharacterStatus, int who) EChooseTarget()
    {
        List<CharacterStatus> activePlayers = new List<CharacterStatus>();
        activePlayers.Add(P1Status);
        if (P2Status != null)
            activePlayers.Add(P2Status);
        if (P3Status != null)
            activePlayers.Add(P3Status);


        //TODO: MAKE IT ACTUALLY PICK RANDOM, I THINK I MIGHT HAVE TO PUT "who" IN THE SCRIPTABLEOBJECT
        //TODO: BECAUSE IF P2 IS MISSING, HOW WILL P3 GET THE CORRECT "who" VALUE?
        return (activePlayers[0], 1);
    }


    /*
     ---------------------------------------------------------
     -------------------Attack Button Phase-------------------
     ---------------------------------------------------------
     */
    public void OnAttackButtonPress()
    {
        //Disable ability to select any other move
        choicesPanel.gameObject.SetActive(false);

        switch (battleState)
        {
            case BattleState.P1TURN:
                StartCoroutine(P1ATK());
                break;

            case BattleState.P2TURN:
                StartCoroutine(P2ATK());
                break;

            case BattleState.P3TURN:
                StartCoroutine(P3ATK());
                break;

        }
    }

    IEnumerator P1ATK()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayerDoesAttack(player1Debug, P1Status, E1Status, 1));
    }

    IEnumerator P2ATK()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayerDoesAttack(player2Debug, P2Status, E1Status, 1));
    }

    IEnumerator P3ATK()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayerDoesAttack(player3Debug, P3Status, E1Status, 1));
    }

    public void OnEnergyButtonPress()
    {
        //Should give access to options on what abilities to use
    }

    public void OnFuseButtonPress()
    {
        //Infuses main player with a splittable
    }


    /*
     ---------------------------------------------------------
     --------------------Split and Combine--------------------
     ---------------------------------------------------------
     */
    public void OnSplitButtonPress()
    {

        // 1. Check if the player is allowed to split (enough HP + free slot)
        if (P1Status.currHealth > P1Status.HPSplit && (P2Status == null || P3Status == null)) 
        {
            splitPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("No free spaces or not enough HP");
        }
    }

    public void OnSplitCheckButtonPress()
    {
        Debug.Log("Split Check Was pressed");
        string name = splitPanel.GetChild(0).GetComponent<TMP_InputField>().text.ToLower();
        string P2 = "";
        string P3 = "";
        if (P2Status != null) P2 = P2Status.charName.ToLower();
        if (P3Status != null) P3 = P3Status.charName.ToLower();

        Debug.Log(name);

        //Go through choices
        foreach (CharacterStatus chr in splittableChars)
        {
            string choiceName = chr.charName.ToLower();
            Debug.Log(choiceName);
            if(choiceName == name && choiceName != P2 && choiceName != P3)
            {
                if (P2Status == null)
                    StartCoroutine(PerformSplit(chr, 2));
                else
                    StartCoroutine(PerformSplit(chr, 3));
                break;
            }
        }
        
    }
    IEnumerator PerformSplit(CharacterStatus character, int who)
    {
        choicesPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        if (who == 2)
        {
            P2Status = character;
            P2Status.resetStats();
            P2Status.ResetEN();
            P1Status.SplitFromMain(P2Status);

            playerStatusHUD.setUIVisible(who, true);

            playerStatusHUD.SetStatusHUD(1, P1Status);
            playerStatusHUD.SetStatusHUD(who, P2Status);

            player2Debug.SetActive(true);
            p2DebugMat.color = debugSetColor(P2Status.charName.ToLower());
        }
            
        else //3 
        {
            P3Status = character;
            P3Status.resetStats();
            P1Status.SplitFromMain(P3Status);

            playerStatusHUD.setUIVisible(who, true);

            playerStatusHUD.SetStatusHUD(1, P1Status);
            playerStatusHUD.SetStatusHUD(who, P3Status);

            player3Debug.SetActive(true);
            p3DebugMat.color = debugSetColor(P3Status.charName.ToLower());
        }
            



        StartCoroutine(WhoGoesNext());
    }

    public Color debugSetColor(string name)
    {
        Color col = new Color();
        col.a = 1;
        switch (name)
        {
            case "fire":
                col.r = 1; col.g = 0.13f; col.b = 0;
                break;
            case "ice":
                col.r = 0; col.g = 1; col.b = 1;
                break;
            case "arcane":
                col.r = 0.73f; col.g = 0; col.b = 1;
                break;
            case "normal":
                col.r = 1; col.g = 1; col.b = 1;
                break;
        }

        return col;
    }

    public void OnSplitBackButtonPress()
    {
        splitPanel.gameObject.SetActive(false);
    }

    public void OnReturnButtonPress()
    {
        choicesPanel.gameObject.SetActive(false);
        
        //You need to check BattleState to know which character should recombine
        //Then recombine the HP and stuff before disabling the character
        switch (battleState)
        {
            case BattleState.P2TURN:
                StartCoroutine(Recombination(2));
                break;
            case BattleState.P3TURN:
                StartCoroutine(Recombination(3));
                break;

        }
    }

    IEnumerator Recombination(int who)
    {
        //TODO: Not an actual TODO, but note, YOU CAN'T PASS STATUSES, THEY WON'T UPDATE
        yield return new WaitForSeconds(0.5f);

        //Hide UI 
        playerStatusHUD.setUIVisible(who, false);

        //Disable the Debug model
        if (who == 2)
        {
            //Re-combine
            P1Status.ReturnToMain(P2Status);
            P2Status = null;
            player2Debug.SetActive(false);

            //Update Main UI
            playerStatusHUD.SetStatusHUD(1, P1Status);
        }
            
        else //3 
        {
            P1Status.ReturnToMain(P3Status);
            P3Status = null;
            player3Debug.SetActive(false);

            //Update Main UI
            playerStatusHUD.SetStatusHUD(1, P1Status);
        }
            


        //TODO: MAKE SURE TO CHECK IF THIS ACTUALLY NULLIFIES THE REAL STATUS
        //nullify
        

        yield return StartCoroutine(WhoGoesNext());
    }


    /*
     ---------------------------------------------------------
     -------------------Attack and Win check------------------
     ------------------------Coroutines-----------------------
     */
    IEnumerator PlayerDoesAttack(GameObject player, CharacterStatus PStatus, CharacterStatus target, int who)
    {
        // Trigger animation
        //player.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1f);

        float acc = 1; //Special Stat for Phase 2 of game idea, just put here to remember...
        int damage = Mathf.CeilToInt(acc *
                                    ((PStatus.attack + PStatus.boostATK) * UnityEngine.Random.Range(0.7f, 1f) /
                                       Mathf.Ceil(0.1f * target.defense)));

        //Debug.Log($"Initial attack with boost is {P1Status.attack + P1Status.boostATK}");
        //Debug.Log($"This is multiplied by random {UnityEngine.Random.Range(0.9f, 1f)}");
        //Debug.Log($"Enemy defense is {E1Status.defense}, with multiplication 0.1f its {0.1f * E1Status.defense}");
        //Debug.Log($"And finally divided with {Mathf.Ceil(0.1f * E1Status.defense)}");
        //Debug.Log(damage);

        enemyStatusHUD.LoseHP(who, target, damage);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsMainEnemyDead());
    }

    IEnumerator IsMainEnemyDead()
    {
        //This should later be changed as ALL enemies being 0, or that enemies get a defeated state
        if (E1Status.currHealth <= 0)
        {
            battleState = BattleState.WIN;
            yield return StartCoroutine(EndBattle());
        }
        else
        { yield return StartCoroutine(WhoGoesNext()); }
    }

    IEnumerator EnemyDoesAttack(GameObject enemy, CharacterStatus EStatus, CharacterStatus target, int who)
    {
        //TODO: Some place will eventually need a spot for enemies to pick a special attack based on chance?
        // Animate attack
        //enemy.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1);

        playerStatusHUD.LoseHP(who, target, 10);
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(IsPlayerDead());
    }

    IEnumerator IsPlayerDead()
    {
        //TODO: WARNING     
        //TODO: THERE IS CURRENTLY NO TRIGGER FOR SPLITTED CHARACTERS TO BE DEFEATED AND DEACTIVATED

        if (P1Status.currHealth <= 0)
        {
            battleState = BattleState.LOST;
            yield return StartCoroutine(EndBattle());
        }
        else
        {
            yield return StartCoroutine(WhoGoesNext());
        }
    }

    /*
     ---------------------------------------------------------
     ----------------------Battle Ender-----------------------
     ---------------------------------------------------------
     */
    IEnumerator EndBattle()
    {
        //RECOMBINE CHARACTERS
        if (P2Status != null)
            P1Status.ReturnToMain(P2Status);
        if (P3Status != null)
            P1Status.ReturnToMain(P3Status);

        if (battleState == BattleState.WIN)
        {
            // Show victory?
            yield return new WaitForSeconds(1);
            LevelLoader.instance.LoadLevel("Overworld");
        }
        else if (battleState == BattleState.LOST)
        {
            //Show a game over later instead
            yield return new WaitForSeconds(1);
            LevelLoader.instance.LoadLevel("Overworld");
        }
    }

    /*
     ---------------------------------------------------------
     -------------------Turn Order Manager--------------------
     ---------------------------------------------------------
     */
    IEnumerator WhoGoesNext()
    {
        //Debug.Log("Who Goes Next?");
        //0. Check if stack is empty, if not, then you know who should go next
        Debug.Log(turnQueue.Count);
        Debug.Log("P2Status is ");
        Debug.Log(P2Status);
        if (turnQueue.Count == 0)
        {
            //Debug.Log("Queue is empty");
            //1. Place non-null turn characters into list, to know which characters need BufferUp in worst case
            List<(int, int, BattleState, CharacterStatus)> speeds = new List<(int, int, BattleState, CharacterStatus)>();

            //if (P1Status.currSPD >= P1Status.speed + P1Status.boostSPD)
            speeds.Add((P1Status.currSPD, P1Status.speed + P1Status.boostSPD, BattleState.P1TURN, P1Status));

            if (P2Status != null)
                speeds.Add((P2Status.currSPD, P2Status.speed + P2Status.boostSPD, BattleState.P2TURN, P2Status));
            if (P3Status != null)
                speeds.Add((P3Status.currSPD, P3Status.speed + P3Status.boostSPD, BattleState.P3TURN, P3Status));
            if (E1Status != null)
                speeds.Add((E1Status.currSPD, E1Status.speed + E1Status.boostSPD, BattleState.E1TURN, E1Status));
            if (E2Status != null)
                speeds.Add((E2Status.currSPD, E2Status.speed + E2Status.boostSPD, BattleState.E2TURN, E2Status));
            if (E3Status != null)
                speeds.Add((E3Status.currSPD, E3Status.speed + E3Status.boostSPD, BattleState.E3TURN, E3Status));

            //Debug.Log("non-null Speeds Got Added");
            //2. Go through the list in reverse, remove ineligible ones from list into another list for BufferUp
            List<(int, int, BattleState, CharacterStatus)> clean = new List<(int, int, BattleState, CharacterStatus)>();

            for (int i = speeds.Count-1; i >=0; i--)
            {
                //if (speeds[i].Item1 >= speeds[i].Item2 && speeds[i].Item1 < lowestSpeed)
                if (speeds[i].Item1 < speeds[i].Item2)
                {
                    clean.Add(speeds[i]);
                    speeds.RemoveAt(i);
                }
            }

            //Debug.Log("Cleaning Done");

            //3. Check if there are any eligible characters, if not, send BufferUp for all non-null and finish
            if (speeds.Count != 0)
            {
                //Debug.Log("Someone is eligible!");
                //4. Sort based on current speed descending (fastest first).
                speeds = speeds.OrderByDescending(x => x.Item1).ToList();

                //Debug.Log("Speed ordered");

                //5. You now have the character order, get subtractable, add order and reduce buffer
                int reduce = speeds[speeds.Count-1].Item1;

                //Debug.Log("Reduce picked");

                foreach ((int, int, BattleState, CharacterStatus) arg in speeds)
                {
                    turnQueue.Enqueue(arg.Item3);
                    arg.Item4.currSPD -= reduce;
                }
                //Debug.Log("Enqueuement done");
            }
            else
            {
                //Debug.Log("Noone eligible");
                foreach ((int, int, BattleState, CharacterStatus) arg in clean)
                    arg.Item4.BufferUP();
            }

            //6. Run this command again to make sure if we need to do this again
            yield return StartCoroutine(WhoGoesNext());
        }
        else
        {
            yield return StartCoroutine(TheyGoNext());
        }
    }

    IEnumerator TheyGoNext()
    {
        //Debug.Log("They Go Next!");
        var val = turnQueue.Dequeue();

        switch (val)
        {
            case BattleState.P1TURN:
                battleState = BattleState.P1TURN;
                yield return StartCoroutine(P1Turn());
                break;
            case BattleState.P2TURN:
                battleState = BattleState.P2TURN;
                yield return StartCoroutine(P2Turn());
                break;
            case BattleState.P3TURN:
                battleState = BattleState.P3TURN;
                yield return StartCoroutine(P3Turn());
                break;
            case BattleState.E1TURN:
                battleState = BattleState.E1TURN;
                yield return StartCoroutine(E1Turn());
                break;
            case BattleState.E2TURN:
                battleState = BattleState.E2TURN;
                yield return StartCoroutine(E2Turn());
                break;
            case BattleState.E3TURN:
                battleState = BattleState.E3TURN;
                yield return StartCoroutine(E3Turn());
                break;
        }
    }


}
