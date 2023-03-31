using UnityEngine;
using System.Collections;

public class SingleMicrophoneCapture : MonoBehaviour
{ 
    private bool micConnected = false;
       
    private int minFreq;
    private int maxFreq;
      
    private AudioSource goAudioSource;
      
    void Start()
    { 
        if (Microphone.devices.Length <= 0)
        { 
            Debug.LogWarning("Microphone not connected!");
        }
        else  
        {   
            micConnected = true; 
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq); 
            if (minFreq == 0 && maxFreq == 0)
            { 
                maxFreq = 44100;
            } 
            goAudioSource = this.GetComponent<AudioSource>();
        }
    }

    public void StartVoiceRecord()
    {
        if (micConnected)
        {
            if (!Microphone.IsRecording(null))
            { 
                goAudioSource.clip = Microphone.Start(null, true, 20, maxFreq); 
            }  
        } 
    }

    public void StopVoiceRecord()
    {  
        Microphone.End(null);
    }

    public void PlayReplayVoice()
    {
        goAudioSource.Play();  
    }
   
}