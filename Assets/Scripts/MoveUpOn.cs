using System.Diagnostics;
using UnityEngine;

public class MoveUpOnTouch : MonoBehaviour
{
    public int counter = 0;


    // Wird aufgerufen, wenn ein anderes Objekt mit Collider dich berührt
    private void OnCollisionEnter(Collision collision)
    {
        // Optional: Nur reagieren, wenn der Spieler es berührt
        if (collision.gameObject.CompareTag("Player"))
        {
            counter++;

        }
    }

    void Update()
    {
        if (counter > 0)
        {
            transform.position += Vector3.up * Time.deltaTime;
        }
    }
}