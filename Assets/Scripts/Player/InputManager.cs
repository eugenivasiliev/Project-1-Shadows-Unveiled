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
            if (Input.GetKeyDown(KeyCode.X)) jumped.Invoke();
            if (Input.GetKeyUp(KeyCode.X)) jumping.Invoke();
            if (Input.GetKeyDown(KeyCode.Z)) dashed.Invoke();
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (Input.GetKeyDown(KeyCode.C) && (sceneIndex != 2 && sceneIndex != 3)) changeElement.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape)) onEscape.Invoke();
        }
    }
}

