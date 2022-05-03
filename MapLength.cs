using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;

public class MapLength : MonoBehaviour
{
    public ERRoadNetwork roadNetwork;
    public ERRoad road;
    // Start is called before the first frame update
    void Start()
    {
        mapLength();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void mapLength()
    {
        roadNetwork = new ERRoadNetwork();

        road = roadNetwork.GetRoadByName("road_0001");

        float roadLength = road.GetLength();

        Debug.Log(roadLength);

        Invoke("mapLength", 5);
    }
}
