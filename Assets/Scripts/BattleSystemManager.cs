using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System;
using System.Linq;

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
    public CharacterStatus E1Status;
    private CharacterStatus E2Status;
    private CharacterStatus E3Status;

    //MUST UPDATE LATER TO GET CORRECT FIELDS
    public StatusHUD playerStatusHUD;
    public StatusHUD enemyStatusHUD;
    public RectTransform choices;

    private BattleState battleState;

    //private bool hasClicked = true;

    private Queue<BattleState> turnQueue;

    //This would be better as a hashset... but how to give it characters?
    public List<CharacterStatus> splittableChars;



    private void Start()
    {
        battleState = BattleState.START;
        turnQueue = new Queue<BattleState>();
        StartCoroutine(BeginBattle());
    }

    IEnumerator BeginBattle()
    {
        //Debug.Log("Battle Start");
        // spawn main characters on the platforms
        //Debug.Log(P1Status);
        //Debug.Log(E1Status);
        //Debug.Log(P1Status.characterGO);
        //Debug.Log(E1Status.characterGO);

        //enemy1 = Instantiate(E1Status.characterGO, E1BattlePos); enemy1.SetActive(true);
        //player1 = Instantiate(P1Status.characterGO.transform.GetChild(0).gameObject, P1BattlePos); player1.SetActive(true);

        // make the characters models invisible in the beginning ?
        //enemy1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);
        //player1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);

        // set the characters stats in HUD displays
        playerStatusHUD.SetStatusHUD(1,P1Status);
        enemyStatusHUD.SetStatusHUD(1, E1Status);

        //Call stat resets 
        P1Status.resetStats();
        E1Status.resetStats();
        //In case enemies don't split, start with multiple, I need to make a check and reset them too.


        //This time is essentially like a pause before starting fadein, might be necessary, might not.
        //yield return new WaitForSeconds(0.2f);

        // fade in our characters sprites
        //yield return StartCoroutine(FadeInOpponents());

        yield return new WaitForSeconds(0.4f);


        //Start checking who should go next
        yield return WhoGoesNext();
    }

    IEnumerator P1Turn()
    {
        // Notify whose turn it is?
        //...
        yield return new WaitForSeconds(0.5f);

        //Set panel contents
        choices.gameObject.SetActive(true);
        SetPanel(true, true, false);

        // Let Player click? 
        //hasClicked = false;
    }

    IEnumerator P2Turn()
    {
        // Notify whose turn it is?
        //...
        yield return new WaitForSeconds(0.5f);

        choices.gameObject.SetActive(true);
        SetPanel(false, false, true);
    }

    IEnumerator P3Turn()
    {
        // Notify whose turn it is?
        //...
        yield return new WaitForSeconds(0.5f);

        choices.gameObject.SetActive(true);
        SetPanel(false, false, true);
    }

    private void SetPanel(bool fuse, bool split, bool returner)
    {
        //Debug.Log(choices.transform.childCount);
        choices.GetChild(2).gameObject.SetActive(fuse);     //Fuse   Button
        choices.GetChild(3).gameObject.SetActive(split);    //Split  Button
        choices.GetChild(4).gameObject.SetActive(returner); //Return Button
    }

    public void OnAttackButtonPress()
    {
        //CHECK WHOSE TURN IT IS?

        // don't allow player to click on 'attack'
        // button if it's not his turn!
        //if (battleState != BattleState.P1TURN)
        //    return;

        // allow only a single action per turn
        //if (!hasClicked)
        //{
        //StartCoroutine(PlayerAttack());
        StartCoroutine(P1ATK());

            // block user from repeatedly 
            // pressing attack button  
            //hasClicked = true;
        //}
    }

    public void OnEnergyButtonPress()
    {

    }

    public void OnFuseButtonPress()
    {

    }

    public void OnSplitButtonPress()
    {
        //Make sure to not allow splitting IF there are already 3 characters
        //Otherwise they should get a resetStats and their HP values should be modified along with
        //the main player
    }

    public void OnReturnButtonPress()
    {
        //You need to check BattleState to know which character should recombine
        //Then recombine the HP and stuff before disabling the character
    }

    IEnumerator P1ATK()
    {
        //Disable ability to select any other move
        choices.gameObject.SetActive(false);

        // trigger the execution of attack ANIMATION in 'BattlePresence' animator
        //player1.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1f);


        // Decrease enemy 1 health based on stats
        // damage = Math.Ceil(accuracy * ((ATK * Random(0.8f, 1)) / Math.Ceil(0.2f * DEF)))

        float acc = 1; //Special Stat for Phase 2 of game idea, just put here to remember...

        int damage = Mathf.CeilToInt(acc *
                                    ( (P1Status.attack + P1Status.boostATK) * UnityEngine.Random.Range(0.7f,1f) /
                                       Mathf.Ceil(0.1f * E1Status.defense)));

        Debug.Log($"Initial attack with boost is {P1Status.attack + P1Status.boostATK}");
        Debug.Log($"This is multiplied by random {UnityEngine.Random.Range(0.9f, 1f)}");
        Debug.Log($"Enemy defense is {E1Status.defense}, with multiplication 0.1f its {0.1f * E1Status.defense}");
        Debug.Log($"And finally divided with {Mathf.Ceil(0.1f * E1Status.defense)}");

        Debug.Log(damage);
        enemyStatusHUD.LoseHP(1, E1Status, damage);
        
        yield return new WaitForSeconds(1f);

        //This should later be changed as ALL enemies being 0, or that enemies get a defeated state
        if (E1Status.currHealth <= 0)
        {
            battleState = BattleState.WIN;
            yield return StartCoroutine(EndBattle());
        }
        else
        { yield return StartCoroutine(WhoGoesNext()); }

    }

    IEnumerator P2ATK()
    {
        choices.gameObject.SetActive(false);

        //player2.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1f);

        float acc = 1; //Special Stat for Phase 2 of game idea, just put here to remember...
        int damage = Mathf.CeilToInt(acc *
                                    ((P2Status.attack + P2Status.boostATK) * UnityEngine.Random.Range(0.9f, 1f) /
                                       Mathf.Ceil(0.2f * E1Status.defense)));
        enemyStatusHUD.LoseHP(1, E1Status, damage);

        yield return new WaitForSeconds(1f);

        //This should later be changed as ALL enemies being 0, or that enemies get a defeated state
        if (E1Status.currHealth <= 0)
        {
            battleState = BattleState.WIN;
            yield return StartCoroutine(EndBattle());
        }
        else
        { yield return StartCoroutine(WhoGoesNext()); }
    }

    IEnumerator P3ATK()
    {
        choices.gameObject.SetActive(false);

        //player3.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1f);

        float acc = 1; //Special Stat for Phase 2 of game idea, just put here to remember...
        int damage = Mathf.CeilToInt(acc *
                                    ((P3Status.attack + P3Status.boostATK) * UnityEngine.Random.Range(0.9f, 1f) /
                                       Mathf.Ceil(0.2f * E1Status.defense)));
        enemyStatusHUD.LoseHP(1, E1Status, damage);

        yield return new WaitForSeconds(1f);

        //This should later be changed as ALL enemies being 0, or that enemies get a defeated state
        if (E1Status.currHealth <= 0)
        {
            battleState = BattleState.WIN;
            yield return StartCoroutine(EndBattle());
        }
        else
        { yield return StartCoroutine(WhoGoesNext()); }
    }

    IEnumerator E1Turn()
    {
        yield return new WaitForSeconds(0.5f);
        
        // play attack ANIMATION by triggering it inside the enemy animator/ 
        //enemy1.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1);

        // as before, decrease playerhealth by a fixed
        // amount of 10. You probably want to have some
        // more complex logic here.
        playerStatusHUD.LoseHP(1, P1Status, 10);
        yield return new WaitForSeconds(1f);

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

    IEnumerator E2Turn()
    {
        //For now I'm not working on multiple enemies, so just default to E1 in case of a mistake
        yield return StartCoroutine(E1Turn());
    }
    IEnumerator E3Turn()
    {
        yield return StartCoroutine(E1Turn());
    }

    IEnumerator EndBattle()
    {
        if (battleState == BattleState.WIN)
        {
            //RECOMBINE CHARACTERS
            if (P2Status != null)
            {
                P1Status.maxHealth += P2Status.maxHealth;
                P1Status.currHealth += P2Status.currHealth;
            }
            if (P3Status != null)
            {
                P1Status.maxHealth += P3Status.maxHealth;
                P1Status.currHealth += P3Status.currHealth;
            }


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

    IEnumerator WhoGoesNext()
    {
        //Debug.Log("Who Goes Next?");
        //0. Check if stack is empty, if not, then you know who should go next

        if (turnQueue.Count == 0)
        {
            //Debug.Log("Queue is empty");
            //1. Place non-null turn characters into list, to know which characters need BufferUp in worst case
            List<(int, int, BattleState, CharacterStatus)> speeds = new List<(int, int, BattleState, CharacterStatus)>();

            //if (P1Status.currSPD >= P1Status.speed + P1Status.boostSPD)
            speeds.Add((P1Status.currSPD, P1Status.speed + P1Status.boostSPD, BattleState.P1TURN, P1Status));

            if (E1Status != null)
                speeds.Add((E1Status.currSPD, E1Status.speed + E1Status.boostSPD, BattleState.E1TURN, E1Status));
            if (P2Status != null)
                speeds.Add((P2Status.currSPD, P2Status.speed + P2Status.boostSPD, BattleState.P2TURN, P2Status));
            if (P3Status != null)
                speeds.Add((P3Status.currSPD, P3Status.speed + P3Status.boostSPD, BattleState.P3TURN, P3Status));
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

            //1. Get buffers
            /*(int, int, CharacterStatus)[] speeds = new (int, int, CharacterStatus)[6] { 
                (P1Status.currSPD, P1Status.speed + P1Status.boostSPD, P1Status), 
                (1000, 0, null),
                (1000, 0, null),
                (E1Status.currSPD,E1Status.speed + E1Status.boostSPD, E1Status), 
                (1000, 0, null),
                (1000, 0, null)};

            if (P2Status != null)
                speeds[1] = (P2Status.currSPD, P2Status.speed + P2Status.boostSPD, P2Status);
            if (P3Status != null)
                speeds[2] = (P3Status.currSPD, P3Status.speed + P3Status.boostSPD, P3Status);
            if (E2Status != null)
                speeds[4] = (E2Status.currSPD, E2Status.speed + E2Status.boostSPD, E2Status);
            if (E3Status != null)
                speeds[5] = (E3Status.currSPD, E3Status.speed + E3Status.boostSPD, E3Status);


            //2. find lowest ELIGIBLE buffer (currSPD >= speed+boostSPD)
            int idx = -1;
            int lowestSpeed = 1000;

            for(int i = 0; i < speeds.Length; i++)
            {
                if(speeds[i].Item1 >= speeds[i].Item2 && speeds[i].Item1 < lowestSpeed)
                {
                    idx = i;
                    lowestSpeed = speeds[i].Item1;
                }
            }


            //3.1 Lowest ELIGIBLE found, enqueue as many to queue as you can
            //    starting from highest currSpeed to lowest
            //    and subtract the lowest value from everyone
            if(idx != -1)
            {
                //TODO
            }
            //3.2 Not found, reset all
            else
            {
                foreach((int,int,CharacterStatus) choice in speeds)
                {
                    choice.Item3.BufferUP();
                }
            }

            //4. Run this command again to make sure if we need to do this again
            yield return StartCoroutine(WhoGoesNext());*/
        }
        else
        {
            yield return StartCoroutine(TheyGoNext());
        }

        /*
        //2. Find the lowest currSpeed and its index to know who goes next
        int minIndex = Array.IndexOf(speeds, speeds.Min());


        //3. Subtract the lowest currSpeed on everyone else, reset currSpeed to speed+boostSPD on chosen
        //   Should I do it directly here or have a function?

        //4. Give turn to chosen

        switch (minIndex)
        {
            case 0:
                battleState = BattleState.P1TURN;
                yield return StartCoroutine(P1Turn());
                break;
            case 1:
                battleState = BattleState.P2TURN;
                yield return StartCoroutine(P2Turn());
                break;
            case 2:
                battleState = BattleState.P3TURN;
                yield return StartCoroutine(P3Turn());
                break;
            case 3:
                battleState = BattleState.E1TURN;
                yield return StartCoroutine(E1Turn());
                break;
            case 4:
                battleState = BattleState.E2TURN;
                yield return StartCoroutine(E2Turn());
                break;
            case 5:
                battleState = BattleState.E3TURN;
                yield return StartCoroutine(E3Turn());
                break;

        }   */
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
