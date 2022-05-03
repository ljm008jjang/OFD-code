using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;
using System.Text;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    public Follower normalCar;
    public Follower sportCar;

    public Follower player;
    public GameObject settingPage;
    public GameObject startUI;
    public GameObject completeUI;
    public GameObject failUI;
    public GameObject OpenNewCar;
    public GameObject FuelUI;
    public GameObject stageUI;
    public GameObject TutorialUI;
    public GameObject lockSportCar;
    public GameObject RandomBoxPage;
    public GameObject randomBoxImg;
    public GameObject normalCarInspector;
    public GameObject sportCarInspector;
    public TextMeshProUGUI failBtnText;
    public TextMeshProUGUI failBtnADText;
    public TextMeshProUGUI failBtnADScaleText;
    public TextMeshProUGUI successBtnText;
    public TextMeshProUGUI successBtnADText;
    public TextMeshProUGUI SuccessBtnADScaleText;
    public TextMeshProUGUI MyGold;
    public TextMeshProUGUI MyStage;
    public TextMeshProUGUI goldUpgreadText;
    public TextMeshProUGUI engineUpgreadText;
    public TextMeshProUGUI fuelUpgreadText;
    public TextMeshProUGUI goldLevelText;
    public TextMeshProUGUI engineLevelText;
    public TextMeshProUGUI fuelLevelText;
    public TextMeshProUGUI ClickPowerText;
    public TextMeshProUGUI ClickEarningText;
    public TextMeshProUGUI ClickFuelText;
    public TextMeshProUGUI ClickGarageText;
    public TextMeshProUGUI DownHillText;
    public TextMeshProUGUI UpHillText;
    public TextMeshProUGUI flatText;
    public TextMeshProUGUI RandomBoxResultText;
    public TextMeshProUGUI[] normalCarText;
    public TextMeshProUGUI[] sportCarText;
    public TextMeshProUGUI RBGetOrBackText;
    public TextMeshProUGUI RewardRBBtnOffText;
    public TextMeshProUGUI HMText;
    public TextMeshProUGUI ClearLastLevelText;
    public TextMeshProUGUI AllClearText;
    public Slider mapGuage;
    public Slider[] normalCarGauge;
    public Slider[] sportCarGauge;
    public Image settingBackground;
    public Image fuelRemainImg;
    public Image tutorialBackGround;
    public Image RandomBoxBackground;
    public Image[] RBRewardImage;
    public Image boxOpenBtn;
    public Image ClearLastLevel;
    public Image NotOpenHM;
    public Image RBInfo;
    //public Animator[] upgreadPanel;
    public List<Animator> startBtns;
    public GameObject SoundBtn;
    public GameObject VibrateBtn;
    public Sprite blueButtonSprite;
    public Sprite grayButtonSprite;
    public Button tapToStartBtn;
    public Button SuccessBtnAD;
    public Button FailBtnAD;
    public Button SuccessBtnNonAD;
    public Button FailBtnNonAD;
    public Button openGarageBtn;
    public Button chooseBasicCarBtn;
    public Button chooseSportCarBtn;
    public Button noAdsBtn;
    public Button powerUpgreadeBtn;
    public Button fuelUpgreadeBtn;
    public Button coinUpgreadeBtn;
    public Button openSettingBtn;
    public Button getRandomBoxResultBtn;
    public Button openRBBoxBtn;
    public Button exitRBPageBtn;
    public Button openRBPageBtn;
    public Button RewardRBBtn;
    public Button LeftBtn;
    public Button RightBtn;
    public Button normalBtn;
    public Button hardBtn;

    public ScrollRect SelectCarScroll;
    public TextMeshProUGUI tapToStartText;
    public MoveCoin moveCoin;// 0-성공 기본 1- 성공 광고 2- 실패 기본 3- 실패 광고
    public List<GameObject> pooledCoin;
    public GameObject coinPrefab;
    public GameObject coinPocket;
    public Transform coinTarget;
    public Transform coinIntial;
    public Transform coinIntialAD;
    public SoundScript soundScript;
    public VibrateScript VibrateScript;
    public ParticleSystem RBParticle;
    public ParticleSystem RBResultParticle;
    public Camera mainCamera;
    public Camera UICamera;
    int movingCoinNum = 10;
    WaitForSeconds second = new WaitForSeconds(1f);

    private void Start()
    {
        startBtns.Add(coinUpgreadeBtn.GetComponent<Animator>());
        startBtns.Add(fuelUpgreadeBtn.GetComponent<Animator>());
        startBtns.Add(powerUpgreadeBtn.GetComponent<Animator>());
        startBtns.Add(openGarageBtn.GetComponent<Animator>());
        startBtns.Add(openRBPageBtn.GetComponent<Animator>());
        startBtns.Add(hardBtn.GetComponent<Animator>());
        startBtns.Add(normalBtn.GetComponent<Animator>());
        if (!gameManager.isPurchasedNoAD)
        {
            startBtns.Add(noAdsBtn.GetComponent<Animator>());
        }
        else
        {
            noAdsBtn.gameObject.SetActive(!gameManager.isPurchasedNoAD);
        }

        soundScript = SoundBtn.GetComponent<SoundScript>();
        VibrateScript = VibrateBtn.GetComponent<VibrateScript>();

        if (soundScript.isSoundOn)
        {
            SoundBtn.GetComponent<Image>().sprite = blueButtonSprite;
            
        }
        else
        {
            SoundBtn.GetComponent<Image>().sprite = grayButtonSprite;
            AudioListener.pause = !soundScript.isSoundOn;
        }

        if (VibrateScript.isVibrateOn)
        {
            VibrateBtn.GetComponent<Image>().sprite = blueButtonSprite;
        }
        else
        {
            VibrateBtn.GetComponent<Image>().sprite = grayButtonSprite;
        }

        if (gameManager.isHardModePlaying)
        {
            normalBtn.gameObject.SetActive(true);
        }
        else
        {
            hardBtn.gameObject.SetActive(true);
        }

        UpdateStageUI();
        UpdateGoldUI();

        UpdateEngineUpgreadText();
        UpdateFuelUpgreadText();
        UpdateGoldUpgreadText();

        MakePoolingCoin();
    }

    void MakePoolingCoin()
    {

        pooledCoin = new List<GameObject>();

        GameObject tmp;

        for (int i = 0; i < movingCoinNum; i++)
        {
            tmp = Instantiate(coinPrefab, coinPocket.transform);

            //    tmp.transform.SetParent()

            tmp.SetActive(false);

            pooledCoin.Add(tmp);
        }

    }

    public void UpdateStageUI()
    {
        if(!gameManager.isHardModePlaying)
            MyStage.text = "Level: " + (SceneManager.GetActiveScene().buildIndex);
        else
            MyStage.text = "Level: " + (SceneManager.GetActiveScene().buildIndex - 30);
    }

    public void UpdateGoldUI()
    {
        MyGold.text = gameManager.gold.ToString();
    }
    public void UpdateGoldUpgreadText()
    {
        goldLevelText.text = "Level " + gameManager.goldLevel.ToString();
        goldUpgreadText.text = gameManager.goldUpgreadCost.ToString();
    }
    public void UpdateFuelUpgreadText()
    {
        fuelLevelText.text = "Level " + player.fuelLevel.ToString();
        fuelUpgreadText.text = player.fuelUpgreadCost.ToString();
    }
    public void UpdateEngineUpgreadText()
    {
        engineLevelText.text = "Level " + player.engineLevel.ToString();
        engineUpgreadText.text = player.engineUpgreadCost.ToString();
    }

    public void UpdateMapGaugeUI()//움직일때 상단 맵 이동정도 표시
    {
        mapGuage.value = player.Distancetravelled / gameManager.mapLength;
    }

    public void UpdateFuelRemainUI()
    {
        FuelUI.transform.position = Vector3.Lerp(fuelRemainImg.transform.position, Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up * 2.5f), Time.deltaTime);

        fuelRemainImg.fillAmount = gameManager.fuelCapacity / player.fullFuelCapacity;
    }

    

    public void IfClickTapToStartBtn()
    {
        FuelUI.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up * 2.5f);

        mapGuage.gameObject.SetActive(true);

        FuelUI.gameObject.SetActive(true);

        //        startUI.SetActive(false);
       
        for (int i = 0; i < startBtns.Count; i++)
            {
                startBtns[i].SetTrigger("IsStart");
        }



        StartCoroutine(OffStartUI());

        openGarageBtn.GetComponent<Button>().enabled = false;

        noAdsBtn.GetComponent<Button>().enabled = false;

        powerUpgreadeBtn.GetComponent<Button>().enabled = false;

        fuelUpgreadeBtn.GetComponent<Button>().enabled = false;

        coinUpgreadeBtn.GetComponent<Button>().enabled = false;

        openRBPageBtn.enabled = false;

        tapToStartBtn.gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex >= gameManager.indexWhenChangeLogic)
        {
            LeftBtn.gameObject.SetActive(true);
            RightBtn.gameObject.SetActive(true);
        }

        //차고르는 버튼도 애니매이션 추가
    }

    IEnumerator OffStartUI()
    {
        yield return second;

        startUI.gameObject.SetActive(false);
    }

    public void IfClickSettingBtn()
    {
        if (soundScript.isSoundOn)
            openSettingBtn.GetComponent<AudioSource>().Play();

        settingBackground.gameObject.SetActive(true);

        settingPage.gameObject.SetActive(true);

        for (int i = 0; i < player.audioSources.Length; i++)
            player.audioSources[i].Stop();
    }

    public void IfClickOffSettingBtn()
    {
        if (soundScript.isSoundOn)
            openSettingBtn.GetComponent<AudioSource>().Play();

        settingBackground.gameObject.SetActive(false);

        settingPage.gameObject.SetActive(false);

        for (int i = 0; i < player.audioSources.Length; i++)
            player.audioSources[i].Play();
    }

    public void IfClickFailBtn()
    {
        gameManager.canMove = false;

        failUI.SetActive(false);

        startUI.SetActive(true);



        

        player.isAction = false;

        gameManager.isClickRight = true;

        openGarageBtn.GetComponent<Button>().enabled = true;

        noAdsBtn.GetComponent<Button>().enabled = true;

        powerUpgreadeBtn.GetComponent<Button>().enabled = true;

        fuelUpgreadeBtn.GetComponent<Button>().enabled = true;

        coinUpgreadeBtn.GetComponent<Button>().enabled = true;

        tapToStartBtn.gameObject.SetActive(true);
        
        UpdateGoldUI();

        UpdateStageUI();
    }
    public void IfIsFail()
    {
        gameManager.RewardRange = Random.Range(2, 4);
        failBtnText.text = ((int)gameManager.incomeGold).ToString();
        failBtnADText.text = ((int)gameManager.incomeGold * gameManager.RewardRange).ToString();
        failBtnADScaleText.text = "<b><#FFE400>X" + gameManager.RewardRange.ToString() + "</color></b>";

        FailBtnAD.enabled = true;

        FailBtnNonAD.enabled = true;

        failUI.SetActive(true);

        mapGuage.gameObject.SetActive(false);

        FuelUI.gameObject.SetActive(false);

        //player.isAction = false;

        failBtnADScaleText.GetComponent<Animator>().SetTrigger("Succeed");

        if (SceneManager.GetActiveScene().buildIndex >= gameManager.indexWhenChangeLogic)
        {
            LeftBtn.gameObject.SetActive(false);
            RightBtn.gameObject.SetActive(false);
        }
    }
    public void IfIsSuccess()
    {
        mapGuage.gameObject.SetActive(false);

        FuelUI.gameObject.SetActive(false);

        completeUI.SetActive(true);

        SuccessBtnNonAD.enabled = true;

        SuccessBtnAD.enabled = true;
    }

    public void ReloadUISetting()
    {
        completeUI.SetActive(false);

        startUI.gameObject.SetActive(true);


        openGarageBtn.GetComponent<Button>().enabled = true;

        noAdsBtn.GetComponent<Button>().enabled = true;

        powerUpgreadeBtn.GetComponent<Button>().enabled = true;

        fuelUpgreadeBtn.GetComponent<Button>().enabled = true;

        coinUpgreadeBtn.GetComponent<Button>().enabled = true;

        openRBPageBtn.enabled = true;

        tapToStartBtn.gameObject.SetActive(true);

        UpdateGoldUI();

        UpdateStageUI();

        UpdateEngineUpgreadText();
        UpdateFuelUpgreadText();
        UpdateGoldUpgreadText();
    }

    public void ReloadUISettingWhenChangeCar()
    {
        completeUI.SetActive(false);

        tapToStartBtn.gameObject.SetActive(true);

        UpdateGoldUI();

        UpdateStageUI();

        UpdateEngineUpgreadText();
        UpdateFuelUpgreadText();
        UpdateGoldUpgreadText();
    }

    public void IfClickSoundSetting()
    {

        if (soundScript.isSoundOn)
        {
            SoundBtn.GetComponent<Image>().sprite = grayButtonSprite;

            AudioListener.pause = soundScript.isSoundOn;

            soundScript.isSoundOn = false;

            gameManager.valueSave.SaveBool("isSoundOn", soundScript.isSoundOn);
        }
        else
        {
            AudioSource soundAudio = SoundBtn.GetComponent<AudioSource>();

            SoundBtn.GetComponent<Image>().sprite = blueButtonSprite;

            AudioListener.pause = soundScript.isSoundOn;

            soundScript.isSoundOn = true;

            soundAudio.Play();

            gameManager.valueSave.SaveBool("isSoundOn", soundScript.isSoundOn); 
        }
    }

    public void IfClickVibrateSetting()
    {
        AudioSource vibrateAudio = SoundBtn.GetComponent<AudioSource>();
        if (VibrateScript.isVibrateOn)
        {
            if(soundScript.isSoundOn)
            vibrateAudio.Play();

            VibrateBtn.GetComponent<Image>().sprite = grayButtonSprite;

            VibrateScript.isVibrateOn = false;

            gameManager.valueSave.SaveBool("isVibrateOn", VibrateScript.isVibrateOn);
        }
        else
        {
            VibrateBtn.GetComponent<Image>().sprite = blueButtonSprite;

            VibrateScript.isVibrateOn = true;

            if (soundScript.isSoundOn)
                vibrateAudio.Play();

            gameManager.valueSave.SaveBool("isVibrateOn", VibrateScript.isVibrateOn);
        }
    }

    public void ClickOpenGarageBtn()
    {
        if (soundScript.isSoundOn)
            openGarageBtn.GetComponent<AudioSource>().Play();

        startUI.gameObject.SetActive(false);

        chooseBasicCarBtn.enabled = true;

        chooseSportCarBtn.enabled = true;

        SelectCarScroll.gameObject.SetActive(true);

        if (gameManager.isOpenSportCar)
            lockSportCar.SetActive(false);
    }

    public void ClickCloseGarageBtn()
    {
        if (soundScript.isSoundOn)
            openGarageBtn.GetComponent<AudioSource>().Play();

        startUI.gameObject.SetActive(true);

        SelectCarScroll.gameObject.SetActive(false);
    }

    public void ClickDownOrUpHill()
    {
        if(SceneManager.GetActiveScene().buildIndex.Equals(2))
        {
            //tutorialBackGround.gameObject.SetActive(false);
            DownHillText.gameObject.SetActive(false);
            TutorialUI.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
        else if(SceneManager.GetActiveScene().buildIndex.Equals(3))
        {
            //tutorialBackGround.gameObject.SetActive(false);
            UpHillText.gameObject.SetActive(false);
            TutorialUI.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void ClickOpenRandomBoxPage()
    {
        startUI.gameObject.SetActive(false);
        openSettingBtn.gameObject.SetActive(false);
        MyStage.gameObject.SetActive(false);
        tapToStartBtn.gameObject.SetActive(false);
        if (gameManager.isHardModePlaying)
            HMText.gameObject.SetActive(false);
        RandomBoxPage.gameObject.SetActive(true);
        UICamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        if (gameManager.isClickRBRBtn) { 
            RewardRBBtnOffText.text = (300 - gameManager.timeAfterLastPlay) / 60 + ":" + ((300 - gameManager.timeAfterLastPlay) % 60).ToString("D2");
        }

        if (soundScript.isSoundOn)
        {
            boxOpenBtn.GetComponent<AudioSource>().Play();
            
        }
            

        if (gameManager.gold < 10000)
        {
            boxOpenBtn.color = Color.gray;

        }
        else
        {
            boxOpenBtn.color = Color.white;
        }
        
    }

    public void ClickRandomBoxResultBtn()
    {
        for(int i=0; i < RBRewardImage.Length; i++)
        {
            if (RBRewardImage[i].IsActive())
            {
                RBRewardImage[i].gameObject.SetActive(false);
            }
        }
        if (gameManager.gold < 10000)
        {
            boxOpenBtn.color = Color.gray;

        }
        else
        {
            boxOpenBtn.color = Color.white;
        }

        if (soundScript.isSoundOn)
            boxOpenBtn.GetComponent<AudioSource>().Play();

        RandomBoxBackground.gameObject.SetActive(false);
    }

    public void ClickExitRBPage()
    {
        startUI.gameObject.SetActive(true);
        openSettingBtn.gameObject.SetActive(true);
        MyStage.gameObject.SetActive(true);
        tapToStartBtn.gameObject.SetActive(true);
        if(gameManager.isHardModePlaying)
            HMText.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        UICamera.gameObject.SetActive(false);
        
        RandomBoxPage.gameObject.SetActive(false);
    }

    public void OpenNormalCarInspector()
    {
        float[] ip = { normalCar.engineLevel, normalCar.fuelLevel, normalCar.limitSpeed, normalCar.fuelRate };

        normalCarGauge[0].value = ip[0] * 0.01f;
        normalCarGauge[1].value = ip[1] * 0.01f;
        normalCarGauge[2].value = ip[2] * 0.01f;
        normalCarGauge[3].value = 1-(ip[3] - 0.5f);

        normalCarText[0].text = ip[0].ToString();
        normalCarText[1].text = ip[1].ToString();
        normalCarText[2].text = (Mathf.Round(ip[2]*10)*0.1f).ToString();
        normalCarText[3].text = (20-(Mathf.Round(ip[3] * 100)*0.1f)).ToString();

        normalCarInspector.gameObject.SetActive(true);
    }

    public void ClickCloseNCInspector()
    {
        normalCarInspector.gameObject.SetActive(false);
    }

    public void OpenSCInspector()
    {
        float[] ip = { (float)sportCar.engineLevel, (float)sportCar.fuelLevel, sportCar.limitSpeed, sportCar.fuelRate };

        sportCarGauge[0].value = ip[0] * 0.01f;
        sportCarGauge[1].value = ip[1] * 0.01f;
        sportCarGauge[2].value = ip[2] * 0.01f;
        sportCarGauge[3].value = 1 - (ip[3] - 0.5f);

        sportCarText[0].text = ip[0].ToString();
        sportCarText[1].text = ip[1].ToString();
        sportCarText[2].text = ip[2].ToString();
        sportCarText[3].text = (20 - (ip[3] * 10)).ToString();

        sportCarInspector.gameObject.SetActive(true);
    }

    public void ClickCloseSCInspector()
    {
        sportCarInspector.gameObject.SetActive(false);
    }

    public void ClickHomeBtn()
    {
        gameManager.OffSetting();
        if (player.isAction)
        {
            gameManager.WhenFail();
        }
        
    }

    public void ClickNOHMBtn()
    {
        NotOpenHM.gameObject.SetActive(false);
    }

    public void OpenRBInfo()
    {
        RBInfo.gameObject.SetActive(true);
    }
    public void CloseRBInfo()
    {
        RBInfo.gameObject.SetActive(false);
    }
}
