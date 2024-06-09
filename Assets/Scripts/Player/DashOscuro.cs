using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashOscuro : MonoBehaviour
{

    public LayerMask dashThroughLayers; 

    public float dashDistance = 5f;
    public float dashDuration = 0.2f; 
    public float dashCooldown = 1f; 
    private bool isDashing = false; 
    private bool canDash = true; 



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            Vector2 dashDirection = Vector2.zero;
            if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
            {
                dashDirection.x = Mathf.Sign(inputDirection.x);
            }
            else
            {
                dashDirection.y = Mathf.Sign(inputDirection.y);
            }

            StartCoroutine(PerformDash(dashDirection));
        }
    }

    IEnumerator PerformDash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;

        float dashTimer = 0f;

        while (dashTimer < dashDuration)
        {
            Vector2 dashAmount = direction * dashDistance * Time.deltaTime;


            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, dashDistance, dashThroughLayers);
            if (hit.collider != null)
            {

                dashAmount = direction * hit.distance;
            }

            transform.position += (Vector3)dashAmount;

            dashTimer += Time.deltaTime;

            yield return null; 
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}