﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidBase : MonoBehaviour
{
    [Header("Astroid base properties")]
    public float movementSpeed = 10.0f;

    public float shakeAmount = 5.0f;
    public float shakeDuration = 1.0f;
    //public Vector2 targetLocation;

    public GameObject[] onCollisionParticles = null;

    [Header("AudioSounds")]
    [SerializeField] private AudioClip[] m_TraveClips     = null;
    [SerializeField] private AudioClip[] m_ImpactClips    = null;
    private SoundEffectManager m_SoundEffectManager       = null;

    private Transform m_TravelLocation = null;
    private bool m_HasShaken = false;

    public void Init(Vector2 targetLocation)
    {
        //this.targetLocation = targetLocation;
        m_SoundEffectManager = GetComponent<SoundEffectManager>();
        m_TravelLocation = new GameObject().transform;
        m_TravelLocation.parent = transform;
        m_TravelLocation.position = targetLocation;

        var direction = new Vector3(m_TravelLocation.position.x, m_TravelLocation.position.y, 0) - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

        transform.GetChild(0).rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        AttemptPlaySound(m_TraveClips);
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, m_TravelLocation.position, movementSpeed*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        MonoBehaviour[] posibleDamagebales = other.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour posibleDamagebale in posibleDamagebales)
        {
            if (posibleDamagebale is IDamageable)
            {
                IDamageable damageable = (IDamageable) posibleDamagebale;
                damageable.TakeDamage(1);
            }
        }

        if (other.tag == "Enviorment")
        {
            if (!m_HasShaken)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShaker>().ShakeCamera(shakeDuration, shakeAmount);
                m_HasShaken = true;
                m_SoundEffectManager.StopAllAudio();
                AttemptPlaySound(m_ImpactClips);
                GetComponentInChildren<SpriteRenderer>().enabled = false;
                foreach (var particle in onCollisionParticles)
                {
                    GameObject particleObject = Instantiate(particle, transform.position, Quaternion.identity);
                }
                GameManager.Instance.InitiatePanic();
                Destroy(gameObject, 5.0f);
            }
        }
    }

    public void AttemptPlaySound(AudioClip[] audioClips)
    {
        if (audioClips.Length == 0 || audioClips == null)
        {
            Debug.Log("Attempted sound array is empty");
            return;
        }

        m_SoundEffectManager.PlaySound(audioClips);
    }
}
