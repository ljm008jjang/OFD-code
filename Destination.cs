using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Destination : MonoBehaviour
{
    public GameManager gameManager;
    //public Follower player;
    public bool isExitDestination = false;// destination프리펩을 지나치면 속도가 0이 되도록 한다
    public bool isEnterDestination = false;// destination프리펩을 들어가면 속도가 speedWhenFinished으로 선형보간

    public int finalIndex;

    IEnumerator DownSpeed()
    {
        while(isEnterDestination && gameManager.player.isAction)
        {
            gameManager.player.speed = Mathf.Lerp(gameManager.player.speed, gameManager.player.speedWhenFinished, Time.deltaTime * 3);

            yield return null;
        }
        yield break;
    }

    IEnumerator StopSpeed()
    {
        while (isExitDestination && gameManager.player.isAction)
        {
            gameManager.player.speed = 0;

            yield return null;
        }
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer.Equals(8))
        {
            gameManager.UImanager.FuelUI.gameObject.SetActive(false);

            gameManager.fuelCapacity = gameManager.player.fullFuelCapacity;

            gameManager.onResist = false;

            gameManager.isTexiMove = false;

            if (gameManager.player.Distancetravelled >= 0)
            {
                gameManager.incomeGold = (int)(gameManager.mapLength) * gameManager.goldGain;
                gameManager.RewardRange = Random.Range(2, 4);
                gameManager.UImanager.successBtnText.text = ((int)gameManager.incomeGold).ToString();
                gameManager.UImanager.successBtnADText.text = ((int)gameManager.incomeGold * gameManager.RewardRange).ToString();
                gameManager.UImanager.SuccessBtnADScaleText.text = "<b><#FFE400>X" + gameManager.RewardRange.ToString() + "</color></b>";

            }

            isEnterDestination = true;

            StartCoroutine(DownSpeed());
            //           player.isAction = false;

            //           gameManager.canMove = false;

            if (SceneManager.GetActiveScene().buildIndex >= gameManager.indexWhenChangeLogic)
            {
                gameManager.UImanager.LeftBtn.gameObject.SetActive(false);
                gameManager.UImanager.RightBtn.gameObject.SetActive(false);
            }

            

        }

        if (other.gameObject.layer.Equals(11) && gameManager.isTexiMove)
        {
            gameManager.isTexiMove = false;

            gameManager.WhenFail();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer.Equals(8) && gameManager.canMove)
        {
            isEnterDestination = false;

            isExitDestination = true;

            if(!gameManager.isFail)
            gameManager.ActiveNextButton();

            if (SceneManager.GetActiveScene().buildIndex.Equals(finalIndex))
            {
                StartCoroutine(ChangeColor());
            }

            gameManager.canMove = false;

//            StartCoroutine(player.StaticPos());

            Camera.main.GetComponent<MoveCamera>().isDoingCameraMov = true;

            StartCoroutine(StopSpeed());
            
            

        }
    }

    IEnumerator ChangeColor()
    {
        gameManager.UImanager.ClearLastLevel.gameObject.SetActive(true);

        Color colorImage = gameManager.UImanager.ClearLastLevel.color;
        Color colorText = gameManager.UImanager.ClearLastLevelText.color;
        float x = 1;
        while(x>=0.01){
            Debug.Log(x);

            x = Mathf.Lerp(x, 0, Time.deltaTime);

            colorImage.a = x;
            gameManager.UImanager.ClearLastLevel.color = colorImage;
            //new Color(colorImage.r, colorImage.g, colorImage.b, x);
            colorText.a = x;
            gameManager.UImanager.ClearLastLevelText.color = colorText;
            //new Color(colorText.r, colorText.g, colorText.b, x);

            yield return null;
        }

        colorImage.a = 0;
        gameManager.UImanager.ClearLastLevel.color = colorImage;
        //new Color(colorImage.r, colorImage.g, colorImage.b, x);
        colorText.a = 0;
        gameManager.UImanager.ClearLastLevelText.color = colorText;
        //new Color(colorText.r, colorText.g, colorText.b, x);

        gameManager.UImanager.ClearLastLevel.gameObject.SetActive(false);
    }

}
