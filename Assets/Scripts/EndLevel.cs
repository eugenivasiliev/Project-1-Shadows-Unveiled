using System;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public string levelUnlocker;

    public static Action onLevelEnd = delegate { };
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerPrefs.SetInt(levelUnlocker, 1);

        onLevelEnd.Invoke();
    }
}
