using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    // Clips de sonido y AudioSource para cada clase de sonido
    [Header("Audios Sources")]
    public AudioSource footstepLightSource;
    public AudioSource footstepDarkSource;
    public AudioSource jumpSource;
    public AudioSource fallWaterSource;
    public AudioSource dashDarkSource;
    public AudioSource dashLightSource;
    public AudioSource changeSoundSource;
    public AudioSource breakWallSoundSource;
    public AudioSource dieSoundSource;

    [Header("Audio Clips")]
    public AudioClip[] footstepLightClips;
    public AudioClip[] footstepDarkClips;
    public AudioClip jumpClip;
    public AudioClip fallWaterClip;
    public AudioClip dashDarkClip;
    public AudioClip dashLightClip;
    public AudioClip changeSoundClip;
    public AudioClip breakWallSoundClip;
    public AudioClip dieSoundClip;

    private bool lightMode = false;
    //Footsteps Light variable------------------------------------------------------------------
    public float footstepLightDelay = 0.5f;
    public float volumeLightFootsteps = 1.0f; // Volumen del sonido
    private bool isWalkingLight = false;
    private int currentLightClipIndex = 0; // Índice del clip de paso actual
    //Footsteps Dark variable-------------------------------------------------------------------
    public float footstepDarkDelay = 0.5f;
    public float volumeDarkFootsteps = 2.0f; // Volumen del sonido
    private bool isWalkingDark = false;
    private int currentDarkClipIndex = 0; // Índice del clip de paso actual
    //Jump variable-----------------------------------------------------------------------------
    private Animator animator;
    private bool isFalling = false;
    //Dash dark variable------------------------------------------------------------------------
    public float volumeDashDark = 2.0f; // Volumen del sonido
    public float pitchMultiplierDark = 2.0f; // Multiplicador de pitch para ajustar la velocidad del sonido de dash
    //Dash light variable-----------------------------------------------------------------------
    public float volumeDashLight = 1.0f; // Volumen del sonido
    public float pitchMultiplierLight = 1.0f; // Multiplicador de pitch para ajustar la velocidad del sonido de dash

    void Start()
    {
        // Carga los clips de audio desde la carpeta "Footsteps" dentro de "SFX"
        LoadLightFootstepClips();
        LoadDarkFootstepClips();
        animator = GetComponent<Animator>();
        jumpSource = GetComponent<AudioSource>();
        jumpSource.clip = jumpClip;
    }
    

    // Métodos para ejecutar acciones en las clases de sonido
    //Footsteps Light sound----------------------------------------------------------------------
    private void LoadLightFootstepClips()
    {
        // Ruta relativa a la carpeta "Assets"
        string folderPath = "Audio/SFX/Footsteps";
        // Obtiene todos los archivos de audio en la carpeta
        string[] audioFiles = Directory.GetFiles(Application.dataPath + "/" + folderPath);

        // Crea un array para almacenar los clips de audio
        footstepLightClips = new AudioClip[audioFiles.Length];

        // Carga cada archivo de audio como un AudioClip
        for (int i = 0; i < audioFiles.Length; i++)
        {
            string filePath = "Assets/" + folderPath + "/" + Path.GetFileName(audioFiles[i]);
            footstepLightClips[i] = (AudioClip)Resources.Load(filePath, typeof(AudioClip));
        }
    }

    //Footsteps Dark sound----------------------------------------------------------------------
    private void LoadDarkFootstepClips()
    {
        // Ruta relativa a la carpeta "Assets"
        string folderPath = "Audio/SFX/Footsteps";
        // Obtiene todos los archivos de audio en la carpeta
        string[] audioFiles = Directory.GetFiles(Application.dataPath + "/" + folderPath);

        // Crea un array para almacenar los clips de audio
        footstepDarkClips = new AudioClip[audioFiles.Length];

        // Carga cada archivo de audio como un AudioClip
        for (int i = 0; i < audioFiles.Length; i++)
        {
            string filePath = "Assets/" + folderPath + "/" + Path.GetFileName(audioFiles[i]);
            footstepDarkClips[i] = (AudioClip)Resources.Load(filePath, typeof(AudioClip));
        }
    }

    //PLAY SOUNDS--------------------------------------------------------------------------------
    //Footsteps Light--------------------------------------
    public void PlayLightFootsteps()
    {
        if (!footstepLightSource.isPlaying && isWalkingLight)
        {
            AudioClip footstepClip = footstepLightClips[currentLightClipIndex]; // Escoge un clip aleatorio de pasos
            footstepLightSource.clip = footstepClip;
            footstepLightSource.volume = volumeLightFootsteps; // Ajusta el volumen del AudioSource
            footstepLightSource.Play();
            currentLightClipIndex = (currentLightClipIndex + 1) % footstepLightClips.Length;
            Invoke("PlayFootsteps", footstepLightDelay); // Programa el próximo paso después del retardo
        }
    }

    public void StopLightFootsteps()
    {
        CancelInvoke("PlayFootsteps"); // Cancela cualquier paso programado
        footstepLightSource.Stop();
    }

    //Footsteps Dark---------------------------------------
    public void PlayDarkFootsteps()
    {
        if (!footstepDarkSource.isPlaying && isWalkingDark)
        {
            AudioClip footstepClip = footstepDarkClips[currentDarkClipIndex]; // Escoge un clip aleatorio de pasos
            footstepDarkSource.clip = footstepClip;
            footstepDarkSource.volume = volumeDarkFootsteps; // Ajusta el volumen del AudioSource
            footstepDarkSource.Play();
            currentDarkClipIndex = (currentDarkClipIndex + 1) % footstepDarkClips.Length;
            Invoke("PlayFootsteps", footstepDarkDelay); // Programa el próximo paso después del retardo
        }
    }

    public void StopDarkFootsteps()
    {
        CancelInvoke("PlayFootsteps"); // Cancela cualquier paso programado
        footstepDarkSource.Stop();
    }

    //jump--------------------------------------------------
    public void PlayJumpSound()
    {
        // Reproducir el sonido de salto solo si no se está cayendo
        if (!isFalling)
        {
            jumpSource.clip = jumpClip;
            jumpSource.Play();
        }
    }

    //Fall Water---------------------------------------------
    void OnTriggerEnter(Collider other)
    {
        // Verificar si el jugador ha entrado en el área de agua
        if (other.CompareTag("Player"))
        {
            PlayFallWaterSound();
        }
    }

    // Método para reproducir el sonido de caída en el agua
    public void PlayFallWaterSound()
    {
        if (fallWaterClip != null && fallWaterSource != null)
        {
            fallWaterSource.clip = fallWaterClip;
            fallWaterSource.Play();
        }
    }

    //Dash Dark---------------------------------------------
    public void PlayDashDarkSound()
    {
        if (dashDarkClip != null && dashDarkSource != null)
        {
            dashDarkSource.clip = dashDarkClip;
            dashDarkSource.volume = volumeDashDark; // Ajusta el volumen del AudioSource
            dashDarkSource.pitch = pitchMultiplierDark; // Ajusta el pitch del AudioSource
            dashDarkSource.Play();
        }
    }

    //Dash Light--------------------------------------------
    public void PlayDashLightSound()
        {
            if (dashLightClip != null && dashLightSource != null)
            {
                dashLightSource.clip = dashLightClip;
                dashLightSource.volume = volumeDashLight; // Ajusta el volumen del AudioSource
                dashLightSource.pitch = pitchMultiplierLight; // Ajusta el pitch del AudioSource
                dashLightSource.Play();
            }
    }

    //Change------------------------------------------------
    public void PlayChangeSound()
    {
        if (changeSoundClip != null && changeSoundSource != null)
        {
            changeSoundSource.clip = changeSoundClip;
            changeSoundSource.Play();
        }
    }

    //Break Wall--------------------------------------------
    public void PlayBreakWallSound()
    {
        if (breakWallSoundClip != null && breakWallSoundSource != null)
        {
            breakWallSoundSource.clip = breakWallSoundClip;
            breakWallSoundSource.Play();
        }
    }

    //Die---------------------------------------------------
    public void PlayDieSound()
    {
        if (dieSoundClip != null && dieSoundSource != null)
        {
            dieSoundSource.clip = dieSoundClip;
            dieSoundSource.Play();
        }
    }


    //UPDATE---------------------------------------------------------------------------------------
    private void Update()
    {
        //footsteps------------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (lightMode == true)
            {
                isWalkingLight = true;
                PlayLightFootsteps();
            }
            else
            {
                isWalkingDark = true;
                PlayDarkFootsteps();
            }
            
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            if (lightMode == true)
            {
                isWalkingLight = false;
                StopLightFootsteps();
            }
            else
            {
                isWalkingDark = false;
                StopDarkFootsteps();
            }
        }

        //jump-----------------------------------------------------------------
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("FallingAnim"))
        {
            // Si está cayendo y no se ha activado el sonido de caída, reproducir el sonido de caída
            if (!isFalling)
            {
                jumpSource.clip = jumpClip;
                jumpSource.Play();
                isFalling = true;
            }
        }
        else
        {
            // Restablecer el estado de caída si la animación de caída no está activa
            isFalling = false;
        }
    }
}