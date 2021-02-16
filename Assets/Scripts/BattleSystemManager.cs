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
    private List<GameObject> playerDebuggerList = new List<GameObject>();

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
    public CharacterStatus E1Status;

    private Dictionary<BattleState, CharacterStatus> statusDict = new Dictionary<BattleState, CharacterStatus>();

    public StatusHUD playerStatusHUD;
    public StatusHUD enemyStatusHUD;
    public RectTransform choicesPanel;

    private BattleState battleState;

    private Queue<BattleState> turnQueue;
    private int turn = 0;
    public TextMeshProUGUI turner;

    //This would be better as a hashset... but how to give it characters?
    public List<CharacterStatus> splittableChars;
    public RectTransform splitPanel;

    public RectTransform TurnQueuePanel;

    public GameObject flyingBall;
    public Material flyingBallColor;
    public GameObject flyingBall2;
    public Material flyingBall2Color;
    public GameObject flyingBall3;
    public Material flyingBall3Color;

    public Shader MainColor;
    public Shader IceColor;
    public Shader ArcaneColor;
    public Shader FireColor;
    public Shader EnemyColor;



    private void Start()
    {
        battleState = BattleState.START;
        turnQueue = new Queue<BattleState>();
        StartCoroutine(BeginBattle());

        statusDict.Add(BattleState.P1TURN, P1Status);
        statusDict.Add(BattleState.E1TURN, E1Status);
        statusDict.Add(BattleState.P2TURN, null);
        statusDict.Add(BattleState.P3TURN, null);
        statusDict.Add(BattleState.E2TURN, null);
        statusDict.Add(BattleState.E3TURN, null);

        playerDebuggerList.Add(player1Debug);
        playerDebuggerList.Add(player2Debug);
        playerDebuggerList.Add(player3Debug);

    }

    /*
     ---------------------------------------------------------
     ----------------------Battle Start-----------------------
     ---------------------------------------------------------
     */

    IEnumerator BeginBattle()
    {
        // Spawn main characters on the platforms
        //enemy1 = Instantiate(E1Status.characterGO, E1BattlePos); enemy1.SetActive(true);
        //player1 = Instantiate(P1Status.characterGO.transform.GetChild(0).gameObject, P1BattlePos); player1.SetActive(true);
        // make the characters models invisible in the beginning ?
        //enemy1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);
        //player1.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);

        //Call stat resets 
        P1Status.resetStats();
        P1Status.ResetHP(); //Only called for this project... in real game this should be disabled
        P1Status.ResetEN(); //Same here
        P1Status.fuseName = P1Status.charName;
        E1Status.resetStats();
        E1Status.ResetHP();
        E1Status.ResetEN();

        // set the characters stats in HUD displays
        playerStatusHUD.SetStatusHUD(0, P1Status);
        enemyStatusHUD.SetStatusHUD(0, E1Status);

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
        float DMG = statusDict[battleState].updateStatuses();

        if (statusDict[battleState].boostATK > 0)
            playerStatusHUD.Buff(statusDict[battleState].placement, true);
        else
            playerStatusHUD.Buff(statusDict[battleState].placement, false);

        yield return new WaitForSeconds(0.5f);

        //Set panel contents
        SetPanel(true, true, false);
    }

    IEnumerator P2Turn()
    {
        float DMG = statusDict[battleState].updateStatuses();

        if (statusDict[battleState].boostATK > 0)
            playerStatusHUD.Buff(statusDict[battleState].placement, true);
        else
            playerStatusHUD.Buff(statusDict[battleState].placement, false);

        yield return new WaitForSeconds(0.5f);

        SetPanel(false, false, true);
    }

    IEnumerator P3Turn()
    {
        float DMG = statusDict[battleState].updateStatuses();

        if (statusDict[battleState].boostATK > 0)
            playerStatusHUD.Buff(statusDict[battleState].placement, true);
        else
            playerStatusHUD.Buff(statusDict[battleState].placement, false);

        yield return new WaitForSeconds(0.5f);

        SetPanel(false, false, true);
    }

    private void SetPanel(bool fuse, bool split, bool returner)
    {
        choicesPanel.gameObject.SetActive(true);
        choicesPanel.GetChild(2).gameObject.SetActive(fuse);     //Fuse   Button
        choicesPanel.GetChild(3).gameObject.SetActive(split);    //Split  Button
        choicesPanel.GetChild(4).gameObject.SetActive(returner); //Return Button
        SetSidePanel(false, false, false, false, false, false, false);
    }

    private void SetSidePanel(bool split, bool main, bool ice, bool arcane, bool fire, bool fuse, bool heal)
    {
        choicesPanel.GetChild(5).gameObject.SetActive(split);    //Split  Panel, always default false
        choicesPanel.GetChild(6).gameObject.SetActive(main);     //Main   Char Special moves, same
        choicesPanel.GetChild(7).gameObject.SetActive(ice);      //Ice    Char Special moves, same
        choicesPanel.GetChild(8).gameObject.SetActive(arcane);   //Arcane Char Special moves, same
        choicesPanel.GetChild(9).gameObject.SetActive(fire);     //Fire   Char Special moves, same
        choicesPanel.GetChild(10).gameObject.SetActive(fuse);    //Fuse   Panel, always default false too
        choicesPanel.GetChild(11).gameObject.SetActive(heal);    //Heal   Panel, always default false too
    }

    IEnumerator E1Turn()
    {

        yield return new WaitForSeconds(0.5f); SetSidePanel(false, false, false, false, false, false, false);

        int DMG = statusDict[battleState].updateStatuses();
        if(DMG > 0)
        {
            enemyStatusHUD.LoseHP(0, statusDict[battleState], DMG, new Color(1, 0.7f, 0, 1));
            yield return new WaitForSeconds(0.6f);

            if (statusDict[battleState].currHealth <= 0)
            {
                yield return StartCoroutine(IsMainEnemyDead());
                yield break;
            }

        }

        if (statusDict[battleState].DoT_Turns > 0)
            enemyStatusHUD.Debuff(statusDict[battleState].placement, true);
        else
            enemyStatusHUD.Debuff(statusDict[battleState].placement, false);

        float chance = UnityEngine.Random.Range(0f, 1f);

        //Additional check to determine if the enemy can use a special move
        if ( (statusDict[BattleState.P2TURN] != null || statusDict[BattleState.P3TURN] != null)
            && statusDict[battleState].currEnergy >= 20 
            && chance <= 0.3f)
        {
            enemyStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 20);
            yield return StartCoroutine(EnemyDoesAttackAll(enemy1Debug, E1Status));
        }
        else
        {
            (CharacterStatus, int, GameObject) toAttack = EChooseTarget();
            yield return StartCoroutine(EnemyDoesAttack(enemy1Debug, E1Status, toAttack.Item3, toAttack.Item1, toAttack.Item2));
        }
        
    }

    IEnumerator E2Turn()
    {
        int DMG = statusDict[battleState].updateStatuses();
        //For now I'm not working on multiple enemies, so just default to E1 in case of a mistake
        yield return StartCoroutine(E1Turn());
    }
    IEnumerator E3Turn()
    {
        int DMG = statusDict[battleState].updateStatuses();
        yield return StartCoroutine(E1Turn());
    }



    private (CharacterStatus, int, GameObject) EChooseTarget()
    {
        List<CharacterStatus> activePlayers = new List<CharacterStatus>();
        activePlayers.Add(P1Status);
        if (statusDict[BattleState.P2TURN] != null)
            activePlayers.Add(statusDict[BattleState.P2TURN]);
        if (statusDict[BattleState.P3TURN] != null)
            activePlayers.Add(statusDict[BattleState.P3TURN]);

        int choice = UnityEngine.Random.Range(0, activePlayers.Count);

        //playerDebuggerList[statusDict[battleState].placement
        return (activePlayers[choice],
                activePlayers[choice].placement, 
                playerDebuggerList[activePlayers[choice].placement]);
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
        StartCoroutine(MoveOverSeconds(flyingBall,player1Debug.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayerDoesAttack(player1Debug, statusDict[battleState], E1Status, 0));
    }

    IEnumerator P2ATK()
    {
        StartCoroutine(MoveOverSeconds(flyingBall, player2Debug.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayerDoesAttack(player2Debug, statusDict[battleState], E1Status, 0));
    }

    IEnumerator P3ATK()
    {
        StartCoroutine(MoveOverSeconds(flyingBall, player3Debug.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayerDoesAttack(player3Debug, statusDict[battleState], E1Status, 0));
    }

    public void OnEnergyButtonPress()
    {
        switch(statusDict[battleState].fuseName)
        {
            case "normal":
                SetSidePanel(false, true, false, false, false, false, false);
                break;
            case "ice":
                SetSidePanel(false, false, true, false, false, false, false);
                break;
            case "arcane":
                SetSidePanel(false, false, false, true, false, false, false);
                break;
            case "fire":
                SetSidePanel(false, false, false, false, true, false, false);
                break;
        }
    }

    /*
     ---------------------------------------------------------
     -------------------------Fusing--------------------------
     ---------------------------------------------------------
     */

    public void OnFuseButtonPress()
    {
        //Infuses main player with a splittable, does not need special conditions to be opened
       SetSidePanel(false, false, false, false, false, true, false);
    }

    public void OnNormalFuseButtonPress() { OnFuseCheckButtonPress("normal"); }
    public void OnIceFuseButtonPress() { OnFuseCheckButtonPress("ice"); }
    public void OnArcaneFuseButtonPress() { OnFuseCheckButtonPress("arcane"); }
    public void OnFireFuseButtonPress() { OnFuseCheckButtonPress("fire"); }

    public void OnFuseCheckButtonPress(string name)
    {

        string P1 = statusDict[BattleState.P1TURN].fuseName;
        string P2 = "";
        string P3 = "";
        if (statusDict[BattleState.P2TURN] != null) P2 = statusDict[BattleState.P2TURN].charName;
        if (statusDict[BattleState.P3TURN] != null) P3 = statusDict[BattleState.P3TURN].charName;

        if (name != P1 && name != P2 && name != P3)
            StartCoroutine(PerformFuse(name));
    }

    IEnumerator PerformFuse(string name)
    {
        choicesPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        //TODO: Set fuse name and fuse color, Make sure that energy picking checks FuseName to decide
        //TODO: Which panel to open... attacking animations are going to be complicated.

        statusDict[BattleState.P1TURN].fuseName = name;
        p1DebugMat.color = debugSetColor(name);

        StartCoroutine(WhoGoesNext());
    }


    /*
     ---------------------------------------------------------
     --------------------Split and Combine--------------------
     ---------------------------------------------------------
     */
    public void OnSplitButtonPress()
    {

        // 1. Check if the player is allowed to split (enough HP + free slot)
        if (P1Status.currHealth > P1Status.HPSplit && (statusDict[BattleState.P2TURN] == null || statusDict[BattleState.P3TURN] == null)) 
        {
            SetSidePanel(true, false, false, false, false, false, false);
        }
        else
        {
            Debug.Log("No free spaces or not enough HP");
        }
    }
    public void OnIceSplitButtonPress()
    {
        OnSplitCheckButtonPress(splittableChars[0]);
    }
    public void OnArcaneSplitButtonPress()
    {
        OnSplitCheckButtonPress(splittableChars[1]);
    }
    public void OnFireSplitButtonPress()
    {
        OnSplitCheckButtonPress(splittableChars[2]);
    }

    private void OnSplitCheckButtonPress(CharacterStatus status)
    {
        string name = status.charName;

        string P1 = statusDict[BattleState.P1TURN].fuseName;
        string P2 = "";
        string P3 = "";
        if (statusDict[BattleState.P2TURN] != null) P2 = statusDict[BattleState.P2TURN].charName;
        if (statusDict[BattleState.P3TURN] != null) P3 = statusDict[BattleState.P3TURN].charName;

        if(name != P1 && name != P2 && name != P3)
        {
            if (statusDict[BattleState.P2TURN] == null)
                StartCoroutine(PerformSplit(status, 1));
            else
                StartCoroutine(PerformSplit(status, 2));
        }
    }

    IEnumerator PerformSplit(CharacterStatus character, int who)
    {
        choicesPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        if (who == 1)
        {
            statusDict[BattleState.P2TURN] = character;
            statusDict[BattleState.P2TURN].resetStats();
            statusDict[BattleState.P2TURN].ResetEN();
            statusDict[BattleState.P2TURN].currSPD = 0;
            statusDict[BattleState.P2TURN].placement = who;

            playerStatusHUD.setUIVisible(who, true);

            P1Status.SplitFromMain(statusDict[BattleState.P2TURN]);

            playerStatusHUD.SetStatusHUD(0, P1Status);
            playerStatusHUD.SetStatusHUD(who, statusDict[BattleState.P2TURN]);

            

            player2Debug.SetActive(true);
            p2DebugMat.color = debugSetColor(statusDict[BattleState.P2TURN].charName);
        }
            
        else //2
        {
            statusDict[BattleState.P3TURN] = character;
            statusDict[BattleState.P3TURN].resetStats();
            statusDict[BattleState.P3TURN].ResetEN();
            statusDict[BattleState.P3TURN].currSPD = 0;
            statusDict[BattleState.P3TURN].placement = who;

            playerStatusHUD.setUIVisible(who, true);

            P1Status.SplitFromMain(statusDict[BattleState.P3TURN]);

            playerStatusHUD.SetStatusHUD(0, P1Status);
            playerStatusHUD.SetStatusHUD(who, statusDict[BattleState.P3TURN]);

            

            player3Debug.SetActive(true);
            p3DebugMat.color = debugSetColor(statusDict[BattleState.P3TURN].charName);
        }
            
        StartCoroutine(WhoGoesNext());
    }

    public void OnReturnButtonPress()
    {
        choicesPanel.gameObject.SetActive(false);

        //You need to check BattleState to know which character should recombine
        //Then recombine the HP and stuff before disabling the character
        switch (battleState)
        {
            case BattleState.P2TURN:
                StartCoroutine(Recombination(1));
                break;
            case BattleState.P3TURN:
                StartCoroutine(Recombination(2));
                break;

        }
    }

    IEnumerator Recombination(int who)
    {
        yield return new WaitForSeconds(0.5f);

        //Hide UI 
        playerStatusHUD.setUIVisible(who, false);

        //Remove buffs/debuffs
        //TODO: This could be the spot to do buff passing and exponentiality
        playerStatusHUD.Buff(statusDict[battleState].placement, false);
        playerStatusHUD.Debuff(statusDict[battleState].placement, false);

        //Re-combine
        P1Status.ReturnToMain(statusDict[battleState]);
        statusDict[battleState] = null;

        //Disable the Debug model
        if (who == 1)
            player2Debug.SetActive(false);
        else //2
            player3Debug.SetActive(false);

        //Update Main UI
        playerStatusHUD.SetStatusHUD(0, P1Status);

        yield return StartCoroutine(WhoGoesNext());
    }

    public Color debugSetColor(string name)
    {
        Color col = new Color();
        col.a = 1;
        switch (name)
        {
            case "fire":
                col.r = 1f; col.g = 0.54f; col.b = 0;
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
            case "enemy":
                col.r = 1; col.g = 0; col.b = 0; col.a = 1;
                break;
            case "null":
                col.r = 0; col.g = 0; col.b = 0; col.a = 0;
                break;
        }

        return col;
    }

    public Shader debugSetShader(string name)
    {
        switch (name)
        {
            case "fire":
                return FireColor;
            case "ice":
                return IceColor;
            case "arcane":
                return ArcaneColor;
            case "normal":
                return MainColor;
            case "enemy":
                return EnemyColor;
            case "null":
                return MainColor;
        }

        return MainColor;
    }



    /*
     ---------------------------------------------------------
     -------------------Character Abilities-------------------
     ---------------------------------------------------------
     */
    //TODO: At the beginning of each of these, add an energy check to see if suitable
    public void EN_Main_StrongAttack()
    {
        if(statusDict[battleState].currEnergy >= 20)
        {
            playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 20);
            choicesPanel.gameObject.SetActive(false);
            StartCoroutine(STRATK(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], E1Status, 0));
        }

    }

    IEnumerator STRATK(GameObject player, CharacterStatus PStatus, CharacterStatus target, int who)
    {
        //player.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(MoveOverSeconds(flyingBall, player.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);

        float acc = 1;
        int damage = Mathf.CeilToInt(2*acc *
                                    ((PStatus.attack + PStatus.boostATK) * UnityEngine.Random.Range(0.7f, 1f) /
                                       Mathf.Ceil(0.1f * target.defense)));
        enemyStatusHUD.LoseHP(who, target, damage, Color.white);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsMainEnemyDead());
    }

    public void EN_Main_Recover()
    {
        // A double up ability that does a good heal on alive targets
        // while having a weaker heal and revive on defeated targets
        // NEEDS A PICKABLE TARGET
        if (statusDict[battleState].currEnergy >= 45)
        {
            SetSidePanel(false, true, false, false, false, false, true);
            Transform healChoices = choicesPanel.GetChild(11);
            //healChoices.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = debugSetColor(statusDict[battleState].fuseName);

            if (statusDict[BattleState.P2TURN] == null)
                healChoices.GetChild(1).gameObject.SetActive(false);
            else
            {
                healChoices.GetChild(1).gameObject.SetActive(true);
                healChoices.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = debugSetColor(statusDict[BattleState.P2TURN].fuseName);
            }

            if (statusDict[BattleState.P3TURN] == null)
                healChoices.GetChild(2).gameObject.SetActive(false);
            else
            {
                healChoices.GetChild(2).gameObject.SetActive(true);
                healChoices.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = debugSetColor(statusDict[BattleState.P3TURN].fuseName);
            }
        }
    }

    public void P1HealButton()
    {
        playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 45);
        choicesPanel.gameObject.SetActive(false);
        StartCoroutine(HealTarget(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], statusDict[BattleState.P1TURN], 0));
    }

    public void P2HealButton()
    {
        playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 45);
        choicesPanel.gameObject.SetActive(false);
        StartCoroutine(HealTarget(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], statusDict[BattleState.P2TURN], 1));
    }

    public void P3HealButton() 
    {
        playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 45);
        choicesPanel.gameObject.SetActive(false);
        StartCoroutine(HealTarget(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], statusDict[BattleState.P3TURN], 2));
    }

    IEnumerator HealTarget(GameObject player, CharacterStatus PStatus, CharacterStatus target, int who)
    {
        //player.GetComponent<Animator>().SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        int heal = 100;
        playerStatusHUD.GainHP(who, target, heal);

        if (!target.isAlive)
        {
            target.isAlive = true;
            if(who == 1)
            {
                player2Debug.transform.Rotate(new Vector3(-90, 0, 0));
                player2Debug.transform.position += new Vector3(0, 0.5f, 0);
            }
            if (who == 2)
            {
                player3Debug.transform.Rotate(new Vector3(-90, 0, 0));
                player3Debug.transform.position += new Vector3(0, 0.5f, 0);
            }


        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(WhoGoesNext());
    }
    public void EN_Ice_IceBall()
    {
        if (statusDict[battleState].currEnergy >= 15)
        {
            playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 15);
            choicesPanel.gameObject.SetActive(false);
            StartCoroutine(IceBall(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], E1Status, 0));
        }
    }

    IEnumerator IceBall(GameObject player, CharacterStatus PStatus, CharacterStatus target, int who)
    {
        StartCoroutine(MoveOverSeconds(flyingBall, player.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        //player.GetComponent<Animator>().SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        float acc = 1;
        int damage = Mathf.CeilToInt(1.3f * acc *
                                    ((PStatus.attack + PStatus.boostATK) * UnityEngine.Random.Range(0.7f, 1f) /
                                       Mathf.Ceil(0.1f * target.defense)));

        enemyStatusHUD.LoseHP(who, target, damage, Color.white);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsMainEnemyDead());
    }

    public void EN_Ice_Stagger()
    {
        if (statusDict[battleState].currEnergy >= 40)
        {
            playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 40);
            choicesPanel.gameObject.SetActive(false);
            StartCoroutine(Stagger(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], BattleState.E1TURN, 0));
        }
    }
    IEnumerator Stagger(GameObject player, CharacterStatus PStatus, BattleState target, int who)
    {
        //player.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(MoveOverSeconds(flyingBall, player.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);

        int i = 0;
        foreach(BattleState state in turnQueue.ToArray())
        {
            if (target == state)
            {
                for (int j = i+1; j < TurnQueuePanel.transform.childCount - 1; j++)
                {
                    TurnQueuePanel.transform.GetChild(j).GetComponent<UnityEngine.UI.Image>().color = TurnQueuePanel.transform.GetChild(j + 1).GetComponent<UnityEngine.UI.Image>().color;
                }
            }
            i++;
        }

        turnQueue = new Queue<BattleState>(turnQueue.Where(x => x != target));


        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsMainEnemyDead());
    }

        public void EN_Arcane_Bolt()
    {
        if (statusDict[battleState].currEnergy >= 15)
        {
            playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 15);
            choicesPanel.gameObject.SetActive(false);
            StartCoroutine(Bolt(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], E1Status, 0));
        }
    }

    IEnumerator Bolt(GameObject player, CharacterStatus PStatus, CharacterStatus target, int who)
    {
        //player.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(MoveOverSeconds(flyingBall, player.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);

        float acc = 1;
        int damage = Mathf.CeilToInt(1.4f * acc *
                                    ((PStatus.attack + PStatus.boostATK) * UnityEngine.Random.Range(0.7f, 1f) /
                                       Mathf.Ceil(0.1f * target.defense)));
        enemyStatusHUD.LoseHP(who, target, damage, Color.white);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsMainEnemyDead());
    }

    public void EN_Arcane_Power()
    {
        if (statusDict[battleState].currEnergy >= 40)
        {
            playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 40);
            choicesPanel.gameObject.SetActive(false);
            StartCoroutine(ATKUP(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState]));
        }
    }

    IEnumerator ATKUP(GameObject player, CharacterStatus PStatus)
    {
        //player.GetComponent<Animator>().SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        PStatus.boostATK = 20;
        playerStatusHUD.Buff(statusDict[battleState].placement, true);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(WhoGoesNext());
    }

    public void EN_Fire_Fireball()
    {
        if (statusDict[battleState].currEnergy >= 15)
        {
            playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 15);
            choicesPanel.gameObject.SetActive(false);
            StartCoroutine(Fireball(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], E1Status, 0));
        }
    }

    IEnumerator Fireball(GameObject player, CharacterStatus PStatus, CharacterStatus target, int who)
    {
        //player.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(MoveOverSeconds(flyingBall, player.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);

        float acc = 1;
        int damage = Mathf.CeilToInt(1.6f * acc *
                                    ((PStatus.attack + PStatus.boostATK) * UnityEngine.Random.Range(0.7f, 1f) /
                                       Mathf.Ceil(0.1f * target.defense)));
        enemyStatusHUD.LoseHP(who, target, damage, Color.white);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsMainEnemyDead());
    }

    public void EN_Fire_Flame()
    {
        if (statusDict[battleState].currEnergy >= 40)
        {
            playerStatusHUD.LoseEN(statusDict[battleState].placement, statusDict[battleState], 40);
            choicesPanel.gameObject.SetActive(false);
            StartCoroutine(Flame(playerDebuggerList[statusDict[battleState].placement], statusDict[battleState], E1Status, 0));
        }
    }
    //TODO: Note, Bug or Feature, but the enemy will only die after a player turn has passed when DoT takes HP to 0
    IEnumerator Flame(GameObject player, CharacterStatus PStatus, CharacterStatus target, int who)
    {
        //player.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(MoveOverSeconds(flyingBall, player.transform.position, enemy1Debug.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);

        target.DoT = 15;
        target.DoT_Turns = 3;

        enemyStatusHUD.Debuff(statusDict[BattleState.E1TURN].placement, true);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsMainEnemyDead());
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

        enemyStatusHUD.LoseHP(who, target, damage, Color.white);

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

    IEnumerator EnemyDoesAttack(GameObject enemy, CharacterStatus EStatus, GameObject player, CharacterStatus target, int who)
    {
        // Animate attack
        //enemy.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1);

        StartCoroutine(MoveOverSeconds(flyingBall, enemy.transform.position, player.transform.position, 0.5f, debugSetShader(statusDict[battleState].fuseName)));
        yield return new WaitForSeconds(0.5f);

        float acc = 1;
        int damage = Mathf.CeilToInt(1.5f * acc *
                                    ((EStatus.attack + EStatus.boostATK) * UnityEngine.Random.Range(0.7f, 1f) /
                                       Mathf.Ceil(0.1f * target.defense)));

        playerStatusHUD.LoseHP(who, target, damage, Color.white);
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(IsPlayerDead());
    }

    IEnumerator EnemyDoesAttackAll(GameObject enemy, CharacterStatus EStatus)
    {
        // Animate attack
        //enemy.GetComponent<Animator>().SetTrigger("Attack");
        //yield return new WaitForSeconds(1);

        List<CharacterStatus> activePlayers = new List<CharacterStatus>();
        activePlayers.Add(P1Status);
        if (statusDict[BattleState.P2TURN] != null)
            activePlayers.Add(statusDict[BattleState.P2TURN]);
        if (statusDict[BattleState.P3TURN] != null)
            activePlayers.Add(statusDict[BattleState.P3TURN]);

        List<GameObject> balls = new List<GameObject>();
        balls.Add(flyingBall);
        balls.Add(flyingBall2);
        balls.Add(flyingBall3);

        for (int i = 0; i < activePlayers.Count; i++)
        {
            StartCoroutine(MoveOverSeconds(balls[i], 
                enemy.transform.position,
                playerDebuggerList[activePlayers[i].placement].transform.position, 
                0.5f,
                debugSetShader(statusDict[battleState].fuseName)));
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < activePlayers.Count; i++)
        {
            float acc = 1;
            int damage = Mathf.CeilToInt(1.5f * acc *
                                        ((EStatus.attack + EStatus.boostATK) * UnityEngine.Random.Range(0.7f, 1f) /
                                            Mathf.Ceil(0.1f * activePlayers[i].defense)));

            playerStatusHUD.LoseHP(activePlayers[i].placement, activePlayers[i], damage, Color.white);
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(IsPlayerDead());
    }

    IEnumerator IsPlayerDead()
    {

        if(statusDict[BattleState.P2TURN] != null)
        {
            if (statusDict[BattleState.P2TURN].isAlive && statusDict[BattleState.P2TURN].currHealth <= 0)
            {
                statusDict[BattleState.P2TURN].isAlive = false;
                player2Debug.transform.Rotate(new Vector3(90, 0, 0));
                player2Debug.transform.position -= new Vector3(0, 0.5f, 0);

                //This shouldn't be necessary when you don't debug HP to 0
                playerStatusHUD.SetStatusHUD(1, statusDict[BattleState.P2TURN]);
            }
        }

        if (statusDict[BattleState.P3TURN] != null)
        {
            if (statusDict[BattleState.P3TURN].isAlive && statusDict[BattleState.P3TURN].currHealth <= 0)
            {
                statusDict[BattleState.P3TURN].isAlive = false;
                player3Debug.transform.Rotate(new Vector3(90, 0, 0));
                player3Debug.transform.position -= new Vector3(0, 0.5f, 0);

                //Here as well
                playerStatusHUD.SetStatusHUD(2, statusDict[BattleState.P3TURN]);
            }
        }

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
        if (statusDict[BattleState.P2TURN] != null)
            P1Status.ReturnToMain(statusDict[BattleState.P2TURN]);
        if (statusDict[BattleState.P3TURN] != null)
            P1Status.ReturnToMain(statusDict[BattleState.P3TURN]);

        //Turn Main back to normal as well
        statusDict[BattleState.P1TURN].fuseName = "normal";
        p1DebugMat.color = debugSetColor("normal");

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
        //0. Check if stack is empty, if not, then you know who should go next
        if (turnQueue.Count == 0)
        {
            //1. Place non-null turn characters into list, to know which characters need BufferUp in worst case
            List<(int, int, BattleState, CharacterStatus)> speeds = new List<(int, int, BattleState, CharacterStatus)>();

            //if (P1Status.currSPD >= P1Status.speed + P1Status.boostSPD)
            speeds.Add((statusDict[BattleState.P1TURN].currSPD, statusDict[BattleState.P1TURN].speed + statusDict[BattleState.P1TURN].boostSPD, BattleState.P1TURN, statusDict[BattleState.P1TURN]));

            if (statusDict[BattleState.P2TURN] != null && statusDict[BattleState.P2TURN].isAlive)
                speeds.Add((statusDict[BattleState.P2TURN].currSPD, statusDict[BattleState.P2TURN].speed + statusDict[BattleState.P2TURN].boostSPD, BattleState.P2TURN, statusDict[BattleState.P2TURN]));
            if (statusDict[BattleState.P3TURN] != null && statusDict[BattleState.P3TURN].isAlive)
                speeds.Add((statusDict[BattleState.P3TURN].currSPD, statusDict[BattleState.P3TURN].speed + statusDict[BattleState.P3TURN].boostSPD, BattleState.P3TURN, statusDict[BattleState.P3TURN]));
            if (statusDict[BattleState.E1TURN] != null)
                speeds.Add((statusDict[BattleState.E1TURN].currSPD, statusDict[BattleState.E1TURN].speed + statusDict[BattleState.E1TURN].boostSPD, BattleState.E1TURN, statusDict[BattleState.E1TURN]));
            if (statusDict[BattleState.E2TURN] != null && statusDict[BattleState.E2TURN].isAlive)
                speeds.Add((statusDict[BattleState.E2TURN].currSPD, statusDict[BattleState.E2TURN].speed + statusDict[BattleState.E2TURN].boostSPD, BattleState.E2TURN, statusDict[BattleState.E2TURN]));
            if (statusDict[BattleState.E3TURN] != null && statusDict[BattleState.E3TURN].isAlive)
                speeds.Add((statusDict[BattleState.E3TURN].currSPD, statusDict[BattleState.E3TURN].speed + statusDict[BattleState.E3TURN].boostSPD, BattleState.E3TURN, statusDict[BattleState.E3TURN]));

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

            //3. Check if there are any eligible characters, if not, send BufferUp for all non-null and finish
            if (speeds.Count != 0)
            {
                //4. Sort based on current speed descending (fastest first).
                speeds = speeds.OrderByDescending(x => x.Item1).ToList();

                //5. You now have the character order, get subtractable, add order and reduce buffer
                int reduce = speeds[speeds.Count-1].Item1;

                TurnQueuePanel.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = debugSetColor("null");
                int order = 1;
                foreach ((int, int, BattleState, CharacterStatus) arg in speeds)
                {
                    turnQueue.Enqueue(arg.Item3);
                    arg.Item4.currSPD -= reduce;

                    //Mark turn queue coloration as well, WARNING THAT IT WILL BREAK WITH 6 RIGHT NOW
                    TurnQueuePanel.transform.GetChild(order).GetComponent<UnityEngine.UI.Image>().color = debugSetColor(arg.Item4.fuseName);
                    order++;

                }
                turn += 1;
                turner.text = $"Turn {turn}";

            }
            else
            {
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
        var val = turnQueue.Dequeue();

        for (int i = 0; i < TurnQueuePanel.transform.childCount-1; i++) 
        {
            TurnQueuePanel.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().color = TurnQueuePanel.transform.GetChild(i+1).GetComponent<UnityEngine.UI.Image>().color;
        }

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

    //Special "Animation" for moving a ball over
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 start, Vector3 end, float seconds, Shader shade)
    {
        //flyingBallColor.SetColor("albedo", col);
        //flyingBallColor.color = col;
        flyingBallColor.shader = shade;
        objectToMove.SetActive(true);
        objectToMove.transform.position = start;

        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
        objectToMove.SetActive(false);
    }


}
