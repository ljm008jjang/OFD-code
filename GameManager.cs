    using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PathCreation;
//using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public float fuelCapacity;//현재 연료량
    public string carName;
    public bool clickRorL = false;
    public float goldGain; // 골드획득량
    public float incomeGold; // 스테이지가 끝난 후 얻게되는 골드양
    public int RewardRange;

    //public float playerMoveLength;//이동한 길이
    public float gravityCoefficient; //중력계수
    //public float savedGravityCoefficient; //중력계수 저장

    public float mapLengthCoefficient;


    public int goldUpgreadCost = 50;
    public int gold = 10000; // 현재 재화 량
    public float mapLength;//맵의 길의
    public float RightCoefficient = 1.5f;
    [SerializeField]
    int[] texiNumByIndex;
    public int texiNum;
    


    public bool isClickRight = true;
    public bool isTexiMove = false;
    public bool isHardModePlaying = false;
    public bool isHardModeOpen = false;

    public bool isFail = false;
    public bool canMove = false;
    public bool onResist = false;
    public bool isTutorial = true;
    public bool isOpenSportCar = false; //스포츠카 열렸음?

    public bool isCurrentBasicCar;// 현재 차 부울값 확인용
    public bool isCurrentSportCar;
    public bool isFuelEmpty = true;

    public int goldLevel;
    public bool isClickRBRBtn = false;

    public List<PathCreator> pathCreators;
    public MakeRoad makeRoad;
    public Destination destination;
    public Follower player;
    public UIManager UImanager;
    public SceneMover sceneManager;
    public Texi[] texi;
    StartTutorial startTutorial;
    public Admob_reward admobReward;
    public int InterstitialStack = 0;//이니셜 광고 실행 스택
    public int indexWhenChangeLogic = 31;
    public ValueSave valueSave;
    public ObstacleManager obstacleManager = null;

    bool waitSecondForVibrate = false;

    public bool isPurchasedNoAD = false;

    bool bPaused;
    int sC;

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            bPaused = true;
            Time.timeScale = 0;
        }
        else
        {
            if (bPaused)
            {
                bPaused = false;
                if (!UImanager.settingPage.activeSelf)
                {
                    Time.timeScale = 1;
                }
            }
        }
    }

    private void Awake()
    {
        //Application.targetFrameRate = 30;

        valueSave = FindObjectOfType<ValueSave>();
        //Application.targetFrameRate = 60;

        
        WhenAwake();

        player.audioSources[0].Play();
        player.audioSources[1].Play();

    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex >= indexWhenChangeLogic && isHardModePlaying)
        {
            for (int i = 0; i < texiNum; i++)
                texi[i].PathCreator = player.PathCreator;

            UImanager.HMText.gameObject.SetActive(true);
        }

        SettingUpgradeCost();

        StartCoroutine(player.StaticPos());

        if (isClickRBRBtn)
        {

            UImanager.RewardRBBtn.enabled = false;

            UImanager.RewardRBBtn.gameObject.SetActive(false);

            StartCoroutine(CountTime());
        }

        if (SceneManager.GetActiveScene().buildIndex >= indexWhenChangeLogic && obstacleManager == null)
        {
            obstacleManager = FindObjectOfType<ObstacleManager>();
        }
        mapLength = player.PathCreator.path.length - mapLengthCoefficient - Vector3.Distance(destination.transform.position, player.PathCreator.path.GetPoint(player.PathCreator.path.NumPoints - 1));

        incomeGold = (int)mapLength * goldGain;


    }

    // Update is called once per frame

    IEnumerator WhenAction()
    {
        while (player.isAction)
        {

            UImanager.UpdateMapGaugeUI();

            UImanager.UpdateFuelRemainUI();


            if (fuelCapacity <= 0.01f && !isFuelEmpty)
            {
                isFuelEmpty = true;
                if (isFuelEmpty)
                {
                    StopCoroutine(PlayerSoundUp());
                    StartCoroutine(PlayerSoundDown());
                }

            }




            if (player.speed <= 0.0001f && fuelCapacity <= 0.01f && !isFail )//연료 떨어지면 실패
            {
                WhenFail();
                yield break;
            }

            yield return null;

        }

        yield break;
    }


   public void WhenFail()
    {
        if (!isFail)
        {
            isFail = true;

            incomeGold = (int)player.Distancetravelled * goldGain;

            isTexiMove = false;



            fuelCapacity = 0;

            for (int i = 0; i < player.smoke.Length; i++)
                player.smoke[i].Stop();

            UImanager.IfIsFail();

            UImanager.failUI.GetComponent<Animator>().SetTrigger("IsComplete");

            // playerMoveLength = 0;


            Camera.main.GetComponent<MoveCamera>().isDoingCameraMov = true;

            StopCoroutine(WhenAction());
        }
        
    }

    public void UpgradeEngine()
    {
        if (gold >= player.engineUpgreadCost)
        {
            if (UImanager.soundScript.isSoundOn)
                UImanager.powerUpgreadeBtn.GetComponent<AudioSource>().Play();

            gold -= player.engineUpgreadCost;

            player.engineForce += 0.005f; // 레벨 받아서 하는게 좋지 않을까?

            switch (player.engineLevel)
            {
                case 1:
                    player.engineUpgreadCost = 75;
                    break;
                case 2:
                    player.engineUpgreadCost = 100;
                    break;
                default:
                    player.engineUpgreadCost = 100 * (player.engineLevel - 1);
                    break;
            }

            player.engineLevel++;
                   
            if ((player.engineLevel%10).Equals(1))
            {
                if(player.limitSpeed < 100)
                {
                    player.limitSpeed += 0.1f;
                }
                    
            }
            

            if(player.limitSpeed > 100)
            {
                player.limitSpeed = 100;
            }

            if (isCurrentBasicCar)
            {
            valueSave.SaveInt("engineLevelb", player.engineLevel);
                valueSave.SaveFloat("limitSpeedb", player.limitSpeed);
            }
            else if(isCurrentSportCar)
            {
            valueSave.SaveInt("engineLevels", player.engineLevel);
                valueSave.SaveFloat("limitSpeeds", player.limitSpeed);
            }
            valueSave.SaveInt("gold", gold);
            UImanager.UpdateEngineUpgreadText();
            UImanager.UpdateGoldUI();


        }
        else
            return;
    }

    public void UpgradeFuel()
    {
        if (gold >= player.fuelUpgreadCost)
        {
            if (UImanager.soundScript.isSoundOn)
                UImanager.fuelUpgreadeBtn.GetComponent<AudioSource>().Play();

            gold -= player.fuelUpgreadCost;

            player.fullFuelCapacity += 0.5f;

            fuelCapacity = player.fullFuelCapacity;

            switch (player.fuelLevel)
            {
                case 1:
                    player.fuelUpgreadCost = 75;
                    break;
                case 2:
                    player.fuelUpgreadCost = 100;
                    break;
                default:

                    player.fuelUpgreadCost = 100 * (player.fuelLevel - 1);

                    break;
            }

            player.fuelLevel++;

           
            if ((player.fuelLevel % 10).Equals(1))
            {
                if (player.fuelRate > 0.5f)
                {
                    player.fuelRate -= 0.01f;
                }
                        
            }
            

            if(player.fuelRate < 0.5f)
            {
                player.fuelRate = 0.5f;
            }

            if (isCurrentBasicCar)
            {
                valueSave.SaveInt("fuelLevelb", player.fuelLevel);
                valueSave.SaveFloat("fuelRateb", player.fuelRate);
            }
            else if (isCurrentSportCar)
            {
                valueSave.SaveInt("fuelLevels", player.fuelLevel);
                valueSave.SaveFloat("fuelRates", player.fuelRate);
            }
            valueSave.SaveInt("gold", gold);
            UImanager.UpdateFuelUpgreadText();
            UImanager.UpdateGoldUI();


        }
        else
            return;
    }

    public void UpgradeGold()
    {
        if (gold >= goldUpgreadCost)
        {
            if (UImanager.soundScript.isSoundOn)
                UImanager.coinUpgreadeBtn.GetComponent<AudioSource>().Play();

            gold -= goldUpgreadCost;
            
            goldGain += 0.1f;

            switch (goldLevel)
            {
                case 1:
                    goldUpgreadCost = 75;
                    break;
                case 2:
                    goldUpgreadCost = 100;
                    break;
                default:
                    goldUpgreadCost = 100 * (goldLevel - 1);
                    break;
            }

            goldLevel++;

            valueSave.SaveInt("goldLevel", goldLevel);
            valueSave.SaveInt("gold", gold);
            UImanager.UpdateGoldUpgreadText();
            UImanager.UpdateGoldUI();

        }
        else
            return;

    }

    public void SettingUpgradeCost()
    {
        if(isCurrentBasicCar)
            player.engineForce = 0.055f + 0.005f * (player.engineLevel - 1); // 레벨 받아서 하는게 좋지 않을까?
        else if(isCurrentSportCar)
            player.engineForce = 0.1f + 0.005f * (player.engineLevel - 1);

        switch (player.engineLevel)
        {
            case 1:
                player.engineUpgreadCost = 50;
                break;
            case 2:
                player.engineUpgreadCost = 75;
                break;
            default:
                player.engineUpgreadCost = 100 * (player.engineLevel - 2);
                break;
        }
        UImanager.UpdateEngineUpgreadText();
  

        if(isCurrentBasicCar)
            player.fullFuelCapacity = 10 + 0.5f * (player.fuelLevel - 1);
        else if(isCurrentSportCar)
            player.fullFuelCapacity = 10 + 0.5f * (player.fuelLevel - 1);

        fuelCapacity = player.fullFuelCapacity;

        switch (player.fuelLevel)
        {
            case 1:
                player.fuelUpgreadCost = 50;
                break;
            case 2:
                player.fuelUpgreadCost = 75;
                break;
            default:
                player.fuelUpgreadCost = 100 * (player.fuelLevel - 2);
                break;
        }
        UImanager.UpdateFuelUpgreadText();
        

        goldGain = 1 + 0.1f * (goldLevel - 1);

        switch (goldLevel)
        {
            case 1:
                goldUpgreadCost = 50;
                break;
            case 2:
                goldUpgreadCost = 75;
                break;
            default:
                goldUpgreadCost = 100 * (goldLevel - 2);
                break;
        }
        UImanager.UpdateGoldUpgreadText();
        UImanager.UpdateGoldUI();

        valueSave.SaveInt("goldLevel", goldLevel);

            

    }
    

    public void OnSetting()//세팅 버튼을 누를 시 
    {

        /*UImanager.IfClickSettingBtn();

        player.savedSpeed = player.speed;

        player.speed = 0;

        gravityCoefficient = 0; //중력 효과 무시

        canMove = false;
        */
        UImanager.IfClickSettingBtn();

        Time.timeScale = 0;
    }

    public void OffSetting()
    {

        /* UImanager.IfClickOffSettingBtn();

         player.speed = player.savedSpeed;

         player.savedSpeed = 0;

         gravityCoefficient = savedGravityCoefficient; //중력 효과 무시

         canMove = true;
        */
        
        UImanager.IfClickOffSettingBtn();

        Time.timeScale = 1;
    }

    public void ClickFailBtn()
    {
        /*        isFail = false;

                player.Distancetravelled = 0;

                player.speed = 0;

                fuelCapacity = player.fullFuelCapacity;

                gold += incomeGold;

                incomeGold = 0;

                mapLength = player.PathCreator.path.length - mapLengthCoefficient - Vector3.Distance(destination.transform.position, player.PathCreator.path.GetPoint(player.PathCreator.path.NumPoints - 1));

                UImanager.IfClickFailBtn();
        */

        sceneManager.GoCurrentScene();
        
    }

   
    public void ActiveNextButton()
    {
        

        for(int i=0; i< player.smoke.Length;i++)
        player.smoke[i].Stop();

        UImanager.IfIsSuccess();

        UImanager.completeUI.GetComponent<Animator>().SetTrigger("IsComplete");

        StopCoroutine(PlayerSoundUp());

        StartCoroutine(PlayerSoundDown());

        UImanager.SuccessBtnADScaleText.GetComponent<Animator>().SetTrigger("Succeed");
        //   playerMoveLength = 0;
    }

    public void ReloadSettingOverlap()
    {
        CountTexiNum();

        clickRorL = false;

        player.PathCreator = FindObjectOfType<PathCreator>();

        if (SceneManager.GetActiveScene().buildIndex >= indexWhenChangeLogic && isHardModePlaying)
        {
            for(int i=0; i < texiNum; i++)
            {
                texi[i].gameObject.SetActive(true);
                texi[i].PathCreator = player.PathCreator;
                texi[i].right = texi[i].rightCoefficient;
            }

        }
        else
        {
            for (int i = 0; i < texiNum; i++)
            {
                texi[i].gameObject.SetActive(false);
            }
        }

        if (SceneManager.GetActiveScene().buildIndex >= indexWhenChangeLogic && obstacleManager == null)
        {
            obstacleManager = FindObjectOfType<ObstacleManager>();
        }

        //startCoroutineObstacle();

        makeRoad.CreateRoad();

        makeRoad.FixDestinationLocation();

        UImanager.completeUI.GetComponent<Animator>().SetTrigger("IsComplete");

        player.Distancetravelled = 0;

        player.speed = 0;

        fuelCapacity = player.fullFuelCapacity;

        player.isAction = false;

        Camera.main.GetComponent<MoveCamera>().isDoingCameraMov = false;

        //Camera.main.transform.position += player.transform.up * 10 + player.transform.right * 10;
        //Camera.main.transform.LookAt(player.transform);

        mapLength = player.PathCreator.path.length - mapLengthCoefficient - Vector3.Distance(destination.transform.position, player.PathCreator.path.GetPoint(player.PathCreator.path.NumPoints - 1));

        player.right = RightCoefficient;
   

        isClickRight = true;

        player.UpdateCarAndCam();
    }

    public void ReloadSetting()
    {

        ReloadSettingOverlap();
        UImanager.ReloadUISetting();
        //incomeGold = 0;
        incomeGold = (int)mapLength * goldGain;
    }

    public void ReloadSettingAD()
    {
        ReloadSettingOverlap();
        UImanager.ReloadUISetting();

        if(sceneManager.adsManager.isAdsEnd)
        {
            gold += (int)incomeGold * (RewardRange - 1);
            sceneManager.adsManager.isAdsEnd = false;
        }
        


        valueSave.SaveInt("gold", gold);
        UImanager.UpdateGoldUI();
        incomeGold = (int)mapLength * goldGain;
    }

    public void ReloadSettingWhenChangeCar()
    {
        ReloadSettingOverlap();
        UImanager.ReloadUISettingWhenChangeCar();
    }

    public void ClickSoundSetting()
    {
        UImanager.IfClickSoundSetting();


    }

    public void ClickVibrateSetting()
    {
        UImanager.IfClickVibrateSetting();
    }

    void WhenAwake()
    {
        fuelCapacity = player.fullFuelCapacity;

        
        player.PathCreator = FindObjectOfType<PathCreator>();


      

        player.right = RightCoefficient;
        for (int i = 0; i < texiNum; i++)
            texi[i].right = texi[i].rightCoefficient;

        isClickRight = true;

        makeRoad.CreateRoad();

        //makeRoad.FixDestinationLocation();
    }
    
    IEnumerator GOGOGO()
    {
        while(player.isAction && fuelCapacity > 0 && canMove && onResist)
        {
            if (!clickRorL)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    for (int i = 0; i < player.smoke.Length; i++)
                        player.smoke[i].Play();

                    /*if (UImanager.VibrateScript.isVibrateOn && waitSecondForVibrate == false && onResist == true)
                    {
                        DoVibrate();
                    }
                    */
                }
                if (Input.GetMouseButton(0))
                {
                    /*if (UImanager.VibrateScript.isVibrateOn)
                        DoVibrate();
                    */

                    for (int i = 0; i < player.smoke.Length; i++)
                        player.smoke[i].startSize = player.speed * 0.025f;
                            //Mathf.Lerp(player.smoke[i].startSize, player.speed * 0.025f, Time.deltaTime);

                    if (isCurrentBasicCar)
                    { // 일반 자동차 연비 및 기어

                        //일반 자동차 연비
                        fuelCapacity -= Time.deltaTime * player.fuelRate; // 1초에 1씩 소비;


                        //노말카는 1단 기어만 존재
                        if (player.speed < player.limitSpeed)  // 최고속도 제한
                        {
                            if (player.speed + player.engineForce * Time.deltaTime * 750 < player.limitSpeed)
                                player.speed += player.engineForce * Time.deltaTime * 750; // 750 = FuelAlpha
                            else
                                player.speed = player.limitSpeed;
                        }


                    }

                    else if (isCurrentSportCar)
                    { // 스포츠카 연비 및 기어변속구간 설정


                        //스포츠카 연비
                        fuelCapacity -= Time.deltaTime * player.fuelRate;




                        // 기어변속구간
                        if (player.speed < 6)  // 초반 가속 제한 (정지 관성)
                        {
                            sC = 300;
                            if (player.speed + player.engineForce * Time.deltaTime * sC < player.limitSpeed)
                                player.speed += player.engineForce * Time.deltaTime * sC; // 750 = FuelAlpha
                            else
                                player.speed = player.limitSpeed;

                        }
                        else if (6 <= player.speed && player.speed < 16)  // 초반 가속 제한 (정지 관성)
                        {
                            sC = 450;
                            if (player.speed + player.engineForce * Time.deltaTime * sC < player.limitSpeed)
                                player.speed += player.engineForce * Time.deltaTime * sC; // 750 = FuelAlpha
                            else
                                player.speed = player.limitSpeed;

                        }
                        else if (16 <= player.speed && player.speed < 30)  // 초반 가속 제한 (정지 관성)
                        {
                            sC = 600;
                            if (player.speed + player.engineForce * Time.deltaTime * sC < player.limitSpeed)
                                player.speed += player.engineForce * Time.deltaTime * sC; // 750 = FuelAlpha
                            else
                                player.speed = player.limitSpeed;

                        }
                        else if (30 <= player.speed && player.speed < player.limitSpeed)  // 최고속도 제한
                        {
                            sC = 700;
                            if (player.speed + player.engineForce * Time.deltaTime * sC < player.limitSpeed)
                                player.speed += player.engineForce * Time.deltaTime * sC; // 750 = FuelAlpha
                            else
                                player.speed = player.limitSpeed;
                        }



                    }





                    if (player.speed < 0.5f)  // 떨어지다가 올라갈때 스피드 영점조절
                    {

                        player.speed = 0.5f;
                        //player.speed += player.engineForce * Time.deltaTime * player.FuelAlpha;

                    }

                    if (player.breakLight && player.RearSideLight && !player.isClick)
                    {
                        player.isClick = true;
                        player.breakLight.SetActive(false);
                        player.RearSideLight.SetActive(true);
                    }

                }
                if (Input.GetMouseButtonUp(0))
                {
                    for (int i = 0; i < player.smoke.Length; i++)
                        player.smoke[i].Stop();

                    /*if(UImanager.VibrateScript.isVibrateOn == true && waitSecondForVibrate == true)
                    {
                        waitSecondForVibrate = false;
                    }
                    */
                }
                if (Input.GetMouseButtonUp(0) && player.isClick && player.breakLight)
                {
                    player.isClick = false;
                    player.RearSideLight.SetActive(false);

                }
            }
            

            yield return null;
        }

        yield break;
    }

    public void ClickTapToStart()
    {
        player.isAction = true;

        StartCoroutine(WhenAction());
        StartCoroutine(player.GravityEffect());


        fuelCapacity = player.fullFuelCapacity;
        canMove = true;
        onResist = true;
        isFuelEmpty = false;
        isTexiMove = true;

        StartCoroutine(player.PlayAudioSource());
        StartCoroutine(player.ResistForce());

        for (int i = 0; i < player.audioSources.Length; i++)
        {
            player.audioSources[i].Play();
        }

        StopCoroutine(PlayerSoundDown());
        StartCoroutine(PlayerSoundUp());
        StartCoroutine(GOGOGO());

        if(SceneManager.GetActiveScene().buildIndex >= indexWhenChangeLogic)
        {
            StartCoroutine(ChangeRight());

            if (isHardModePlaying)
            {
                for (int i = 0; i < texiNum; i++)
                {
                    StartCoroutine(texi[i].TexiMove());

                }                   
                for (int i = 0; i < texiNum; i++)
                    StartCoroutine(texi[i].crossMove());
            }
            
        }
        if(UImanager.AllClearText.gameObject.activeSelf)
            UImanager.AllClearText.gameObject.SetActive(false);


        UImanager.IfClickTapToStartBtn();
    }

    public void ClickBasicCarBtn()
    {
        if(!isCurrentBasicCar)
        {
            isCurrentBasicCar = true;

            //나머지 부울값은 false로!
            if(isCurrentSportCar)
            {
                isCurrentSportCar = false;
            }

            sceneManager.GoBasicCarScene();
        }
        else if(isCurrentBasicCar)
        {
            UImanager.normalCarInspector.gameObject.SetActive(false);
            UImanager.SelectCarScroll.gameObject.SetActive(false);
            UImanager.startUI.gameObject.SetActive(true);
        }
        valueSave.SaveBool("isCurrentBasicCar", isCurrentBasicCar);
        valueSave.SaveBool("isCurrentSportCar", isCurrentSportCar);
    }

    public void ClickSportCarBtn()
    {
        if(!isCurrentSportCar && isOpenSportCar)
        {
            isCurrentSportCar = true;

            if(isCurrentBasicCar)
            {
                isCurrentBasicCar = false;
            }

            sceneManager.GoSportCarScene();
        }
        else if (isCurrentSportCar)
        {
            UImanager.sportCarInspector.gameObject.SetActive(false);
            UImanager.SelectCarScroll.gameObject.SetActive(false);
            UImanager.startUI.gameObject.SetActive(true);
        }
        valueSave.SaveBool("isCurrentBasicCar", isCurrentBasicCar);
        valueSave.SaveBool("isCurrentSportCar", isCurrentSportCar);
    }

    public void ClickOpenNewCarBtn()
    {
        UImanager.lockSportCar.SetActive(false);

        UImanager.OpenNewCar.gameObject.SetActive(false);
    }

    public void CloseChooseCarUI()
    {
        UImanager.SelectCarScroll.gameObject.SetActive(false);
    }

    public IEnumerator PlayerSoundDown()
    {
        int speed = 1;

        while(player.audioSources[0].volume>=0)
        {

            player.audioSources[0].volume = Mathf.Lerp(player.audioSources[0].volume, 0, Time.deltaTime * speed);
            player.audioSources[1].volume = Mathf.Lerp(player.audioSources[1].volume, 0, Time.deltaTime * speed);

            if (player.audioSources[0].volume <= 0.01f && player.audioSources[1].volume <= 0.01f)
            {
                player.audioSources[0].volume = 0;
                player.audioSources[1].volume = 0;
                yield break;
            }

            yield return null;
        }


    }

    public IEnumerator PlayerSoundUp()
    {
        int speed = 3;

        while (player.audioSources[0].volume <=1 && player.isAction)
        {

            player.audioSources[0].volume = Mathf.Lerp(player.audioSources[0].volume, 1, Time.deltaTime * speed);

            if (player.audioSources[0].volume >= 0.99f)
            {
                player.audioSources[0].volume = 1;

                yield break;
            }
            yield return null;
        }
    }

    void DoVibrate()
    {
        waitSecondForVibrate = true;

        StartCoroutine(Vibrate());

    }

    IEnumerator Vibrate()
    {
        while (UImanager.VibrateScript.isVibrateOn && waitSecondForVibrate && onResist)
        {
            Vibrator.Vibrate(100);
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void IfPurchasingNoAD()
    {
        if(!isPurchasedNoAD)
        {
            isPurchasedNoAD = true;

            valueSave.SaveBool("isPurchasedNoAD", isPurchasedNoAD);

            UImanager.startBtns.Remove(UImanager.noAdsBtn.GetComponent<Animator>());

            StartCoroutine(removeNoADBtn());

            sceneManager.adsManager.DestroyBanner();
        }
    }

    IEnumerator removeNoADBtn()
    {
        yield return new WaitForSeconds(0.1f);

        UImanager.noAdsBtn.gameObject.SetActive(false);
    }

    IEnumerator ClickOpenBtn()
    {
        UImanager.randomBoxImg.GetComponent<Animator>().SetTrigger("Open");
        yield return new WaitForSeconds(1.8f);
        UImanager.RBParticle.Play();

        yield return new WaitForSeconds(1.2f);
        

        UImanager.randomBoxImg.GetComponent<Animator>().SetTrigger("Open");

        int rand = UnityEngine.Random.Range(0, 90);
        if(rand >=0 && rand <= 4)//엔진1
        {
            if ((player.engineLevel%10).Equals(0))
            {
                player.limitSpeed += 0.1f;
                if (player.limitSpeed > 100)
                {
                    player.limitSpeed = 100;
                }
            }

          
            player.engineLevel += 1;
            player.engineForce += 0.005f;
            UImanager.UpdateEngineUpgreadText();
            UImanager.RBRewardImage[0].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Engine 1 level Upgrade!";
        }
        else if(rand >=5 && rand <= 7)//엔진2
        {
            if ((player.engineLevel % 10).Equals(9) || (player.engineLevel % 10).Equals(0))
            {
                player.limitSpeed += 0.1f;

                if (player.limitSpeed > 100)
                {
                    player.limitSpeed = 100;
                }
            }

            
            player.engineLevel += 2;
            player.engineForce += 0.005f * 2;
            UImanager.UpdateEngineUpgreadText();
            UImanager.RBRewardImage[0].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Engine 2 level Upgrade!";
        }
        else if (rand >= 8 && rand <= 9)//엔진3
        {
            if ((player.engineLevel % 10).Equals(8) || (player.engineLevel % 10).Equals(9) || (player.engineLevel % 10).Equals(0))
            {
                player.limitSpeed += 0.1f;
                if (player.limitSpeed > 100)
                {
                    player.limitSpeed = 100;
                }
            }

            
            player.engineLevel += 3;
            player.engineForce += 0.005f * 3;
            UImanager.UpdateEngineUpgreadText();
            UImanager.RBRewardImage[0].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Engine 3 level Upgrade!";

        }
        else if (rand >= 10 && rand <= 14)//연료1
        {
            if ((player.fuelLevel % 10).Equals(0))
            {
                player.fuelRate -= 0.01f;
                if (player.fuelRate < 0.5f)
                {
                    player.fuelRate = 0.5f;
                }
            }

            player.fuelLevel += 1;
            player.fullFuelCapacity += 0.5f;
            UImanager.UpdateFuelUpgreadText();
            UImanager.RBRewardImage[1].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Fuel 1 level Upgrade!";
        }
        else if (rand >= 15 && rand <= 17)//연료2
        {
            if ((player.fuelLevel % 10).Equals(9) || (player.fuelLevel % 10).Equals(0))
            {
                player.fuelRate -= 0.01f;
                if (player.fuelRate < 0.5f)
                {
                    player.fuelRate = 0.5f;
                }
            }

            player.fuelLevel += 2;
            player.fullFuelCapacity += 0.5f * 2;
            UImanager.UpdateFuelUpgreadText();
            UImanager.RBRewardImage[1].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Fuel 2 level Upgrade!";
        }
        else if (rand >= 18 && rand <= 19)//연료3
        {
            if ((player.fuelLevel % 10).Equals(8) || (player.fuelLevel % 10).Equals(9) || (player.fuelLevel % 10).Equals(0))
            {
                player.fuelRate -= 0.01f;
                if (player.fuelRate < 0.5f)
                {
                    player.fuelRate = 0.5f;
                }
            }

            player.fuelLevel += 3;
            player.fullFuelCapacity += 0.5f * 3;
            UImanager.UpdateFuelUpgreadText();
            UImanager.RBRewardImage[1].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Fuel 3 level Upgrade!";
        }
        else if (rand >= 20 && rand <= 24)//돈1
        {
            goldLevel += 1;
            goldGain += 0.1f;

            UImanager.UpdateGoldUpgreadText();
            UImanager.RBRewardImage[2].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Earning 1 level Upgrade!";
        }
        else if (rand >= 25 && rand <= 27)//돈2
        {
            goldLevel += 2;
            goldGain += 0.1f * 2;

            UImanager.UpdateGoldUpgreadText();
            UImanager.RBRewardImage[2].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Earning 2 level Upgrade!";
        }
        else if (rand >= 28 && rand <= 29)//돈3
        {
            goldLevel += 3;
            goldGain += 0.1f * 3;

            UImanager.UpdateGoldUpgreadText();
            UImanager.RBRewardImage[2].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "Earning 3 level Upgrade!";
        }
        else if (rand >= 30 && rand <= 39)//골드1
        {
            gold += 2500;
            
            UImanager.UpdateGoldUI();
            UImanager.RBRewardImage[3].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "2500 Gold!";
        }
        else if (rand >= 40 && rand <= 49)//골드2
        {
            gold += 5000;

            UImanager.UpdateGoldUI();
            UImanager.RBRewardImage[2].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "5000 Gold!";
        }
        else if (rand >= 50 && rand <= 59)//골드3
        {
            gold += 10000;

            UImanager.UpdateGoldUI();
            UImanager.RBRewardImage[4].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "10000 Gold!";
        }
        else if (rand >= 60 && rand <= 69)//골드4
        {
            gold += 15000;

            UImanager.UpdateGoldUI();
            UImanager.RBRewardImage[5].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "15000 Gold!";
        }
        else if (rand >= 70 && rand <= 79)//골드5
        {
            gold += 20000;

            UImanager.UpdateGoldUI();
            UImanager.RBRewardImage[6].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "20000 Gold!";
        }
        else if (rand >= 80 && rand <= 81)//최고속도1
        {

            player.limitSpeed += 0.1f;
            
            if(player.limitSpeed > 100)
            {
                player.limitSpeed = 100;
            }
            UImanager.RBRewardImage[0].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "'Limit Speed' is increased! 0.1";
        }
        else if (rand >= 82 && rand <= 83)//최고속도2
        {

                player.limitSpeed += 0.2f;

            if (player.limitSpeed > 100)
            {
                player.limitSpeed = 100;
            }
            UImanager.RBRewardImage[0].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "'Limit Speed' is increased! 0.2";
        }
        else if (rand ==84)//최고속도3
        {

                player.limitSpeed += 0.3f;

            if (player.limitSpeed > 100)
            {
                player.limitSpeed = 100;
            }
            UImanager.RBRewardImage[0].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "'Limit Speed' is increased! 0.3";
        }
        else if (rand >= 85 && rand <= 86)//연비1
        {

                player.fuelRate -= 0.01f;

            if(player.fuelRate < 0.5f)
            {
                player.fuelRate = 0.5f;
            }
            UImanager.RBRewardImage[1].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "'Fuel Rate' is increased! 0.1";
        }
        else if (rand >= 87 && rand <= 88)//연비2
        {
 
                player.fuelRate -= 0.02f;

            if (player.fuelRate < 0.5f)
            {
                player.fuelRate = 0.5f;
            }
            UImanager.RBRewardImage[1].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "'Fuel Rate' is increased! 0.2";
        }
        else if (rand ==89 )//연비3
        {
 
                player.fuelRate -= 0.03f;

            if (player.fuelRate < 0.5f)
            {
                player.fuelRate = 0.5f;
            }
            UImanager.RBRewardImage[1].gameObject.SetActive(true);
            UImanager.RandomBoxResultText.text = "'Fuel Rate' is increased! 0.3";
        }

        SettingUpgradeCost();

        if (isCurrentBasicCar)
        {
            valueSave.SaveInt("fuelLevelb", player.fuelLevel);
            valueSave.SaveInt("engineLevelb", player.engineLevel);
            valueSave.SaveFloat("limitSpeedb", player.limitSpeed);
            valueSave.SaveFloat("fuelRateb", player.fuelRate);
        }
        else if (isCurrentSportCar)
        {
            valueSave.SaveInt("fuelLevels", player.fuelLevel);
            valueSave.SaveInt("engineLevels", player.engineLevel);
            valueSave.SaveFloat("limitSpeeds", player.limitSpeed);
            valueSave.SaveFloat("fuelRates", player.fuelRate);
        }
        UImanager.UpdateGoldUI();
        valueSave.SaveInt("gold", gold);
        valueSave.SaveInt("goldLevel", goldLevel);

        UImanager.RandomBoxBackground.gameObject.SetActive(true);
        UImanager.RandomBoxBackground.GetComponent<Animator>().SetTrigger("IsComplete");
        UImanager.openRBBoxBtn.enabled = true;
        UImanager.exitRBPageBtn.enabled = true;
        UImanager.RewardRBBtn.enabled = true;

        UImanager.RBGetOrBackText.text = "Get It";
        //UImanager.RBResultParticle.Play();
    }

    public void ClickOpenRandomBox()
    {
        UImanager.openRBBoxBtn.enabled = false;
        UImanager.RewardRBBtn.enabled = false;
        UImanager.exitRBPageBtn.enabled = false;
        if(gold >= 10000)
        {
            gold -= 10000;
            if (UImanager.soundScript.isSoundOn)
                UImanager.randomBoxImg.GetComponent<AudioSource>().Play();

            UImanager.UpdateGoldUI();
            StartCoroutine(ClickOpenBtn());

        }
        else
        {
            UImanager.RandomBoxBackground.gameObject.SetActive(true);
            UImanager.RandomBoxBackground.GetComponent<Animator>().SetTrigger("IsComplete");
            UImanager.RandomBoxResultText.text = "You don't have 10000 money";
            UImanager.openRBBoxBtn.enabled = true;
            UImanager.exitRBPageBtn.enabled = true;
            UImanager.RewardRBBtn.enabled = true;
            UImanager.RBResultParticle.Stop();

            UImanager.RBGetOrBackText.text = "Back";
            
        }
        
    }

    public void ClickRewardRBBtn()
    {
        if ((Application.internetReachability.Equals(NetworkReachability.ReachableViaCarrierDataNetwork) || Application.internetReachability.Equals(NetworkReachability.ReachableViaLocalAreaNetwork)))
        {
            UImanager.exitRBPageBtn.enabled = false;
            UImanager.openRBBoxBtn.enabled = false;
            admobReward.ShowRewardedAd();

            Invoke("sss", 0.5f);
            
        }
        
    }

    void sss()
    {
        if (admobReward.isLuckyBoxShow == true && admobReward.isAdsExist == true)
        {
            admobReward.isLuckyBoxShow = false;

            isClickRBRBtn = true;

            valueSave.SaveBool("isClickRBRBtn", isClickRBRBtn);
            UImanager.RewardRBBtn.enabled = false;
            UImanager.RewardRBBtn.gameObject.SetActive(false);

            UpdateLastPlayDate();

            StartCoroutine(CountTime());

            if (UImanager.soundScript.isSoundOn)
                UImanager.randomBoxImg.GetComponent<AudioSource>().Play();

            StartCoroutine(ClickOpenBtn());

        }
        else// if (admobReward.isAdsExist == false)
        {
            UImanager.RandomBoxBackground.gameObject.SetActive(true);
            UImanager.RandomBoxBackground.GetComponent<Animator>().SetTrigger("IsComplete");
            UImanager.RandomBoxResultText.text = "Wait a seconds";
            UImanager.openRBBoxBtn.enabled = true;
            UImanager.exitRBPageBtn.enabled = true;
            UImanager.RewardRBBtn.enabled = true;
            UImanager.RBResultParticle.Stop();

            UImanager.RBGetOrBackText.text = "Back";
        }

    }

    IEnumerator CountTime()
    {
        WaitForSeconds second = new WaitForSeconds(1);

        int limit = 360;

        while (timeAfterLastPlay < limit)
        {

            if (UImanager.RandomBoxPage.activeSelf)
            {
                string str = ((limit - timeAfterLastPlay) % 60).ToString("D2");
      
                UImanager.RewardRBBtnOffText.text = (limit-timeAfterLastPlay) / 60 + ":" + str;
            }

            yield return second;
        }

        isClickRBRBtn = false;
        valueSave.SaveBool("isClickRBRBtn", isClickRBRBtn);

        UImanager.RewardRBBtn.gameObject.SetActive(true);
        UImanager.RewardRBBtn.enabled = true;
        
        yield break;
    }

    

    DateTime GetLastPlayDate()
    {
        if (!PlayerPrefs.HasKey("Time"))
        {
            return DateTime.Now;
        }

        string timeBinaryInString = PlayerPrefs.GetString("Time");
        long timeBinaryInLong = Convert.ToInt64(timeBinaryInString);
        return DateTime.FromBinary(timeBinaryInLong);
    }

    void UpdateLastPlayDate()
    {
        PlayerPrefs.SetString("Time", DateTime.Now.ToBinary().ToString());
    }

    public int timeAfterLastPlay
    {
        get
        {
            DateTime currentTime = DateTime.Now;
            DateTime lastPlayTime = GetLastPlayDate();

            return (int)currentTime.Subtract(lastPlayTime).TotalSeconds;
        }
    }
    
    public void ClickLeft()
    {
        if(player.right > 0)
        {
            clickRorL = true;

            isClickRight = false;
        }
    }

    public void OffClickRorL() {

        clickRorL = false;
    }

    public void ClickRight()
    {
        if(player.right < 0)
        {
            clickRorL = true;

            isClickRight = true;
        }
    }

    IEnumerator ChangeRight()
    {
        int s = 3;

        while (player.isAction)
        {
            if(isClickRight)
                player.right = Mathf.Lerp(player.right, RightCoefficient, Time.deltaTime * s);
            else
                player.right = Mathf.Lerp(player.right, -RightCoefficient, Time.deltaTime * s);

            yield return null;
        }

        yield break;
    }

    public void CountTexiNum()
    {
        if (SceneManager.GetActiveScene().buildIndex >= indexWhenChangeLogic && isHardModePlaying)//후에 수정
        {
            if (SceneManager.GetActiveScene().buildIndex <= texiNumByIndex[0])
            {
                texiNum = 1;
            }else if(SceneManager.GetActiveScene().buildIndex <= texiNumByIndex[1])
            {
                texiNum = 2;
            }
        }
    }
    /*
    public void StopCoroutineObstacle()
    {
        if (SceneManager.GetActiveScene().buildIndex < indexWhenChangeLogic && obstacleManager != null)
        {
            if(obstacleManager.stopVelCoroutine != null)
                StopCoroutine(obstacleManager.stopVelCoroutine);

        }
    }

    public void startCoroutineObstacle() {
        if (SceneManager.GetActiveScene().buildIndex < indexWhenChangeLogic && obstacleManager != null)
        {
            obstacleManager.stopVelCoroutine = StartCoroutine(obstacleManager.StopVel());
        }
    }
    */
    public void GetMoney()
    {
        gold += 100000;
        UImanager.UpdateGoldUI();
    }
}


