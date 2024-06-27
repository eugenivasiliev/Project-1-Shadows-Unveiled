using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private RespawnPoint[] checkpoints;
    private int currentCheckpoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < checkpoints.Length; i++) checkpoints[i].Index = i;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) currentCheckpoint = (currentCheckpoint + 1) % checkpoints.Length;
    }

    public void SetCheckpoint(int newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    public Vector2 GetRespawnPoint()
    {
        return checkpoints[currentCheckpoint].transform.position;
    }
}
