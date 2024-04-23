using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridBehaviour : MonoBehaviour, IPointerClickHandler
{
    public Color lutColor;
    public AudioSource audioSource;
    public AudioSource masterAudioSource;
    public int id;

    private UnityEngine.UI.Image image;

    private float delayTimer;
    private float delayLimit = 1;
    private float originalDelayLimit = 1;

    void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
        if(image.color != Color.white)
        {
            delayTimer += Time.deltaTime;
        }

        if (delayTimer >= delayLimit)
            image.color = Color.white;

        //image.color = Color.Lerp(image.color, Color.white, Time.deltaTime);

        if (audioSource.isPlaying && masterAudioSource)
        {
            audioSource.timeSamples = masterAudioSource.timeSamples;
        }
    }

    public void Trigger()
    {
        image.color = lutColor;
        audioSource.PlayOneShot(audioSource.clip);
        delayTimer = 0;
    }    

    public void SetDelayLimit(float d)
    {
        delayLimit = d;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        delayLimit = originalDelayLimit;
        this.Trigger();
    }
}
