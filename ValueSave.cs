using GooglePlayGames.BasicApi;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueSave : MonoBehaviour
{
    //[HideInInspector]
    public GameManager _gm;

    public Follower _flb;
    public Follower _fls;


    //[HideInInspector]
    public SceneMover _sm;
    //[HideInInspector]
    public SoundScript _SS;
   // [HideInInspector]
    public VibrateScript _vs;
    
    public bool isFirstTimePlay = true;
    bool firstTimeAfterUpdate0 = true;//업데이트 후 처음 플레이
    bool firstTimeAfterUpdate1 = true;
    // Start is called before the first frame update
    void Awake()
    {
        //이번 업데이트때 추가된 변수
        //hardModeIndex, isHardModeOverEnd, isHardModePlaying
        //기존 isOverPlayed 부울값 변경해야됨

        if (PlayerPrefs.HasKey("isFirstTimePlay") == true)
        {
            isFirstTimePlay = GetBool("isFirstTimePlay");
        }

        if(PlayerPrefs.HasKey("firstTimeAfterUpdate0") == true)
        {
            firstTimeAfterUpdate0 = GetBool("firstTimeAfterUpdate0");
        }

        if(PlayerPrefs.HasKey("firstTimeAfterUpdate1") == true)
        {
            firstTimeAfterUpdate1 = GetBool("firstTimeAfterUpdate1");
        }

        if(firstTimeAfterUpdate0)
        {
            if((GetValueFloat("fuelRates").Equals(0) || GetValueFloat("fuelRateb").Equals(0) || GetValueFloat("limitSpeeds").Equals(0) || GetValueFloat("limitSpeedb").Equals(0)))
            {
                SaveFloat("fuelRates",1.5f);
                SaveFloat("fuelRateb",1);
                SaveFloat("limitSpeeds", 40);
                SaveFloat("limitSpeedb", 30);
            }
            
            
            firstTimeAfterUpdate0 = false;
            SaveBool("firstTimeAfterUpdate0", firstTimeAfterUpdate0);

        }
        else
        {
            _fls.fuelRate = GetValueFloat("fuelRates");
            _fls.limitSpeed = GetValueFloat("limitSpeeds");
            _flb.fuelRate = GetValueFloat("fuelRateb");
            _flb.limitSpeed = GetValueFloat("limitSpeedb");
        }

        if(firstTimeAfterUpdate1)
        {
            _sm.isOverEnd = false;// 이거는 저장이아니라 basic,sportoverplayer에서 받아오는값

            _sm.hardModeIndex = 31;//나중에 변경
            SaveInt("hardModeIndex", _sm.hardModeIndex);

            _sm.isBasicCarOverPlayed = GetValueBool("isBasicCarOverPlayed");

            if (_sm.isBasicCarOverPlayed)
            {
                _sm.basicCarSceneIndex = 31;//한번 더 더해지는것은 아닌지 확인해야됨
                _sm.isBasicCarOverPlayed = false;
                SaveInt("basicCarSceneIndex", _sm.basicCarSceneIndex);
                SaveBool("isBasicCarOverPlayed", _sm.isBasicCarOverPlayed);
                _gm.isHardModeOpen = true;
                SaveBool("isHardModeOpen", _gm.isHardModeOpen);
            }

            _sm.isSportCarOverPlayed = GetValueBool("isSportCarOverPlayed");

            if (_sm.isSportCarOverPlayed)
            {
                _sm.SportCarSceneIndex = 31;
                _sm.isSportCarOverPlayed = false;
                SaveInt("SportCarSceneIndex", _sm.SportCarSceneIndex);
                SaveBool("isSportCarOverPlayed", _sm.isSportCarOverPlayed);
                _gm.isHardModeOpen = true;
                SaveBool("isHardModeOpen", _gm.isHardModeOpen);
            }
            
            firstTimeAfterUpdate1 = false;
            SaveBool("firstTimeAfterUpdate1", firstTimeAfterUpdate1);
            
        }
        else
        {
            _gm.isHardModeOpen = GetValueBool("isHardModeOpen");
            _sm.isHardModeOverEnd = GetValueBool("isHardModeOverEnd");
            _gm.isHardModePlaying = GetValueBool("isHardModePlaying");
            _sm.hardModeIndex = GetValueInt("hardModeIndex");
        }


        if (isFirstTimePlay == false)
        {
            _gm.isOpenSportCar = GetValueBool("isOpenSportCar");
            if (_gm.isOpenSportCar == true)
            {

            _sm.isSportCarOverPlayed = GetValueBool("isSportCarOverPlayed");
            _gm.isCurrentSportCar = GetValueBool("isCurrentSportCar");
            _fls.engineLevel = GetValueInt("engineLevels");
            _fls.fuelLevel = GetValueInt("fuelLevels");
                
                _fls.fuelRate = GetValueFloat("fuelRates");
                _fls.limitSpeed = GetValueFloat("limitSpeeds");
                
            _sm.SportCarSceneIndex = GetValueInt("SportCarSceneIndex");
            }
            _gm.isCurrentBasicCar = GetValueBool("isCurrentBasicCar");
            _sm.isBasicCarOverPlayed = GetValueBool("isBasicCarOverPlayed");
            _gm.gold = GetValueInt("gold");
            _gm.goldLevel = GetValueInt("goldLevel");
            _flb.engineLevel = GetValueInt("engineLevelb");         
            _flb.fuelLevel = GetValueInt("fuelLevelb");
            
                _flb.fuelRate = GetValueFloat("fuelRateb");
                _flb.limitSpeed = GetValueFloat("limitSpeedb");
            
            _sm.basicCarSceneIndex = GetValueInt("basicCarSceneIndex");
            _gm.InterstitialStack = GetValueInt("InterstitialStack");
            
            _SS.isSoundOn = GetValueBool("isSoundOn");
            _vs.isVibrateOn = GetValueBool("isVibrateOn");
            _gm.isPurchasedNoAD = GetValueBool("isPurchasedNoAD");
            _gm.isTutorial = GetValueBool("isTutorial");
            _gm.isClickRBRBtn = GetValueBool("isClickRBRBtn");
        }
        else
        {
            SaveBool("isCurrentBasicCar", true);
            SaveBool("isCurrentSportCar", false);
            SaveInt("SportCarSceneIndex", _sm.SportCarSceneIndex);
            SaveInt("basicCarSceneIndex", _sm.basicCarSceneIndex);            
            SaveInt("fuelLevelb", _flb.fuelLevel);
            SaveInt("fuelLevels", _fls.fuelLevel);
            SaveInt("engineLevelb", _flb.engineLevel);
            SaveInt("engineLevels", _fls.engineLevel);
            /*
            SaveFloat("fuelRates", _fls.fuelRate);
            SaveFloat("fuelRateb", _flb.fuelRate);
            SaveFloat("limitSpeeds", _fls.limitSpeed);
            SaveFloat("limitSpeedb", _flb.limitSpeed);
            */
            SaveInt("gold", _gm.gold);
            isFirstTimePlay = false;
            SaveBool("isFirstTimePlay" ,isFirstTimePlay);
            SaveBool("isPurchasedNoAD", _gm.isPurchasedNoAD);
            SaveBool("isSoundOn", true);
            SaveBool("isTutorial", true);
            SaveBool("isClickRBRBtn", false);
        }
        _sm.FirstSceneLoad();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void DoReset()
    {
        PlayerPrefs.DeleteAll();
    }
    
    public int GetValueInt(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            int a = PlayerPrefs.GetInt(key);
            return a;
        }
        else
        {
            return 0;
        }

    }

    public void SaveInt(string key, int val)
    {
        PlayerPrefs.SetInt(key, val);
        PlayerPrefs.Save();
    }


    public bool GetValueBool(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            bool a = GetBool(key);
            return a;
        }
        else
        {
            return false;
        }
    }

    public void SaveBool(string key, bool val)
    {
        SetBool(key, val);
        PlayerPrefs.Save();
    }

    public float GetValueFloat(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            float a = PlayerPrefs.GetFloat(key);
            return a;
        }
        else
        {
            return 0;
        }

    }

    public void SaveFloat(string key, float val)
    {
        PlayerPrefs.SetFloat(key, val);
        PlayerPrefs.Save();
    }

    public static void SetBool(string key, bool value)
    {
        if (value)
            PlayerPrefs.SetInt(key, 1);
        else
            PlayerPrefs.SetInt(key, 0);
    }

    public static bool GetBool(string key)
    {
        int tmp = PlayerPrefs.GetInt(key);
        if (tmp.Equals(1))
            return true;
        else
            return false;
    }
}
