using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelector : MonoBehaviour
{
    public GameObject LevelSelectorPanel;

    public void PlayGame(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }
}
