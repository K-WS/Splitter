using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour
{
    public Image P1HPBar;
    public TextMeshProUGUI P1HPVal;
    public Image P1ENBar;
    public TextMeshProUGUI P1ENVal;

    public Image P2HPBar;
    public TextMeshProUGUI P2HPVal;
    public Image P2ENBar;
    public TextMeshProUGUI P2ENVal;

    public Image P3HPBar;
    public TextMeshProUGUI P3HPVal;
    public Image P3ENBar;
    public TextMeshProUGUI P3ENVal;

    /// <summary>
    /// Initializer to automatically set the HP values cased on CharacterStatus ScriptableObject given...
    /// and the integer to know which charcter...
    /// </summary>
    public void SetStatusHUD(int who, CharacterStatus status)
    {
        float curHP = status.currHealth * (100f / status.maxHealth);
        float curEN = status.currEnergy * (100f / status.maxEnergy);

        switch (who)
        {
            case 1:
                P1HPBar.fillAmount = curHP / 100f;
                P1HPVal.SetText($"{status.currHealth}/{status.maxHealth}");

                P1ENBar.fillAmount = curEN / 100f;
                P1ENVal.SetText($"{status.currEnergy}/{status.maxEnergy}");
                break;
            case 2:
                P2HPBar.fillAmount = curHP / 100f;
                P2HPVal.SetText($"{status.currHealth}/{status.maxHealth}");

                P2ENBar.fillAmount = curEN / 100f;
                P2ENVal.SetText($"{status.currEnergy}/{status.maxEnergy}");
                break;
            case 3:
                P3HPBar.fillAmount = curHP / 100f;
                P3HPVal.SetText($"{status.currHealth}/{status.maxHealth}");

                P3ENBar.fillAmount = curEN / 100f;
                P3ENVal.SetText($"{status.currEnergy}/{status.maxEnergy}");
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
                HPBar = P1HPBar; HPVal = P1HPVal;
                break;
            case 2:
                HPBar = P2HPBar; HPVal = P2HPVal;
                break;
            case 3:
                HPBar = P3HPBar; HPVal = P3HPVal;
                break;
            default:
                Debug.LogError("A correct 'who' value was not provided");
                break;
        }

        //Finding the main percentage?
        float percentage = 1 / (float)fillTimes;

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
                //status.currHealth += Mathf.RoundToInt(_fAmount);
                HPBar.fillAmount += _dAmount;
                if (status.currHealth <= status.maxHealth)
                    HPVal.SetText(status.currHealth + "/" + status.maxHealth);
            }
            else
            {
                status.currHealth -= _fAmount;
                HPBar.fillAmount -= _dAmount;
                if (status.currHealth >= 0)
                    HPVal.SetText(status.currHealth + "/" + status.maxHealth);
            }

            //For now, temporary fix like this
            if(fillStep == fillTimes - 1)
            {
                status.currHealth = Mathf.RoundToInt(status.currHealth);
                //HPBar.fillAmount = Mathf.Round(HPBar.fillAmount);
                HPBar.fillAmount = Mathf.Round(HPBar.fillAmount*100)/100.0f;
                HPVal.SetText(status.currHealth + "/" + status.maxHealth);
            }
            yield return new WaitForSeconds(fillDelay);
        }

    }
}
