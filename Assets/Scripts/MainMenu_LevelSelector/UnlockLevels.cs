using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockLevels : MonoBehaviour
{
    public int level1Done;
    public int level2Done;
    public int level3Done;
    public int level4Done;
    

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Level1Done") == 0)
        {
            InitLevelsState();
        }
        else
        {
            LoadLevelsState();
        }
    }

    private void InitLevelsState()
    {
        PlayerPrefs.SetInt("Level1Done", 1);
        level1Done = PlayerPrefs.GetInt("Level1Done");

        level2Done = PlayerPrefs.GetInt("Level2Done");
        if (level2Done == 0)
        {
            PlayerPrefs.SetInt("Level2Done", 0);
        }

        level3Done = PlayerPrefs.GetInt("Level3Done");
        if (level3Done == 0)
        {
            PlayerPrefs.SetInt("Level3Done", 0);
        }

        level4Done = PlayerPrefs.GetInt("Level4Done");
        if (level4Done == 0)
        {
            PlayerPrefs.SetInt("Level4Done", 0);
        }
    }

    private void LoadLevelsState()
    {
        level1Done = PlayerPrefs.GetInt("Level1Done");
        level2Done = PlayerPrefs.GetInt("Level2Done");
        level3Done = PlayerPrefs.GetInt("Level3Done");
        level4Done = PlayerPrefs.GetInt("Level4Done");
    }
}
