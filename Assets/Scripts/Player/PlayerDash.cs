using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private Rigidbody2D _rb2d;

    [SerializeField] private bool _canDash;
    [SerializeField] private float _dashCooldownTime;
    [SerializeField] private float _impuseMultiplier;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _canDash = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("UwU?");
            DashAction();
        }
    }

    private void DashAction()
    {
        if (_canDash)
        {
            Debug.Log("UwU :3");
            StartCoroutine(DashBehaviour());
        }
    }

    private IEnumerator DashBehaviour()
    {
        _canDash = false;
        _rb2d.AddForce(Vector2.right * _impuseMultiplier, ForceMode2D.Force);
        yield return new WaitForSeconds(_dashCooldownTime);
        _canDash = true;
    }
}
