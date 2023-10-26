using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public string tagTarget = "Player";
    public List<Collider2D> detectedObjs = new List<Collider2D>();
    public Collider2D col;

    // Detect when object enters range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == tagTarget)
        {
            detectedObjs.Add(collision);
        }
    }

    // Detect when object leaves range
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == tagTarget)
        {
            if(detectedObjs.Count > 0)
            {
                detectedObjs.Remove(collision);
            }
            else 
            {
                Debug.LogWarning("List object is empty");
            }
        }
    }
}
