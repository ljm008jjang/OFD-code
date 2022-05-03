using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public GameManager gameManager;

    public UIManager uiManager;

    public Admob_test_all adsManager;

    public bool isBasicCarOverPlayed = false;//기본차로 맵30 넘음?

    public bool isSportCarOverPlayed = false;//스포츠카로 맵30 넘음?

    public Follower[] players;

    public MoveCamera moveCamera;

    public ValueSave valueSave;


    public bool isOverEnd = false; // 이것을 통해 맵 무한반복 결정함.

    public bool isHardModeOverEnd = false;

    public int indexWhenChangeCar;

    public int basicCarSceneIndex = 1;

    public int SportCarSceneIndex = 1;

    public int hardModeIndex = 1;//나중에 31로 변환

    public void FirstSceneLoad()//게임 키고 맨 처음 실행됨, ValueSave Awake()에서 실행
    {
        if (SceneManager.GetActiveScene().buildIndex.Equals(0) && valueSave.GetValueBool("isCurrentBasicCar"))
        {
            StartCoroutine(MakeObjectAndLoadNextScene(0));
        }
        else if (SceneManager.GetActiveScene().buildIndex.Equals(0) && valueSave.GetValueBool("isCurrentSportCar"))
        {
            StartCoroutine(MakeObjectAndLoadNextScene(1));
        }
    }

    public void Awake()
    {
        valueSave = FindObjectOfType<ValueSave>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex.Equals(indexWhenChangeCar)  && gameManager.isCurrentBasicCar && gameManager.isOpenSportCar.Equals(false))
        {
            gameManager.isOpenSportCar = true;

            valueSave.SaveBool("isOpenSportCar", gameManager.isOpenSportCar);

            uiManager.OpenNewCar.gameObject.SetActive(true);

        }

    }
    public void GoNextScene()
    {
        if (uiManager.soundScript.isSoundOn)
            uiManager.SuccessBtnNonAD.GetComponent<AudioSource>().Play();

        //gameManager.completeUI.SetActive(false);
        uiManager.SuccessBtnNonAD.enabled = false;
        uiManager.SuccessBtnAD.enabled = false;

        gameManager.onResist = false;

        gameManager.gold += (int)gameManager.incomeGold;

        valueSave.SaveInt("gold", gameManager.gold);

        if ((Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork)) && !gameManager.isPurchasedNoAD)//인터넷 연결이 될때만
        {
            gameManager.InterstitialStack++;

            valueSave.SaveInt("InterstitialStack", gameManager.InterstitialStack);
        }

        if (gameManager.isHardModePlaying && !isHardModeOverEnd)
        {
            valueSave.SaveInt("hardModeIndex", hardModeIndex+1);
        }
        else
        {
            if (gameManager.isCurrentBasicCar && !isOverEnd)
            {
                valueSave.SaveInt("basicCarSceneIndex", basicCarSceneIndex + 1);
            }
            else if (gameManager.isCurrentSportCar && !isOverEnd)
            {
                valueSave.SaveInt("SportCarSceneIndex", SportCarSceneIndex + 1);
            }
        }
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void GoNextSceneAD()
    {

        if (uiManager.soundScript.isSoundOn)
            uiManager.SuccessBtnAD.GetComponent<AudioSource>().Play();

        if (Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork) )
        {
            //gameManager.completeUI.SetActive(false);

            uiManager.SuccessBtnNonAD.enabled = false;

            uiManager.SuccessBtnAD.enabled = false;

            gameManager.onResist = false;

            gameManager.gold += (int)gameManager.incomeGold;

            valueSave.SaveInt("gold", gameManager.gold);

            if (gameManager.isHardModePlaying && !isHardModeOverEnd)
            {
                valueSave.SaveInt("hardModeIndex", hardModeIndex + 1);
            }
            else
            {
                if (gameManager.isCurrentBasicCar && !isOverEnd)
                {
                    valueSave.SaveInt("basicCarSceneIndex", basicCarSceneIndex + 1);
                }
                else if (gameManager.isCurrentSportCar && !isOverEnd)
                {
                    valueSave.SaveInt("SportCarSceneIndex", SportCarSceneIndex + 1);
                }
            }

            StartCoroutine(LoadLevelAD(SceneManager.GetActiveScene().buildIndex + 1));


        }
        else
        {
            GoNextScene();
        }
    }

    public void GoCurrentScene()
    {
        if (uiManager.soundScript.isSoundOn)
            uiManager.FailBtnNonAD.GetComponent<AudioSource>().Play();

        uiManager.FailBtnNonAD.enabled = false;

        uiManager.FailBtnAD.enabled = false;

        gameManager.gold += (int)gameManager.incomeGold;

        valueSave.SaveInt("gold", gameManager.gold);

        if ((Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork)  || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork) ) && !gameManager.isPurchasedNoAD)
        {
            gameManager.InterstitialStack++;
            valueSave.SaveInt("InterstitialStack", gameManager.InterstitialStack);
        }

        StartCoroutine(LoadCurrentLevel());
    }

    public void GoCurrentSceneAD()
    {
        if (uiManager.soundScript.isSoundOn)
            uiManager.FailBtnAD.GetComponent<AudioSource>().Play();

        if (Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork))
        {
            uiManager.FailBtnNonAD.enabled = false;

            uiManager.FailBtnAD.enabled = false;

            gameManager.gold += (int)gameManager.incomeGold;
            
            valueSave.SaveInt("gold", gameManager.gold);

            StartCoroutine(LoadCurrentLevelAD());
        }
        else
        {
            GoCurrentScene();
        }
    }

    public void GoBasicCarScene()
    {

        uiManager.chooseBasicCarBtn.enabled = false;

        uiManager.chooseSportCarBtn.enabled = false;

        uiManager.startUI.gameObject.SetActive(true);

        StartCoroutine(LoadCarLevel(0));
    }

    public void GoSportCarScene()
    {
        uiManager.chooseBasicCarBtn.enabled = false;

        uiManager.chooseSportCarBtn.enabled = false;
        //        uiManager.chooseSportCarBtn.enabled = false;

        uiManager.startUI.gameObject.SetActive(true);

        StartCoroutine(LoadCarLevel(1));
    }

    IEnumerator MakeObjectAndLoadNextScene(int a)
    {
        if(gameManager.isHardModePlaying){
            yield return SceneManager.LoadSceneAsync(hardModeIndex);
            isOverEnd = isHardModeOverEnd;
        }
        else
        {
            if (valueSave.GetValueBool("isCurrentBasicCar"))
            {
                yield return SceneManager.LoadSceneAsync(valueSave.GetValueInt("basicCarSceneIndex"));

                isOverEnd = isBasicCarOverPlayed;
            }
            else if (valueSave.GetValueBool("isCurrentSportCar"))
            {
                yield return SceneManager.LoadSceneAsync(valueSave.GetValueInt("SportCarSceneIndex"));

                isOverEnd = isSportCarOverPlayed;
            }
        }
   

        for (int i = 0; i < players[a].audioSources.Length; i++)
        {
            for (int j = 0; j < players.Length; j++)
            {
                players[j].audioSources[i].Stop();
            }
        }

        for (int i = 0; i < players[a].audioSources.Length; i++)
        {
            players[a].audioSources[i].Play();
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].gameObject.SetActive(false);
        }
        players[a].gameObject.SetActive(true);

        moveCamera.cameraPoint = players[a].cameraPoint;

        uiManager.player = players[a];

        gameManager.player = players[a];

        gameManager.ReloadSettingWhenChangeCar();

        //transition.SetTrigger("Start");


    
    }

    public void LoadLevelOverlp(int levelIndex)
    {
        //gameManager.startCoroutineObstacle();

        if (levelIndex.Equals(indexWhenChangeCar)  && gameManager.isCurrentBasicCar && gameManager.isOpenSportCar.Equals(false))
        {

            gameManager.isOpenSportCar = true;
            valueSave.SaveBool("isOpenSportCar", gameManager.isOpenSportCar);
            uiManager.OpenNewCar.gameObject.SetActive(true);

        }

        

        if (gameManager.isHardModePlaying)
        {
            if (!isHardModeOverEnd)
            {
                hardModeIndex += 1;
                valueSave.SaveInt("hardModeIndex", hardModeIndex);
            }
            if (hardModeIndex.Equals(SceneManager.sceneCountInBuildSettings - 1))
            {
                isHardModeOverEnd = true;
                valueSave.SaveBool("isHardModeOverEnd", isHardModeOverEnd);
                isOverEnd = isHardModeOverEnd;//하드모드에서는 isoverend변수가 필요한가?
            }
        }
        else
        {
            if (gameManager.isCurrentBasicCar)
            {
                basicCarSceneIndex += 1;
                valueSave.SaveInt("basicCarSceneIndex", basicCarSceneIndex);

                if(basicCarSceneIndex >= 31)
                {
                    gameManager.isHardModeOpen = true;
                    valueSave.SaveBool("isHardModeOpen", gameManager.isHardModeOpen);
                }

                if (basicCarSceneIndex.Equals(SceneManager.sceneCountInBuildSettings - 1))
                {
                    isBasicCarOverPlayed = true;
                    valueSave.SaveBool("isBasicCarOverPlayed", isBasicCarOverPlayed);
                    isOverEnd = isBasicCarOverPlayed;//바꿀때도 하자!
                }
            }
            else if (gameManager.isCurrentSportCar)
            {
                SportCarSceneIndex += 1;
                valueSave.SaveInt("SportCarSceneIndex", SportCarSceneIndex);

                if (SportCarSceneIndex >= 31)
                {
                    gameManager.isHardModeOpen = true;
                    valueSave.SaveBool("isHardModeOpen", gameManager.isHardModeOpen);
                }

                if (SportCarSceneIndex.Equals(SceneManager.sceneCountInBuildSettings - 1))
                {

                    isSportCarOverPlayed = true;

                    valueSave.SaveBool("isSportCarOverPlayed", isSportCarOverPlayed);
                    isOverEnd = isSportCarOverPlayed;


                }
            }
        }
        

    }
    IEnumerator LoadLevel(int levelIndex)
    {
        if ((Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork)) && !gameManager.isPurchasedNoAD)
        {
            if (gameManager.InterstitialStack.Equals(gameManager.RewardRange) || gameManager.InterstitialStack >= 4)
            {
                adsManager.ShowInterstitial();
                gameManager.InterstitialStack = 0;
                valueSave.SaveInt("InterstitialStack", gameManager.InterstitialStack);
            }
        }
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCo());
        
        if (!isOverEnd)
        {
            //transition.SetTrigger("End");

            yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCo());

            

            yield return SceneManager.LoadSceneAsync(levelIndex);

            gameManager.ReloadSetting();

            LoadLevelOverlp(levelIndex);

            //transition.SetTrigger("Start");
        }
        else
        {
            StartCoroutine(LoadLevel());
        }
    }

    IEnumerator LoadLevelAD(int levelIndex)
    {
        if (Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork))
        {

            adsManager.ShowRewardedAd();//광고실행

            yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCoAD());


            if (!isOverEnd)
            {
                //transition.SetTrigger("End");

                yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCoAD());

                

                yield return SceneManager.LoadSceneAsync(levelIndex);

                gameManager.ReloadSettingAD();

                LoadLevelOverlp(levelIndex);

               // transition.SetTrigger("Start");
            }
            else
            {
                StartCoroutine(LoadLevelAD());
            }
        }
        else
        {
            StartCoroutine(LoadLevel(levelIndex));
        }
    }
    //a= 플레이어 배열 숫자
    IEnumerator LoadCarLevel(int a)
    {
        if (!gameManager.isHardModePlaying)
        {
           
            
            if (a.Equals(0))
            {
                yield return SceneManager.LoadSceneAsync(basicCarSceneIndex);
                uiManager.normalCarInspector.gameObject.SetActive(false);
            }
            else if (a.Equals(1))
            {
                yield return SceneManager.LoadSceneAsync(SportCarSceneIndex);
                uiManager.sportCarInspector.gameObject.SetActive(false);
            }
            //gameManager.startCoroutineObstacle();

            uiManager.SelectCarScroll.gameObject.SetActive(false);


            if (a.Equals(0))
                isOverEnd = isBasicCarOverPlayed;
            else if (a.Equals(1))
                isOverEnd = isSportCarOverPlayed;

            for (int i = 0; i < players[a].audioSources.Length; i++)
            {
                for (int j = 0; j < players.Length; j++)
                {
                    players[j].audioSources[i].Stop();
                }
            }

            for (int i = 0; i < players[a].audioSources.Length; i++)
            {
                players[a].audioSources[i].Play();
            }

            for (int i = 0; i < players.Length; i++)
            {
                players[i].gameObject.SetActive(false);
            }
            players[a].gameObject.SetActive(true);

            moveCamera.cameraPoint = players[a].cameraPoint;

            uiManager.player = players[a];

            gameManager.player = players[a];

            gameManager.ReloadSettingWhenChangeCar();



            //transition.SetTrigger("Start");

        }
        else
        {
            if (a.Equals(0))
            {
                uiManager.normalCarInspector.gameObject.SetActive(false);
            }
            else if (a.Equals(1))
            {
                uiManager.sportCarInspector.gameObject.SetActive(false);
            }

            uiManager.SelectCarScroll.gameObject.SetActive(false);

            for (int i = 0; i < players[a].audioSources.Length; i++)
            {
                for (int j = 0; j < players.Length; j++)
                {
                    players[j].audioSources[i].Stop();
                }
            }

            for (int i = 0; i < players[a].audioSources.Length; i++)
            {
                players[a].audioSources[i].Play();
            }

            for (int i = 0; i < players.Length; i++)
            {
                players[i].gameObject.SetActive(false);
            }
            players[a].gameObject.SetActive(true);

            moveCamera.cameraPoint = players[a].cameraPoint;

            uiManager.player = players[a];

            gameManager.player = players[a];

            gameManager.ReloadSettingWhenChangeCar();

        }
        //transition.SetTrigger("End");


    }

    //IEnumerator LoadSportCarLevel()
    //{
        
    //        transition.SetTrigger("End");

    //        yield return new WaitForSeconds(2.5f);

    //        yield return SceneManager.LoadSceneAsync(SportCarSceneIndex);

    //    uiManager.SelectCarScroll.gameObject.SetActive(false);

    //        isOverEnd = isSportCarOverPlayed;

    //        players[0].audioSources[0].Stop();
    //        players[0].audioSources[1].Stop();

    //        players[0].gameObject.SetActive(false);

    //        players[1].gameObject.SetActive(true);

    //        players[1].audioSources[1].Play();
    //        players[1].audioSources[0].Play();

    //        moveCamera.cameraPoint = players[1].cameraPoint;

    //        uiManager.player = players[1];

    //        gameManager.player = players[1];

    //        gameManager.ReloadSettingWhenChangeCar();

    //        transition.SetTrigger("Start");

    //}

    IEnumerator LoadLevel()
    {
        /*
        if ((Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork)) && !gameManager.isPurchasedNoAD)
        {
            if (gameManager.InterstitialStack.Equals( gameManager.RewardRange) || gameManager.InterstitialStack >= 4)
            {
                adsManager.ShowInterstitial();
                gameManager.InterstitialStack = 0;
                valueSave.SaveInt("InterstitialStack", gameManager.InterstitialStack);
            }
        }
        */
        //yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCo());

        //transition.SetTrigger("End");

       // yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCo());

        

        yield return SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 1);

        StartCoroutine(AllClearText());
        gameManager.ReloadSetting();

        //transition.SetTrigger("Start");
    }

    IEnumerator LoadLevelAD()
    {
        if (Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork))
        {
            //adsManager.ShowRewardedMultipleMoney();//광고실행

            //yield return new WaitForSeconds(0.1f);

            //yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCoAD());

            //transition.SetTrigger("End");

            yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCoAD());


            

            yield return SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 1);

            StartCoroutine(AllClearText());

            gameManager.ReloadSettingAD();

            //transition.SetTrigger("Start");
        }
        
        else
        {
            StartCoroutine(LoadLevel());
        }
    }

    public void LoadCurrentLevelOverlap()
    { 
        uiManager.failUI.GetComponent<Animator>().SetTrigger("IsComplete");

        gameManager.isFail = false;

        gameManager.clickRorL = false;

        if(gameManager.obstacleManager!=null)
            gameManager.obstacleManager.StartCoroutineSPO();

        gameManager.onResist = false;

        gameManager.player.Distancetravelled = 0;

        gameManager.player.speed = 0;

        gameManager.fuelCapacity = gameManager.player.fullFuelCapacity;

        //gameManager.gold += (int)gameManager.incomeGold;

        valueSave.SaveInt("gold", gameManager.gold);

        gameManager.incomeGold = 0;

        gameManager.mapLength = gameManager.player.PathCreator.path.length - gameManager.mapLengthCoefficient - Vector3.Distance(gameManager.destination.transform.position, gameManager.player.PathCreator.path.GetPoint(gameManager.player.PathCreator.path.NumPoints - 1));

        Camera.main.GetComponent<MoveCamera>().isDoingCameraMov = false;

        

        uiManager.IfClickFailBtn();

        gameManager.player.UpdateCarAndCam();

    }

    IEnumerator LoadCurrentLevel()
    {
        if ((Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork)) && !gameManager.isPurchasedNoAD)
        {
            if (gameManager.InterstitialStack.Equals(gameManager.RewardRange)  || gameManager.InterstitialStack >= 4)
            {
                adsManager.ShowInterstitial();
                gameManager.InterstitialStack = 0;
                valueSave.SaveInt("InterstitialStack", gameManager.InterstitialStack);
            }
        }
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCo());

        //transition.SetTrigger("End");

        yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCo());


        LoadCurrentLevelOverlap();

        uiManager.ReloadUISetting();

        //transition.SetTrigger("Start");
    }

    IEnumerator LoadCurrentLevelAD()
    {
        if (Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork))
        {
            adsManager.ShowRewardedAd();

            yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCoAD());

            //transition.SetTrigger("End");

            yield return StartCoroutine(uiManager.moveCoin.ManyCoinsCoAD());

            //if(adsManager.isAdsEnd)
            //{

                gameManager.gold += (int)gameManager.incomeGold * (gameManager.RewardRange - 1);
                //adsManager.isAdsEnd = false;

            //}
            


            LoadCurrentLevelOverlap();

            uiManager.ReloadUISetting();

            //transition.SetTrigger("Start");
        }
        else
        {
            StartCoroutine(LoadCurrentLevel());
        }
    }

    IEnumerator LoadHardModeLevel(int levelIndex)
    {

        isOverEnd = isHardModeOverEnd;
        if (!isHardModeOverEnd)
        {
            

            yield return SceneManager.LoadSceneAsync(levelIndex);

            gameManager.ReloadSetting();

  
            //hardModeIndex += 1;
            //valueSave.SaveInt("basicCarSceneIndex", basicCarSceneIndex);
            if (hardModeIndex.Equals(SceneManager.sceneCountInBuildSettings - 1))
            {
                //isBasicCarOverPlayed = true;
                //valueSave.SaveBool("isBasicCarOverPlayed", isBasicCarOverPlayed);
                isHardModeOverEnd = true;//바꿀때도 하자!
                valueSave.SaveBool("isHardModeOverEnd",isHardModeOverEnd);
                isOverEnd = true;
            }
            

        }
        else
        {
            StartCoroutine(LoadHardModeLevel());
        }
    }

    IEnumerator LoadHardModeLevel()
    {
        //transition.SetTrigger("End");

       

        yield return SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings-1);

        StartCoroutine(AllClearText());
        

        gameManager.ReloadSetting();

        //transition.SetTrigger("Start");
    }

    IEnumerator LoadBasicModeLevel(int levelIndex)
    {
        if (gameManager.isCurrentBasicCar)
        {
            isOverEnd = isBasicCarOverPlayed;
        }else if (gameManager.isCurrentSportCar)
        {
            isOverEnd = isSportCarOverPlayed;
        }


        if (!isOverEnd)
        {
            

            yield return SceneManager.LoadSceneAsync(levelIndex);

            gameManager.ReloadSetting();


            //hardModeIndex += 1;
            //valueSave.SaveInt("basicCarSceneIndex", basicCarSceneIndex);
           


        }
        else
        {
            StartCoroutine(LoadHardModeLevel());
        }
    }

    public void ifClickHardMode()
    {
        if (!gameManager.isHardModeOpen)
            // SceneManager.GetActiveScene().buildIndex < gameManager.indexWhenChangeLogic)
        {
            uiManager.NotOpenHM.gameObject.SetActive(true);
        }
        else
        {
            if(gameManager.obstacleManager != null)
                gameManager.obstacleManager.StopCoroutineSPO();


            if ((Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork)) && !gameManager.isPurchasedNoAD)
            {
                gameManager.InterstitialStack++;
                valueSave.SaveInt("InterstitialStack", gameManager.InterstitialStack);
            }

            if (!gameManager.isHardModePlaying)
            {
                uiManager.hardBtn.gameObject.SetActive(false);
                uiManager.normalBtn.gameObject.SetActive(true);
                uiManager.HMText.gameObject.SetActive(true);

                gameManager.isHardModePlaying = true;
                ValueSave.SetBool("isHardModePlaying", gameManager.isHardModePlaying);

                StartCoroutine(LoadHardModeLevel(hardModeIndex));
            }
            else
            {
                uiManager.hardBtn.gameObject.SetActive(true);
                uiManager.normalBtn.gameObject.SetActive(false);
                uiManager.HMText.gameObject.SetActive(false);

                gameManager.isHardModePlaying = false;
                ValueSave.SetBool("isHardModePlaying", gameManager.isHardModePlaying);
                if (gameManager.isCurrentBasicCar)
                {
                    StartCoroutine(LoadBasicModeLevel(basicCarSceneIndex));//overplayed면 어떻게하지??
                                                                           //하드모드에서 차바꾸면 차만바꾸게!!
                }
                else if (gameManager.isCurrentSportCar)
                {
                    StartCoroutine(LoadBasicModeLevel(SportCarSceneIndex));
                }

            }
        }

        
    }

    IEnumerator AllClearText()
    {
        uiManager.AllClearText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        uiManager.AllClearText.gameObject.SetActive(false);
    }
}
