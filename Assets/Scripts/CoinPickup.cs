using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int coinValue = 100;
    bool wasCollected = false;
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            FindObjectOfType<GameSession>().AddPoints(coinValue);
            Destroy(gameObject);
        }
    }
}
