using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static Action jumped = delegate {  };
    public static Action jumping;
    public static Action dashed;
    public static Action changeElement;
    
    public static Action onEscape;
    
    void Update()
    {
        if (!MainUIManager.PauseState)
        {
            if (Input.GetKeyDown(KeyCode.Space)) jumped.Invoke();
            if (Input.GetKeyUp(KeyCode.Space)) jumping.Invoke();
            if (Input.GetKeyDown(KeyCode.LeftShift)) dashed.Invoke();
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (Input.GetKeyDown(KeyCode.Q) && (sceneIndex != 2 && sceneIndex != 3)) changeElement.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape)) onEscape.Invoke();
        }
    }
}

