using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AppData.DisableFirstRun();
            gameObject.SetActive(false);
        }
    }
}
