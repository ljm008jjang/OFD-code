using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position,Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 10,~9))
        {
            Debug.Log("t");
        }
        
    }
}
