using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlocker : MonoBehaviour
{
    [Header("Unlocker Script Reference")]
    public UnlockLevels unlockLevels;

    [Space(20f)]


    [Header("Level Unlocker Configuration")]
    public string levelName;    
    
    private int _levelState;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _levelState = PlayerPrefs.GetInt(levelName);
        if (_levelState == 1)
        {
            _button.interactable = true;
        }
        else
        {
            _button.interactable = false;
        }   
        
    }
}
