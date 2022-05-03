using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpHillTutorial : MonoBehaviour
{
    GameManager gameManager;
    bool isFirst = true;

    private void OnTriggerEnter(Collider other)
    {
        gameManager = FindObjectOfType<GameManager>();
        if (other.gameObject.layer.Equals(8) && gameManager.isCurrentBasicCar && isFirst)
        {
            gameManager.UImanager.TutorialUI.gameObject.SetActive(true);

            isFirst = false;

            Time.timeScale = 0;
            //gameManager.UImanager.tutorialBackGround.gameObject.SetActive(true);
            gameManager.UImanager.UpHillText.gameObject.SetActive(true);
        }
    }
}
