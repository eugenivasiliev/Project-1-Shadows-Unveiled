using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public ParticleSystem destroyParticles;
    
    [SerializeField] private AudioSource source;
    private Movement _movement;
    private void OnCollisionStay2D(Collision2D other)
    {
        if (_movement == null)
        {
            _movement = other.gameObject.GetComponent<Movement>();
        }

        if (!_movement.IsOnLightElement())
        {
            if (_movement.IsDashing)
            {
                source.Play();
                destroyParticles.Play();
                Destroy(gameObject);
            }
        }
    }
}
