using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject cameraPoint;
    public float speed;
    public GameManager gameManager;
    public bool isDoingCameraMov = false;
    Vector3 playerRealPos;

    float basicDistance;
    int layermask = 1 << 9;

    private void Awake()
    {
        basicDistance = Vector3.Distance(cameraPoint.transform.position, gameManager.player.transform.position + gameManager.player.transform.up * 0.5f);
    }

    private void Start()
    {
        Camera.main.transform.position = cameraPoint.transform.position;
    }
    // Update is called once per frame
    void LateUpdate()
    {    
        if (gameManager.player.isAction == true)
        {
            playerRealPos = gameManager.player.transform.position + gameManager.player.transform.up * 0.5f;

            Ray ray = new Ray(playerRealPos, cameraPoint.transform.position - playerRealPos);

            if (Physics.Raycast(ray, out RaycastHit hit, basicDistance, layermask))
            {
                Vector3 newCamPoint = hit.point;

                transform.position = Vector3.Lerp(transform.position, newCamPoint, Time.deltaTime * speed);
            }else
            {
                transform.position = Vector3.Lerp(transform.position, cameraPoint.transform.position, Time.deltaTime * speed);
            }

            transform.LookAt(gameManager.player.transform);
        }
        
        else if(isDoingCameraMov == false && gameManager.player.isAction == false)
        {
            transform.position = Vector3.Lerp(transform.position, cameraPoint.transform.position, Time.deltaTime * speed * 0.5f);
        }
        else
        {
            transform.position = cameraPoint.transform.position;
            
            transform.LookAt(gameManager.player.transform);
        }
        

    }



}
