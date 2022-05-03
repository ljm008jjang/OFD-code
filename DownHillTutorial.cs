using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownHillTutorial : MonoBehaviour
{
    GameManager gameManager;
    
    private void OnTriggerEnter(Collider other)
    {
        gameManager = FindObjectOfType<GameManager>();
        if(other.gameObject.layer.Equals(8) && gameManager.isCurrentBasicCar)
        {
            gameManager.UImanager.TutorialUI.gameObject.SetActive(true);
            Time.timeScale = 0;

            //gameManager.UImanager.tutorialBackGround.gameObject.SetActive(true);
            gameManager.UImanager.DownHillText.gameObject.SetActive(true);
        }
    }
}
