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

    /// <summary>
    /// Initializer to automatically set the HP values cased on CharacterStatus ScriptableObject given...
    /// and the integer to know which charcter...
    /// On split characters, call this after their ScriptableObject HP vals updated
    /// </summary>
    public void SetStatusHUD(int who, CharacterStatus status)
    {
        switch (who)
        {
            case 1:
                setCharHUD(status, P1HP, P1EN);
                break;
            case 2:
                setCharHUD(status, P2HP, P2EN);
                break;
            case 3:
                setCharHUD(status, P3HP, P3EN);
                break;
            default:
                Debug.LogError("A correct 'who' value was not provided");
                break;
        }   
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
            case 1:
                P1HP.SetActive(visible);
                P1EN.SetActive(visible);
                break;
            case 2:
                P2HP.SetActive(visible);
                P2EN.SetActive(visible);
                break;
            case 3:
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
    public void LoseHP(int who, CharacterStatus status, int hp)
    {
        StartCoroutine(GraduallySetStatusBar(who, status, hp, false, 10, 0.05f));
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
        StartCoroutine(GraduallySetStatusBar(who, status, hp, true, 10, 0.05f));
    }

    IEnumerator GraduallySetStatusBar(int who, CharacterStatus status, int amount, bool isIncrease, int fillTimes, float fillDelay)
    {
        //Make sure to pick the correct character whose stats are changed
        Image HPBar = null;
        TextMeshProUGUI HPVal = null;

        switch (who)
        {
            case 1:
                HPBar = P1HP.transform.GetChild(0).gameObject.GetComponent<Image>();
                HPVal = P1HP.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            case 2:
                HPBar = P2HP.transform.GetChild(0).gameObject.GetComponent<Image>();
                HPVal = P2HP.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            case 3:
                HPBar = P3HP.transform.GetChild(0).gameObject.GetComponent<Image>();
                HPVal = P3HP.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                break;
            default:
                Debug.LogError("A correct 'who' value was not provided");
                break;
        }

        //Finding the main percentage?
        float percentage = 1 / (float)fillTimes;
        float targetHealth = status.currHealth + amount * (isIncrease ? 1:-1);

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
}
