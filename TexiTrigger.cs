using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexiTrigger : MonoBehaviour
{
    public int setSpeed;
    //float speed;
    Texi texi;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(11))
        {
            texi = other.GetComponent<Texi>();
            texi.WhenTrigger(setSpeed);
        }
    }
    /*
    IEnumerator LerpTexiSpeed()
    {
        
        while ((texi.speed - setSpeed >= 0.001f || texi.speed - setSpeed <= -0.001f) && !texi.gameManager.isFail)
        {
            Debug.Log(setSpeed);
            texi.speed = Mathf.Lerp(texi.speed, setSpeed, Time.deltaTime);
            yield return null;
        }


        yield break;
    }
    */
}
