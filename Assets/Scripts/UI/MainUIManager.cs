using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject inGamePanel;
    public GameObject inSettingsPanel;
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public Animator fadeAnimator;
    public int nextLevelIndex;
    
    public static bool PauseState;

    private void Start()
    {
        volumeSlider.value =  PlayerPrefs.GetFloat("volume");
    }

    private void OnEnable()
    {
        InputManager.onEscape += OnEscapeDone;
        EndLevel.onLevelEnd += EndLevelAnimation;
    }

    private void OnDisable()
    {
        InputManager.onEscape -= OnEscapeDone;
        EndLevel.onLevelEnd -= EndLevelAnimation;

    }

    void OnEscapeDone()
    {
        if (!PauseState)
        {
            PauseStateSettings();
        }
        else
        {
            ResumeStateSettings();
        }
    }

    void PauseStateSettings()
    {
        pausePanel.SetActive(true);
        inGamePanel.SetActive(false);
        Time.timeScale = 0f;

        PauseState = true;
    }

    public void ResumeStateSettings()
    {
        inGamePanel.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        PauseState = false;
    }

    public void EnterSettingsConfiguration()
    {
        inSettingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void ReturnSettingsConfiguration()
    {
        pausePanel.SetActive(true);
        inSettingsPanel.SetActive(false);
    }

    public void ExitGame(int indexScene)
    {
        Time.timeScale = 1f;
        PauseState = false;
        SceneManager.LoadScene(indexScene);
    }

    public void SetVolumePref()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void SetVolume()
    {
        float volume = volumeSlider.value;
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    public void EndLevelAnimation()
    {
        fadeAnimator.SetBool("LevelEnded", true);
    }
}
