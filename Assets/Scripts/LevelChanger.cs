using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public int indexScene;

    public void ChangeLevel()
    {
        SceneManager.LoadScene(indexScene);
    }
}
