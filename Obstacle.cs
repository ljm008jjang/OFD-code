using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 pos;
    public Quaternion rot;
    public Rigidbody rigid;
    GameManager gameManager = null;
    ObstacleManager obstacleManager;
    int radius = 10;
    int layerMask = 1 << 12;

    private void Awake()
    {
        pos = transform.position;
        rot = transform.rotation;
        rigid = GetComponent<Rigidbody>();

    }
    
    private void Start()
    {
        obstacleManager = GetComponentInParent<ObstacleManager>();
        gameManager = obstacleManager.gameManager;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            gameManager.WhenFail();

            gameManager.obstacleManager.StopCoroutineSPO();

            Collider[] obstacles = Physics.OverlapSphere(transform.position, 5, layerMask);

            for(int i=0; i < obstacles.Length; i++)
            {
                Rigidbody rigids = obstacles[i].GetComponent<Rigidbody>();

                rigids.AddExplosionForce(1000, other.transform.position, radius);
            }

            if (gameManager.UImanager.soundScript.isSoundOn)
                AudioStart();

            //other.gameObject.GetComponent<Follower>().gameManager.WhenFail();

        }
    }

    void AudioStart() {
        AudioSource audio = obstacleManager.GetComponent<AudioSource>();

        audio.Play();

    }
    public void ResetPos()
    {
        

        transform.position = pos;
        transform.rotation = rot;

        rigid.velocity = Vector3.zero;
    }
}
