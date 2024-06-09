using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DashPointController : MonoBehaviour
{
    public Vector2 dashDirection;

    public Color inColor;
    public Color outColor;

    private Movement _playerMovement;
    
    private Light2D _lightPoint;

    void Awake()
    {
        _lightPoint = GetComponentInChildren<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _lightPoint.color = inColor;
        if (_playerMovement == null)
        {
            _playerMovement = other.GetComponent<Movement>();
        }
        
        if (_playerMovement.IsOnLightElement())
        {
            if (_playerMovement == null) return;
            
            _playerMovement.CanDashDirection = true;
            _playerMovement.DirectionToDash = dashDirection;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _lightPoint.color = outColor;
        if (_playerMovement == null)
        {
            _playerMovement = other.GetComponent<Movement>();
        }
        
        if (_playerMovement.IsOnLightElement())
        {
            if (_playerMovement == null) return;
        
            _playerMovement.CanDashDirection = false;
            _playerMovement.DirectionToDash = new Vector2(0f,0f);
        }
    }
}
