using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Texi : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject front;
    public GameObject back;
    public PathCreator PathCreator;
    public Coroutine beforeCoroutine;
    float FBDistance;
    public float Distancetravelled;
    public float speed;
    public float right;
    float speedCoefficient;
    public float rightCoefficient;
    WaitForSeconds ws;

    private void Awake()
    {
        FBDistance = front.transform.position.z - back.transform.position.z;
        speedCoefficient = speed;
        ws = new WaitForSeconds(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            gameManager.fuelCapacity = 0;


            if (gameManager.UImanager.soundScript.isSoundOn)
                StartCoroutine(AudioChange());

            gameManager.WhenFail();
        }
        

    }

    IEnumerator AudioChange()
    {
        AudioSource audio = GetComponent<AudioSource>();

        audio.Play();
        audio.volume = 1.0f;

        while (audio.volume >= 0.01)
        {
            audio.volume = Mathf.Lerp(audio.volume, 0, Time.deltaTime * 0.5f);
            yield return null;
        }

        audio.Stop();
    }

    public void FirstTexiPos()
    {
        //차의 앞바퀴와 뒷바퀴를의 위치 및 방향을 통해 움직임이 보다 자연스럽게
        back.transform.position = PathCreator.path.GetPointAtDistance(Distancetravelled + 0.1f, EndOfPathInstruction.Stop);

        front.transform.position = PathCreator.path.GetPointAtDistance(Distancetravelled + 0.1f + FBDistance, EndOfPathInstruction.Stop);

        transform.position = (back.transform.position + front.transform.position) * 0.5f;

        Vector3 arrow = front.transform.position - back.transform.position;

        speed = speedCoefficient;

        transform.forward = arrow;

        right = rightCoefficient;

        transform.position += transform.right * right;
    }


    public IEnumerator TexiMove()
    {
        while (gameManager.isTexiMove)
        {

            Distancetravelled += speed * Time.deltaTime;


            //차의 앞바퀴와 뒷바퀴를의 위치 및 방향을 통해 움직임이 보다 자연스럽게
            back.transform.position = PathCreator.path.GetPointAtDistance(Distancetravelled + 0.1f, EndOfPathInstruction.Stop);

            front.transform.position = PathCreator.path.GetPointAtDistance(Distancetravelled + 0.1f + FBDistance, EndOfPathInstruction.Stop);

            float lengthToCen = Distancetravelled + 0.1f + FBDistance * 0.5f;

            transform.position = (back.transform.position + front.transform.position) * 0.5f;

            if(lengthToCen >= gameManager.mapLength+20)
            {
                Distancetravelled = 10;
            }

            Vector3 arrow = front.transform.position - back.transform.position;

            

            transform.forward = arrow;

            transform.position += transform.right * right;

            yield return null;
        }

        yield break;
    }

    public IEnumerator crossMove()
    {
        
        int i = 0;
        float tmp = right;
        //int s = 3;

        yield return ws;
        yield return ws;

        while (gameManager.isTexiMove)
        {

            while ((right + tmp >= 0.01f || right + tmp <= -0.01f) && gameManager.isTexiMove)
            {
                right = Mathf.Lerp(right, -tmp, Time.deltaTime);
                yield return null;
            }
                
            if(tmp > 0)
            {
                tmp = -1.5f;
            }
            else
            {
                tmp = 1.5f;
            }
            

            yield return ws;
        }

        yield break;
    }

    public void WhenTrigger(int setSpeed)
    {       
        if (beforeCoroutine != null)
            StopCoroutine(beforeCoroutine);

        beforeCoroutine = StartCoroutine(LerpTexiSpeed(setSpeed));
    }

    public IEnumerator LerpTexiSpeed(int setSpeed)
    {

        while ((speed - setSpeed >= 0.1f || speed - setSpeed <= -0.1f) && gameManager.isTexiMove)
        {
            speed = Mathf.Lerp(speed, setSpeed, Time.deltaTime);
            yield return null;
        }


        yield break;
    }
}
