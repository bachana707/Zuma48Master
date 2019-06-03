using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class BallsPath : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.levelPath = GetComponent<PathCreator>();
        GetComponent<RoadMeshCreator>().CreatePath();
    }
}
