using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    public GameObject levelSelectorPanel;


    public void PlayGame(int indexScene) 
    {
        SceneManager.LoadScene(indexScene);
    }

    public void ShowOptions() 
    {
        optionsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void BackToMainMenu() 
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false); 
    }

    public void BackToMainMenuFromCredits()
    {
        mainPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void BackToMainMenuFromLevelSelector()
    {
        mainPanel.SetActive(true);
        levelSelectorPanel.SetActive(false);
    }

    public void ExitGame() 
    {
        Application.Quit();
    }
}
