using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    public Transform Pos1, Pos2;
    public float speed;

    Transform origin, objective;
    
    Rigidbody2D _rb;

    // Start is called before the first frame update
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        origin = Pos1;
        objective = Pos2;
        SetVelocity();
    }

    private void FixedUpdate()
    {
        Vector3 selfToObj = objective.position - this.transform.position;
        Vector3 movementVec = (objective.position - origin.position);
        if (Vector2.Dot(selfToObj, movementVec) < 0)
        {
            Transform temp = objective;
            objective = origin;
            origin = temp;
            SetVelocity();
        }
    }

    void SetVelocity()
    {
        _rb.velocity = (objective.position - origin.position).normalized * speed;
    }
}
