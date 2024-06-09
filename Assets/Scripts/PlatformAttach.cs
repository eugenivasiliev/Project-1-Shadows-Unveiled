using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlatformAttach : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Movement otherMovement = other.gameObject.GetComponent<Movement>();
        if (otherMovement != null)
            otherMovement.standingPlatform = this.GetComponent<Rigidbody2D>();

        other.gameObject.transform.SetParent(transform);
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        Movement otherMovement = other.gameObject.GetComponent<Movement>();
        if (otherMovement != null)
            otherMovement.standingPlatform = null;

        other.gameObject.transform.SetParent(null);
    }
}
