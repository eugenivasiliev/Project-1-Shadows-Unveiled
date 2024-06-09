using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerMovementData currentDataMovement;
    
    public PlayerMovementData lightDataMovement;
    public PlayerMovementData darkDataMovement;

    private void Awake()
    {
        currentDataMovement = lightDataMovement;
    }
}
