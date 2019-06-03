using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDetector : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Constants.NumberTag) && GameManager.Instance.gameActive) {
            GameManager.Instance.OnGameOver();
            
        }
    }
}
