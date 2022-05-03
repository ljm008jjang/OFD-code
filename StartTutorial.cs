using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTutorial : MonoBehaviour
{
    UIManager uiManager;
    GameManager gameManager;
    int tutorialIndex = 0;

    private void OnEnable()
    {
        gameManager = FindObjectOfType<GameManager>();
        uiManager = FindObjectOfType<UIManager>();
        uiManager.tutorialBackGround.GetComponent<Button>().onClick.AddListener(() =>
        {
            WhenClick();
        });

        if(gameManager.isTutorial)
        {
            uiManager.tutorialBackGround.gameObject.SetActive(true);
            uiManager.ClickPowerText.gameObject.SetActive(true);

            uiManager.powerUpgreadeBtn.enabled = false;
            uiManager.fuelUpgreadeBtn.enabled = false;
            uiManager.coinUpgreadeBtn.enabled = false;
            uiManager.openGarageBtn.enabled = false;

            uiManager.powerUpgreadeBtn.transform.SetSiblingIndex(10);
            uiManager.fuelUpgreadeBtn.transform.SetSiblingIndex(1);
            uiManager.coinUpgreadeBtn.transform.SetSiblingIndex(1);


        }
    }

    public void WhenClick()
    {
        if(tutorialIndex.Equals(0))
        {
            uiManager.ClickPowerText.gameObject.SetActive(false);
            uiManager.ClickFuelText.gameObject.SetActive(true);

            uiManager.powerUpgreadeBtn.transform.SetSiblingIndex(1);
            uiManager.fuelUpgreadeBtn.transform.SetSiblingIndex(10);
            uiManager.coinUpgreadeBtn.transform.SetSiblingIndex(1);

            tutorialIndex++;
        }
        else if(tutorialIndex.Equals(1))
        {
            uiManager.ClickFuelText.gameObject.SetActive(false);
            uiManager.ClickEarningText.gameObject.SetActive(true);

            uiManager.powerUpgreadeBtn.transform.SetSiblingIndex(1);
            uiManager.fuelUpgreadeBtn.transform.SetSiblingIndex(1);
            uiManager.coinUpgreadeBtn.transform.SetSiblingIndex(10);

            tutorialIndex++;
        }
        else if (tutorialIndex.Equals(2))
        {
            uiManager.ClickEarningText.gameObject.SetActive(false);
            uiManager.ClickGarageText.gameObject.SetActive(true);

            uiManager.powerUpgreadeBtn.transform.SetSiblingIndex(1);
            uiManager.fuelUpgreadeBtn.transform.SetSiblingIndex(1);
            uiManager.coinUpgreadeBtn.transform.SetSiblingIndex(1);
            uiManager.openGarageBtn.transform.SetSiblingIndex(10);

            tutorialIndex++;
        }
        else if(tutorialIndex.Equals(3))
        {
            uiManager.ClickGarageText.gameObject.SetActive(false);
            uiManager.openGarageBtn.transform.SetSiblingIndex(1);

            uiManager.flatText.gameObject.SetActive(true);

            tutorialIndex++;
        }
        else
        {
            uiManager.tutorialBackGround.gameObject.SetActive(false);
            uiManager.flatText.gameObject.SetActive(false);

            uiManager.powerUpgreadeBtn.enabled = true;
            uiManager.fuelUpgreadeBtn.enabled = true;
            uiManager.coinUpgreadeBtn.enabled = true;
            uiManager.openGarageBtn.enabled = true;

            gameManager.isTutorial = false;
            gameManager.valueSave.SaveBool("isTutorial", gameManager.isTutorial);
        }


    }
 
}
