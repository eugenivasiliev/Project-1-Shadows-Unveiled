using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTube : MonoBehaviour
{
    [SerializeField] private float strength;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb2D = collision.GetComponent<Rigidbody2D>();
        rb2D.AddForce(Vector2.up * strength);
    }
}
