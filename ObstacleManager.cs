using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> obstaclePrefabs;
    public List<Obstacle> obstacles;

    public GameManager gameManager;
    public Coroutine stopVelCoroutine = null;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        for(int i = 0; i < obstaclePrefabs.Count; i++)
        {
            Obstacle[] tmp = obstaclePrefabs[i].GetComponentsInChildren<Obstacle>();

            for(int j=0; j<tmp.Length;j++)
                obstacles.Add(tmp[j]);
        }

       
    }

    private void Start()
    {
        StartCoroutineSPO();
    }

    public void StopCoroutineSPO()
    {
        //AudioSource audio = GetComponent<AudioSource>();
        //audio.Play();
        if(stopVelCoroutine != null)
        {
            StopCoroutine(stopVelCoroutine);
            stopVelCoroutine = null;
        }
    }

    public void StartCoroutineSPO()
    {
        if (stopVelCoroutine == null)
        {
            stopVelCoroutine = StartCoroutine(StaticPosObstacle());
        }
    }

    public IEnumerator StaticPosObstacle()
    {


        while (!gameManager.isFail)
        {

            for(int i=0; i < obstacles.Count; i++)
            {
                obstacles[i].rigid.velocity = Vector3.zero;
                obstacles[i].transform.position = obstacles[i].pos;
                obstacles[i].transform.rotation = obstacles[i].rot;
            }

            yield return null;
        }

        yield break;
    }

   
}
