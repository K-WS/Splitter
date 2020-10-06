using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // NOTE - THESE SHOULD BE MODIFIED ON SPLIT TOO, SPLIT CHARACTERS SHOULD AUTO-GENERATE THEIR STATS LIKE ENEMIES?
    public void SetStatusHUD(CharacterStatus status)
    {
        float curHP = status.currHealth * (100 / status.maxHealth);
        float curEN = status.currEnergy * (100 / status.maxEnergy);

        P1HPBar.fillAmount = curHP / 100;
        P1HPVal.SetText($"{status.currHealth}/{status.maxHealth}");

        P1ENBar.fillAmount = curEN / 100;
        P1ENVal.SetText($"{status.currEnergy}/{status.maxEnergy}");

    }

    //Note, this should also be modified to work for split characters
    //And maybe to be usable on any bar, not just HP
    public void SetHP(CharacterStatus status, float hp)
    {
        StartCoroutine(GraduallySetStatusBar(status, hp, false, 10, 0.05f));
    }
    //Percentage of a percentage gradually removed (lost HP)
    IEnumerator GraduallySetStatusBar(CharacterStatus status, float amount, bool isIncrease, int fillTimes, float fillDelay)
    {
        //Finding the main percentage?
        float percentage = 1 / (float)fillTimes;

        for (int fillStep = 0; fillStep < fillTimes; fillStep++)
        {
            //Taking part of the main percentage?
            float _fAmount = amount * percentage;
            float _dAmount = _fAmount / status.maxHealth;

            if (isIncrease)
            {
                status.currHealth += _fAmount;
                P1HPBar.fillAmount += _dAmount;
                if (status.currHealth <= status.maxHealth)
                    P1HPVal.SetText(status.currHealth + "/" + status.maxHealth);
            }
            else
            {
                status.currHealth -= _fAmount;
                P1HPBar.fillAmount -= _dAmount;
                if (status.currHealth >= 0)
                    P1HPVal.SetText(status.currHealth + "/" + status.maxHealth);
            }
            yield return new WaitForSeconds(fillDelay);
        }
    }

}
