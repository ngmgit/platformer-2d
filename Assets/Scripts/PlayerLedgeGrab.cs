using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeGrab : MonoBehaviour {

    PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ledge Grab" && !playerMovement.CheckIfGrabCorner())
        {
            playerMovement.SetOnGrabStay();
        }
    }
}
