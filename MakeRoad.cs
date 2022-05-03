using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;
using PathCreation;

public class MakeRoad : MonoBehaviour
{
    public List<Vector3> roadPos;
    //    public List<Quaternion> roadRotation;

    public ERRoadNetwork roadNetwork;
    public ERRoad road;
    public Destination destination;

    bool closedLoop = false;

    int currentElement = 0;

    public GameManager gameManager;

    int pointDistance = 0;

    public void CreateRoad()
    {
        roadNetwork = new ERRoadNetwork();

        road = roadNetwork.GetRoadByName("road_0001");

        float roadLength = road.GetLength();

        currentElement = 0;

        roadPos.Clear();
        
        pointDistance = 5;

        while (pointDistance < roadLength - 5)
        {
            roadPos.Add(road.GetPosition(pointDistance, ref currentElement));
            
            pointDistance += 2;
        }

        if (roadPos.Count > 0)
        {
            BezierPath bezierPath = new BezierPath(roadPos, closedLoop, PathSpace.xyz);

            gameManager.player.PathCreator.bezierPath = bezierPath;
        }
    }
    
    public void FixDestinationLocation()
    {
        destination.isExitDestination = false;

        destination.transform.position = gameManager.player.PathCreator.bezierPath.GetPoint(gameManager.player.PathCreator.bezierPath.NumPoints - 15);

        Vector3 arrow = new Vector3();

        arrow = gameManager.player.PathCreator.bezierPath.GetPoint(gameManager.player.PathCreator.bezierPath.NumPoints-10) - gameManager.player.PathCreator.bezierPath.GetPoint(gameManager.player.PathCreator.bezierPath.NumPoints - 15);

        arrow = new Vector3(arrow.x, 0, arrow.z);

        destination.transform.right = arrow;

        /*
        Debug.Log(Vector3.SignedAngle(Vector3.right, arrow , Vector3.right));

        Quaternion quaternion = new Quaternion();

        quaternion.eulerAngles = new Vector3(0, Vector3.SignedAngle(Vector3.right, arrow, Vector3.right), 0);

        destination.transform.rotation = quaternion;
        */

        //destination.transform.right = gameManager.player.PathCreator.bezierPath.GetPoint(gameManager.player.PathCreator.bezierPath.NumPoints - 14) - gameManager.player.PathCreator.bezierPath.GetPoint(gameManager.player.PathCreator.bezierPath.NumPoints - 15);

        //Vector3 vec = new Vector3(gameManager.player.PathCreator.bezierPath.GetAnchorNormalAngle(roadPos.Count - 10), destination.transform.rotation.y, destination.transform.rotation.z);

        //destination.transform.rotation = Quaternion.Euler(vec);
    }
    
}
