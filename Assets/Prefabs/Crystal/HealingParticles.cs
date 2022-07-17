using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingParticles : MonoBehaviour
{
    private enum State { stopping, starting, on, off }
    public bool Active {
        get => state == State.starting || state == State.on;
    }
    private State state = State.off;
    public float fadeRate = 1f;
    public AudioSource healingAudio;
    private ParticleSystem particles;
    Material[] materials;
    public float volumeSetting;
    private float transitionStartTime = 0f;

    void Awake()
    {
        materials = GetComponent<Renderer>().materials;
        particles = GetComponent<ParticleSystem>();
        healingAudio = GetComponent<AudioSource>();
        state = State.off;
        volumeSetting = healingAudio.volume;
    }

    public void EffectStart()
    {
        if (state == State.off || state == State.stopping)
        {
            healingAudio.Play();
            particles.Play();
            transitionStartTime = Time.time;
            state = State.starting;
        }
    }

    public void EffectStop()
    {
        if (state == State.on || state == State.starting)
        {
            transitionStartTime = Time.time;
            state = State.stopping;
        }
    }

    private void fadeOut()
    {
        float pctComplete = (Time.time - transitionStartTime) / fadeRate;
        foreach (Material material in materials)
        {
            Color color = material.GetColor("_TintColor");
            color.a = Mathf.Lerp(1, 0, pctComplete);
            material.SetColor("_TintColor", color);
        }
        healingAudio.volume = Mathf.Lerp(volumeSetting, 0, pctComplete);

        if (Mathf.Approximately(pctComplete, 1))
        {
            healingAudio.volume = 0;
            state = State.off;
        }
    }

    private void fadeIn()
    {
        float pctComplete = (Time.time - transitionStartTime) / fadeRate;
        foreach (Material material in materials)
        {
            Color color = material.GetColor("_TintColor");
            color.a = Mathf.Lerp(0, 1, pctComplete);
            material.SetColor("_TintColor", color);
        }
        healingAudio.volume = Mathf.Lerp(0, volumeSetting, pctComplete);

        if (Mathf.Approximately(pctComplete, 1))
        {
            healingAudio.volume = volumeSetting;
            state = State.on;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
        switch (state)
        {
            case State.stopping:
                fadeOut();
                break;
            case State.starting:
                fadeIn();
                break;
            case State.on:
                break;
            case State.off:
                particles.Stop();
                healingAudio.Stop();
                break;
        }
    }
}
