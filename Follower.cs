using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.SceneManagement;

public class Follower : MonoBehaviour
{

    Vector3 floor;
    public PathCreator PathCreator;
    public GameManager gameManager;
    public GameObject front;
    public GameObject back;
    public GameObject cameraPoint;
    public AudioSource[] audioSources;
    public GameObject breakLight;
    public GameObject RearSideLight;
    public ParticleSystem[] smoke;
    public ValueSave valueSave;

    public float speed;
    //public float savedSpeed;//저장된 속도
    public float speedWhenFinished;
    public float Distancetravelled;
    public float right;

    //private float resist = 0.01f;
    float gravityAngle;//지표면과 각도 
    float gravityRadian;//지표면과 라디안
    public float gravity;
    float RealGravity;
    public bool isAction = false;//값에 따라 해당 물체 정지 및 이전 속도 저장
    //int framerate = 750;

    public bool isClick;

    float FBDistance;

    public float fuelRate;

    public float engineForce;
    public float fullFuelCapacity;
    public float limitSpeed;
    public float resist;
    float NormalCarOriginalResist = 11.25f;// 노말카 레지스트
    float SportCarOriginalResist = 15;// 스포츠카 레지스트
    public int engineUpgreadCost = 50;
    public int fuelUpgreadCost = 50;
    public int engineLevel = 1;
    public int fuelLevel = 1;
    


    public float FuelAlpha = 750; // 기영 추가
    float SavedFuelAlpha = 300;
    public int GravityPlusAlpha = 400; // 기영 추가
    public int GravityMinusAlpha = 700; // 기영 추가
   // public float SportFuelRate = 1.5f; // 기영 추가, 스포츠카 연비 조절 변수
    //public float BasicFuelRate = 1f;
    //public float SavedSportFuelRate = 1.5f;

    int camDistanceWhenStart = 5;

    int soundSpeed = 3;


    private void Awake()
    {
        valueSave = FindObjectOfType<ValueSave>();
        FBDistance = front.transform.position.z - back.transform.position.z;

        floor = Vector3.up;

    }

    private void Start()
    {
        gameManager.CountTexiNum();

        UpdateCarAndCam();     

    }

    private void FixedUpdate()
    {
        if (Distancetravelled < 0)
        {
            speed = 0;
            Distancetravelled = 0;
        }
    }

    public IEnumerator StaticPos()
    {
        WaitForSeconds ws = new WaitForSeconds(0.5f);
        while (!gameManager.canMove)
        {
            speed = 0;
            yield return ws;
        }
        yield break;
    }
    
    public IEnumerator ResistForce()
    {
        while (gameManager.onResist)
        {
            if (0 < speed && speed <= 30)
            {
                speed -= resist * Time.deltaTime;// *  framerate;
            }

            else if (speed > 30)
            {

                speed -= resist * Time.deltaTime * 1.25f;

            }


            else if (speed < 0)
            {
                speed += resist * Time.deltaTime;// * framerate;

            }

            yield return null;
        }
        yield break;
    }


    public IEnumerator GravityEffect()
    {
        while (isAction)
        {
            gravityAngle = 90 - Vector3.Angle(transform.forward, floor);

            gravityRadian = gravityAngle * Mathf.Deg2Rad;

            RealGravity = -0.00981f * Mathf.Tan(gravityRadian) * gameManager.gravityCoefficient;

            gravity = -0.00981f * Mathf.Tan(gravityRadian) * gameManager.gravityCoefficient;

            if (gravity > 0 && speed < 50) // 내리막길에서 플러스 중력 제한
            {
                speed += gravity * Time.deltaTime * GravityPlusAlpha;
            }
            else if (!Input.GetMouseButton(0) && gravity < 0) // 터치안하면 뒤로 굴러감 (강한 중력 작용)
            {
                speed += gravity * Time.deltaTime * GravityMinusAlpha;

            }
            else if (Input.GetMouseButton(0) && gravity < 0 && speed > 10) // 뒤로가다가 엑셀 밟으면 초기에 마이너스 중력 제한
            {
                speed += gravity * Time.deltaTime * GravityMinusAlpha;

            }

            CarMove();
            
            yield return null;
        }

        yield break;
    }

    void CarMove()
    {
        Distancetravelled += speed * Time.deltaTime;

        //차의 앞바퀴와 뒷바퀴를의 위치 및 방향을 통해 움직임이 보다 자연스럽게
        back.transform.position = PathCreator.path.GetPointAtDistance(Distancetravelled + 0.1f, EndOfPathInstruction.Stop);

        front.transform.position = PathCreator.path.GetPointAtDistance(Distancetravelled + 0.1f + FBDistance, EndOfPathInstruction.Stop);

        transform.position = (back.transform.position + front.transform.position) * 0.5f;

        Vector3 arrow = front.transform.position - back.transform.position;

        transform.forward = arrow;

        if(SceneManager.GetActiveScene().buildIndex >= gameManager.indexWhenChangeLogic)
            transform.position += transform.right * right;
    }
    
    public IEnumerator PlayAudioSource()
    {
        while (gameManager.canMove)
        {
            if (Input.GetMouseButton(0) && !gameManager.isFuelEmpty)//출발
            {
                audioSources[0].pitch = Mathf.Lerp(audioSources[0].pitch, speed * 0.055f,Time.deltaTime);
            }
            else//대기
            {
                audioSources[0].pitch = Mathf.Lerp(audioSources[0].pitch, 1, Time.deltaTime);
            }


            yield return null;
        }

        yield break;
    }
    
    /*
    public IEnumerator PlayAudioSource()
    {
        while (gameManager.canMove)
        {
            if (Input.GetMouseButton(0) && !gameManager.isFuelEmpty)//출발
            {
                if (audioSources[0].volume > 0.05f)
                    audioSources[0].volume = Mathf.Lerp(audioSources[0].volume, 0, Time.deltaTime * soundSpeed);
                else
                    audioSources[0].volume = 0;

                if (audioSources[1].volume < 0.95f)
                    audioSources[1].volume = Mathf.Lerp(audioSources[1].volume, 1, Time.deltaTime * soundSpeed);
                else
                    audioSources[1].volume = 1;
            }
            else if (!gameManager.isFuelEmpty)//대기
            {
                if (audioSources[1].volume > 0.05f)
                    audioSources[1].volume = Mathf.Lerp(audioSources[1].volume, 0, Time.deltaTime * soundSpeed);
                else
                    audioSources[1].volume = 0;

                if (audioSources[0].volume < 0.95f)
                    audioSources[0].volume = Mathf.Lerp(audioSources[0].volume, 1, Time.deltaTime * soundSpeed);
                else
                    audioSources[0].volume = 1;
            }


            yield return null;
        }

        yield break;
    }
 */
    public void UpdateCarAndCam()
    {
        right = gameManager.RightCoefficient;
        CarMove();

        if (SceneManager.GetActiveScene().buildIndex >= gameManager.indexWhenChangeLogic && gameManager.isHardModePlaying)
        {
            
            for (int i = 0; i < gameManager.texiNum; i++)
            {
                gameManager.texi[i].Distancetravelled = (gameManager.mapLength/2)*i;
                gameManager.texi[i].FirstTexiPos();
            }
               
        }

        Camera.main.transform.position = cameraPoint.transform.position;
        Camera.main.transform.LookAt(transform.position);
        Camera.main.transform.position += transform.up * camDistanceWhenStart + transform.right * camDistanceWhenStart;
    } 


}
