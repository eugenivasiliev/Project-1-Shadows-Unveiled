using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grip : MonoBehaviour
{
    public PlayerMovement movement;

    Rigidbody2D rb2D;               // se puede poner en ambos scripts???

    bool isTouchingFront = false;   //to know if we are touching a wall
    bool wallSliding;               //to know if we are sliding

    public float wallSlidinfSpeed = 0.5f;   //velocity sliding

    //substituir por una sola variable q compruebe todo??

    bool isTouchingRight;            //to know if we are touching a right wall zone
    bool isTouchingLeft;             //to know if we are touching a left wall zone

    void Start()
    {
        movement = FindAnyObjectByType<PlayerMovement>();
        


    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(movement._groundCheck);
        if (isTouchingFront == true && movement.Grounded == false) //averiguar como pasar el ISGROUND
            wallSliding = true;

        else
            wallSliding = false;

        if (wallSliding)
        {
            //lineas para la animacion--------------
            movement.rb2D.transform.position -= new Vector3(0, wallSlidinfSpeed * Time.deltaTime, 0);
            //Avoids fixing the x speed by directly affecting the y position
        }
    }

    //si hacemos solo de una varieble la comprobacion ELIMINAR funcion------------
    private void OnCollisionStay2D(Collision2D collision) //Check which wall is touching
    {
        if (collision.gameObject.CompareTag("ParedDerecha")) //Change tag name later
        {
            isTouchingFront = true;
            isTouchingRight = true;
        }
        if (collision.gameObject.CompareTag("ParedIzquierda")) //Change tag name later
        {
            isTouchingFront = true;
            isTouchingLeft = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision) //Check that we did not touch the wall
    {
        isTouchingFront = false;
        isTouchingRight = false;
        isTouchingLeft = false;
    }
}