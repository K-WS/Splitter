using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour
{
    public GameObject P1HP;
    public GameObject P1EN;

    public GameObject P2HP;
    public GameObject P2EN;

    public GameObject P3HP;
    public GameObject P3EN;

    public TextMeshProUGUI P1_UI_DMG;
    public TextMeshProUGUI P2_UI_DMG;
    public TextMeshProUGUI P3_UI_DMG;

    public Image P1_Buff;
    public Image P2_Buff;
    public Image P3_Buff;

    public Image P1_Debuff;
    public Image P2_Debuff;
    public Image P3_Debuff;


    /// <summary>
    /// Initializer to automatically set the HP values cased on CharacterStatus ScriptableObject given...
    /// and the integer to know which charcter...
    /// On split characters, call this after their ScriptableObject HP vals updated
    /// </summary>
    public void SetStatusHUD(int who, CharacterStatus status)
    {
        switch (who)
        {
            case 0:
                setCharHUD(status, P1HP, P1EN);
                break;
            case 1:
                setCharHUD(status, P2HP, P2EN);
                break;
            case 2:
                setCharHUD(status, P3HP, P3EN);
                break;
            default:
                Debug.LogError("A correct 'who' value was not provided");
                break;
        }
        P1_UI_DMG.enabled = false;
    }

    private void setCharHUD(CharacterStatus status, GameObject HP, GameObject EN)
    {

        Image HPBar = HP.transform.GetChild(0).gameObject.GetComponent<Image>();
        TextMeshProUGUI HPVal = HP.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        Image ENBar = EN.transform.GetChild(0).gameObject.GetComponent<Image>();
        TextMeshProUGUI ENVal = EN.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        float curHP = status.currHealth * (100f / status.maxHealth);
        float curEN = status.currEnergy * (100f / status.maxEnergy);

        HPBar.fillAmount = curHP / 100f;
        HPVal.SetText($"{status.currHealth}/{status.maxHealth}");

        ENBar.fillAmount = curEN / 100f;
        ENVal.SetText($"{status.currEnergy}/{status.maxEnergy}");
    }

    public void setUIVisible(int who, bool visible)
    {
        switch (who)
        {
            case 0:
                P1HP.SetActive(visible);
                P1EN.SetActive(visible);
                break;
            case 1:
                P2HP.SetActive(visible);
                P2EN.SetActive(visible);
                break;
            case 2:
                P3HP.SetActive(visible);
                P3EN.SetActive(visible);
                break;
            default:
                Debug.LogError("A correct 'who' value was not provided");
                break;
        }
    }


    /// <summary>
    /// The given character Loses a set amount of HP
    /// </summary>
    /// <param name="who"></param>
    /// Which character, the first, second or third?
    /// <param name="status"></param>
    /// Their respective ScriptableObject, which is automatically known
    /// <param name="hp"></param>
    /// Amount of HP to lose
    public void LoseHP(int who, CharacterStatus status, int hp, Color damageType)
    {
        int realLoss = (int)Mathf.Min(hp, status.currHealth);
        StartCoroutine(GraduallySetStatusBar(who, status, realLoss, false, 10, 0.05f));
        showDMG(who, hp, damageType);
    }
    /// <summary>
    /// The given character Gains a set amount of HP
    /// </summary>
    /// <param name="who"></param>
    /// Which character, the first, second or third?
    /// <param name="status"></param>
    /// Their respective ScriptableObject, which is automatically known
    /// <param name="hp"></param>
    /// Amount of HP to gain
    public void GainHP(int who, CharacterStatus status, int hp)
    {
        //CurrHP + hp <= MaxHP -> Fine
        //CurrHP + hp > MaxHP -> heal should be MaxHP - CurrHP
        int realGain = (int) Mathf.Min(hp, status.maxHealth - status.currHealth);
        StartCoroutine(GraduallySetStatusBar(who, status, realGain, true, 10, 0.05f));
        showDMG(who, hp, Color.green);
    }

    IEnumerator GraduallySetStatusBar(int who, CharacterStatus status, int amount, bool isIncrease, int fillTimes, float fillDelay)
    {
        //Make sure to pick the correct character whose stats are changed
        Image HPBar = null;
        TextMeshProUGUI HPVal = null;

        switch (who)
        {
            case 0:
                HPBar = P1HP.transform.GetChild(0).gameObject.GetComponent<Image>();
                HPVal = P1HP.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            case 1:
                HPBar = P2HP.transform.GetChild(0).gameObject.GetComponent<Image>();
                HPVal = P2HP.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            case 2:
                HPBar = P3HP.transform.GetChild(0).gameObject.GetComponent<Image>();
                HPVal = P3HP.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            default:
                Debug.LogError("A correct 'who' value was not provided");
                break;
        }

        //Finding the main percentage?
        float percentage = 1 / (float)fillTimes;
        int targetHealth = Mathf.RoundToInt(status.currHealth + amount * (isIncrease ? 1:-1));

        for (int fillStep = 0; fillStep < fillTimes; fillStep++)
        {
            //Taking part of the main percentage?
            float _fAmount = amount * percentage;
            float _dAmount = _fAmount / status.maxHealth;

            //Debug.Log($"{_fAmount} {_dAmount}");

            if (isIncrease)
            {
                //Note - Be VERY CAREFUL here, I don't know how severe round to int can cause health vs damage to go,
                //But SOMETHING is needed to prevent current health from turning into a visual float.
                status.currHealth += _fAmount;
                HPBar.fillAmount += _dAmount;
                if (status.currHealth <= status.maxHealth)
                    //HPVal.SetText(status.currHealth + "/" + status.maxHealth);
                    HPVal.SetText(Mathf.RoundToInt(status.currHealth) + "/" + status.maxHealth);
            }
            else
            {
                status.currHealth -= _fAmount;
                HPBar.fillAmount -= _dAmount;
                if (status.currHealth >= 0)
                    HPVal.SetText(Mathf.RoundToInt(status.currHealth) + "/" + status.maxHealth);
            }

            //For now, temporary fix like this
            /*if(fillStep == fillTimes - 1)
            {
                status.currHealth = Mathf.RoundToInt(status.currHealth);
                //HPBar.fillAmount = Mathf.Round(HPBar.fillAmount);
                HPBar.fillAmount = Mathf.Round(HPBar.fillAmount*100)/100.0f;
                HPVal.SetText(status.currHealth + "/" + status.maxHealth);
            }*/
            yield return new WaitForSeconds(fillDelay);
        }
        status.currHealth = targetHealth;
        HPVal.SetText(status.currHealth + "/" + status.maxHealth);
    }


    public void LoseEN(int who, CharacterStatus status, int en)
    {
        StartCoroutine(GraduallySetENBar(who, status, en, false, 10, 0.05f));
    }
    public void GainEN(int who, CharacterStatus status, int en)
    {
        StartCoroutine(GraduallySetENBar(who, status, en, true, 10, 0.05f));
    }

    IEnumerator GraduallySetENBar(int who, CharacterStatus status, int amount, bool isIncrease, int fillTimes, float fillDelay)
    {
        //Make sure to pick the correct character whose stats are changed
        Image ENBar = null;
        TextMeshProUGUI ENVal = null;

        Debug.Log($"I am setting EN for {who}");

        switch (who)
        {
            case 0:
                ENBar = P1EN.transform.GetChild(0).gameObject.GetComponent<Image>();
                ENVal = P1EN.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            case 1:
                ENBar = P2EN.transform.GetChild(0).gameObject.GetComponent<Image>();
                ENVal = P2EN.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            case 2:
                ENBar = P3EN.transform.GetChild(0).gameObject.GetComponent<Image>();
                ENVal = P3EN.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            default:
                Debug.LogError("A correct 'who' value was not provided");
                break;
        }

        //Finding the main percentage?
        float percentage = 1 / (float)fillTimes;
        float targetEnergy = status.currEnergy + amount * (isIncrease ? 1 : -1);

        for (int fillStep = 0; fillStep < fillTimes; fillStep++)
        {
            //Taking part of the main percentage?
            float _fAmount = amount * percentage;
            float _dAmount = _fAmount / status.maxEnergy;

            //Debug.Log($"{_fAmount} {_dAmount}");

            if (isIncrease)
            {
                //Note - Be VERY CAREFUL here, I don't know how severe round to int can cause energy vs damage to go,
                //But SOMETHING is needed to prevent current energy from turning into a visual float.
                status.currEnergy += _fAmount;
                ENBar.fillAmount += _dAmount;
                if (status.currEnergy <= status.maxEnergy)
                    ENVal.SetText(Mathf.RoundToInt(status.currEnergy) + "/" + status.maxEnergy);
            }
            else
            {
                status.currEnergy -= _fAmount;
                ENBar.fillAmount -= _dAmount;
                if (status.currEnergy >= 0)
                    ENVal.SetText(Mathf.RoundToInt(status.currEnergy) + "/" + status.maxEnergy);
            }

            yield return new WaitForSeconds(fillDelay);
        }
        status.currEnergy = targetEnergy;
        ENVal.SetText(status.currEnergy + "/" + status.maxEnergy);
    }

    private void showDMG(int who, int hp, Color LossOrGain) 
    {
        switch (who)
        {
            case 0:
                StartCoroutine(ShowingDMG(P1_UI_DMG, hp, LossOrGain));
                break;

            case 1:
                StartCoroutine(ShowingDMG(P2_UI_DMG, hp, LossOrGain));
                break;

            case 2:
                StartCoroutine(ShowingDMG(P3_UI_DMG, hp, LossOrGain));
                break;
        }

        
    }
    IEnumerator ShowingDMG(TextMeshProUGUI text, int hp, Color LossOrGain)
    {
        text.gameObject.SetActive(true);
        text.enabled = true;
        text.text = hp.ToString();
        text.color = LossOrGain;
        yield return new WaitForSeconds(1f);

        text.gameObject.SetActive(false);
        text.enabled = false;
        yield return null;
    }

    public void Buff(int who, bool state)
    {
        switch (who)
        {
            case 0: P1_Buff.gameObject.SetActive(state); P1_Buff.enabled = state; break;
            case 1: P2_Buff.gameObject.SetActive(state); P2_Buff.enabled = state; break;
            case 2: P3_Buff.gameObject.SetActive(state); P3_Buff.enabled = state; break;
        }
    }

    public void Debuff(int who, bool state)
    {
        switch (who)
        {
            case 0: P1_Debuff.gameObject.SetActive(state); P1_Debuff.enabled = state; break;
            case 1: P2_Debuff.gameObject.SetActive(state); P2_Debuff.enabled = state; break;
            case 2: P3_Debuff.gameObject.SetActive(state); P3_Debuff.enabled = state; break;
        }
    }

}
