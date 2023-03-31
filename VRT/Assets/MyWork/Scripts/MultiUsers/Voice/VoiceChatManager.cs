using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatManager : MonoBehaviour
{
    [SerializeField]
    private PhotonVoiceNetwork punVoiceNetwork;
    public Recorder recorder; 

    [SerializeField]
    private Button _SoundOnOffBTN;

    [SerializeField]
    private Sprite _SoundOn;
    [SerializeField]
    private Sprite _SoundOff;

    public Text devicesInfoText;

    private void Awake()
    {
        punVoiceNetwork = PhotonVoiceNetwork.Instance;
        recorder = GameObject.FindObjectOfType<Recorder>();
    }

    private void Start()
    { 
        if (_SoundOnOffBTN != null)
        { 
            _SoundOnOffBTN.onClick.AddListener(this.VoiceSwitchOnClick);
        }   
        if (devicesInfoText != null)
        {
            if (UnityMicrophone.devices == null || UnityMicrophone.devices.Length == 0)
            { 
                Debug.Log( "No microphone device detected!");
            }
            else if (UnityMicrophone.devices.Length == 1)
            {
                Debug.Log(string.Format("Mic.: {0}", UnityMicrophone.devices[0])); 
            }
            else
            {
                Debug.Log(string.Format("Multi.Mic.Devices:\n0. {0} (active)\n", UnityMicrophone.devices[0]));
                for (int i = 1; i < UnityMicrophone.devices.Length; i++)
                {
                    Debug.Log(string.Concat(devicesInfoText.text, string.Format("{0}. {1}\n", i, UnityMicrophone.devices[i])));
                }
            }
        }

#if !UNITY_EDITOR && UNITY_PS4
            UserProfiles.LocalUsers localUsers = new UserProfiles.LocalUsers();
            UserProfiles.GetLocalUsers(localUsers);
            int userID = localUsers.LocalUsersIds[0].UserId.Id;

            punVoiceNetwork.PlayStationUserID = userID;
#elif !UNITY_EDITOR && UNITY_SHARLIN
            punVoiceNetwork.PlayStationUserID = egpvgetLocalUserID();
#endif

        _SoundOnOffBTN.interactable = true;
        _SoundOnOffBTN.gameObject.SetActive(true);
        devicesInfoText.gameObject.SetActive(false);
    } 

    private void OnEnable()
    { 
        punVoiceNetwork.Client.StateChanged += VoiceClientStateChanged; 
    }

    private void OnDisable()
    { 
        punVoiceNetwork.Client.StateChanged -= VoiceClientStateChanged; 
    } 

    private void VoiceSwitchOnClick()
    {
        if (punVoiceNetwork.ClientState == Photon.Realtime.ClientState.Joined)
        {
            punVoiceNetwork.Disconnect(); 
        }
        else if (punVoiceNetwork.ClientState == Photon.Realtime.ClientState.PeerCreated
                     || punVoiceNetwork.ClientState == Photon.Realtime.ClientState.Disconnected)
        {
            punVoiceNetwork.ConnectAndJoinRoom(); 
        }

    } 

    private void VoiceClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
    {
        UpdateUiBasedOnVoiceState(toState);
    }

    private void UpdateUiBasedOnVoiceState(Photon.Realtime.ClientState voiceClientState)
    {
        Debug.Log("PhotonVoice: {0}"+ voiceClientState); 
        switch (voiceClientState)
        {
            case Photon.Realtime.ClientState.Joined:
                devicesInfoText.gameObject.SetActive(false);
                _SoundOnOffBTN.gameObject.SetActive(true);
                _SoundOnOffBTN.image.sprite = _SoundOn;
                _SoundOnOffBTN.interactable = true;   
                if (recorder != null)
                {  
                    Debug.Log(recorder.VoiceDetectorCalibrating ? "Calibrating" : string.Format("Calibrate ({0}s)", 2000 / 1000));
                }
                else
                {
                    Debug.Log("Unavailable"); 
                }
                break;

            case Photon.Realtime.ClientState.PeerCreated: 
            case Photon.Realtime.ClientState.Disconnected:
                if (PhotonNetwork.InRoom)
                {
                    devicesInfoText.gameObject.SetActive(false);
                    _SoundOnOffBTN.interactable = true;
                    _SoundOnOffBTN.gameObject.SetActive(true);
                    _SoundOnOffBTN.image.sprite = _SoundOff;
                    Debug.Log("Voice Connect"); 
                }
                else
                {
                    devicesInfoText.gameObject.SetActive(true);
                    _SoundOnOffBTN.gameObject.SetActive(false);
                    _SoundOnOffBTN.interactable = false;
                    Debug.Log("Voice N/A"); 
                } 
                break;

            default:
                _SoundOnOffBTN.interactable = false;
                _SoundOnOffBTN.gameObject.SetActive(false);
                devicesInfoText.gameObject.SetActive(true);
                Debug.Log("Voice busy"); 
                break;
        }
    }
}
