using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] AudioClip runClip;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip dashClip;
    [SerializeField] AudioClip changeClip;
    [SerializeField] AudioClip wallJumpClip;
    [SerializeField] AudioClip doubleJumpClip;
    private bool hasDoubleJumped;
    private bool hasWallJumped;


    public bool IsFalling { get; private set; }

    [Header("Custom Animator Controllers")]
    public RuntimeAnimatorController lightController;
    public RuntimeAnimatorController darkController;

    [Space(20f)]
    [Header("Color Settings")]
    public Material mainMaterial;
    public Color materialColor;

    public Color lightColor;
    public Color darkColor;
    
    [Range(-10f, 10f)] public float intensity;

    private SpriteRenderer _sr;
    private Animator _animator;
    private Rigidbody2D _rb;
    private Movement _movement;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<Movement>();
    }

    void Start()
    {
        materialColor = lightColor;
    }

    private void OnEnable()
    {
        InputManager.changeElement += ChangeElementTrigger;
    }

    private void OnDisable()
    {
        InputManager.changeElement -= ChangeElementTrigger;
    }

    private void Update()
    {
        mainMaterial.color = new Color(materialColor.r * intensity, materialColor.g * intensity, materialColor.b * intensity, materialColor.a);

        #region MOVEMENT

        _animator.SetBool("IsRunning", Mathf.Abs(_rb.velocity.x) > 0.05f);

        IsFalling = _movement.LastOnGroundTime < 0;

        _animator.SetBool("IsFalling", _rb.velocity.y < -0.05f);

        #endregion

        #region AUDIO
        if (_movement.LastOnGroundTime >= 0)
        {
            hasDoubleJumped = false;
            hasWallJumped = false;
        }
        if (Mathf.Abs(_rb.velocity.x) <= 0.05f && source.clip == runClip) source.Stop();
        if (_movement.IsWallJumping && !hasWallJumped)
        {
            PlayClip(wallJumpClip, false, 0.2f);
            hasWallJumped = true;
        }
        if (_movement.IsDoubleJumping && !hasDoubleJumped)
        {
            PlayClip(doubleJumpClip, false, 0.2f);
            hasDoubleJumped = true;
        }
        if (!IsFalling && Mathf.Abs(_rb.velocity.x) > 0.05f && !source.isPlaying) PlayClip(runClip, true, 0.1f);
        #endregion

    }

    public void JumpAnimationTrigger()
    {
        PlayClip(jumpClip, false, 0.2f);
        _animator.SetTrigger("jump");
    }

    public void DashAnimationTrigger(int state)
    {
        PlayClip(dashClip, false, 0.2f);
        _animator.SetInteger("dashState", state);
    }

    public void ChangeElementTrigger()
    {
        PlayClip(changeClip, false, 0.2f);
        _animator.SetTrigger("changeMode");
    }

    public void ChangeLayerWeight()
    {
        if (_animator.runtimeAnimatorController.ToString() == lightController.ToString())
        {
            materialColor = darkColor;
            _animator.runtimeAnimatorController = darkController;
        }
        else
        {
            materialColor = lightColor;
            _animator.runtimeAnimatorController = lightController;
        }
        
    }

    private void PlayClip(AudioClip clip, bool loop, float volume)
    {
        source.Stop();
        source.clip = clip;
        source.loop = loop;
        source.volume = volume;
        source.Play();
    }
}
