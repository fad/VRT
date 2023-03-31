using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

[RequireComponent(typeof(AudioSource))]
public class ReplayManager : MonoBehaviour
{
    #region Movement Record Fields 

    [Header("Player & Player Clone")]
    public PlayerNetworkSetup _MasterPlayer;
    public PlayerInfo _MasterPlayerInfo;
    public GameObject _ClonePrefab;
    [Header("ActionReplay Objects")] // gameObjects that have ActionReplay Component
    public ActionReplay[] actionReplays;
    public ActionReplay[] cloneActionReplays; 
    
    [Header("Status Replay Objects")] // gameObjects that have StatusReplay Component
    public StatusReplay[] statusReplays;
    public StatusReplay[] cloneStatusReplays; 
     
    bool isInReplayMode;
    int currentReplayIndex;
    
    double _Timer = 0.0;
    bool isRecorderPlaying = false;


    [Header("UI Controller Buttons")]
    public Button _Record;
    public Button _StopRecord;
    public Button _Replay;
    public Button _StopReplay;

   public Text _SessionsText;
    #endregion

    #region Voice Record Fields
    private bool micConnected = false;

    private int minFreq;
    private int maxFreq;
    private float startRecordingTime;

    private AudioSource goAudioSource;
    private AudioClip goAudioClip;
    #endregion

    SpawnManager spawnManager;

    private void Start()
    {
        PlayerNetworkSetup []playerNetworkSetup = FindObjectsOfType<PlayerNetworkSetup>();
        spawnManager = FindObjectOfType<SpawnManager>();


        for (int i = 0; i < playerNetworkSetup.Length; i++)
        {
            playerNetworkSetup[i].SetUpTeleportProviders();
            playerNetworkSetup[i].SetupRemotePlayers();
        }
        
        for (int i = 0; i < playerNetworkSetup.Length; i++)
        {
            if (playerNetworkSetup[i].photonView.IsMine)
            {
                _MasterPlayer = playerNetworkSetup[i];
                _MasterPlayerInfo = playerNetworkSetup[i].GetComponent<PlayerInfo>();

                if (_MasterPlayerInfo.PlayerType == "Actor")
                {
                    _MasterPlayer.GetComponent<UISwitcher>().enabled = true;

                    actionReplays = _MasterPlayer.gameObject.GetComponentsInChildren<ActionReplay>();
                    statusReplays = _MasterPlayer.gameObject.GetComponentsInChildren<StatusReplay>();

                    _SessionsText = _MasterPlayer._SessionsText;

                    _Record = _MasterPlayer._Record;
                    _StopRecord = _MasterPlayer._StopRecord;
                    _Replay = _MasterPlayer._Replay;
                    _StopReplay = _MasterPlayer._StopReplay;


                    _Record.interactable = true;
                    _StopRecord.interactable = false;
                    _Replay.interactable = false;
                    _StopReplay.interactable = false;


                }
                //else
                //{
                //    _MasterPlayer.GetComponent<UISwitcher>().enabled = false;
                //}

            }
        } 

        //rigidbody = GetComponent<Rigidbody>();

      
        // record player real voice from Mic
        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Microphone not connected!");
        }
        else
        { 
            goAudioSource = this.GetComponent<AudioSource>();
        } 
    }

    private void Update()
    {
        _Timer += Time.deltaTime;
    }

    #region Movement Record & Replay

    public void StartRecord()
    {
        isRecorderPlaying = true;

        for (int i = 0; i < actionReplays.Length; i++)
        {
            _Record.interactable = false;
            _StopRecord.interactable = true;
            _Replay.interactable = false;
            _StopReplay.interactable = false;

            actionReplays[i].actionReplayRecords = new List<ActionReplayRecord>();
            actionReplays[i].IsRecord = true;
        }
        for (int i = 0; i < statusReplays.Length; i++)
        {
            statusReplays[i].statusReplayRecords = new List<StatusReplayRecord>();
            statusReplays[i].IsRecord = true;
        }
        StartVoiceRecord();
    }

    public void StartRecordOnGrab(Recordable grabedObject)
    {
        if (isRecorderPlaying)
        {
            if (grabedObject.IsRecordable)
            {
                Array.Resize(ref grabedObject._RecordStartTimes, grabedObject._RecordStartTimes.Length + 1);

                grabedObject._RecordStartTimes[grabedObject._RecordStartTimes.Length - 1] = _Timer;
                ActionReplay actionReplay = grabedObject.gameObject.AddComponent<ActionReplay>();
                actionReplay.IsRecord = true;
            }
        } 
    }

    public void StopRecord()
    {
        isRecorderPlaying = false; 

        for (int i = 0; i < actionReplays.Length; i++)
        {
            _Record.interactable = true;
            _StopRecord.interactable = false;
            _Replay.interactable = true;
            _StopReplay.interactable = false; 

            _SessionsText.text = "" + 01;

            actionReplays[i].IsRecord = false;
            actionReplays[i].hasRecords = true; 
        }
        for (int i = 0; i < statusReplays.Length; i++)
        {
            statusReplays[i].IsRecord = false;
            statusReplays[i].hasRecords = true;
        }

            StopVoiceRecord();
    } 
     

    GameObject clone;

    public void StartReplay()
    {
        clone = spawnManager.InstantiateReplayViaNetwork(_ClonePrefab);

        cloneActionReplays = clone.GetComponentsInChildren<ActionReplay>();
        cloneStatusReplays = clone.GetComponentsInChildren<StatusReplay>();

        for (int i = 0; i < cloneActionReplays.Length; i++)
        {
            cloneActionReplays[i].actionReplayRecords = actionReplays[i].actionReplayRecords;
        }
        for (int i = 0; i < cloneStatusReplays.Length; i++)
        {
            cloneStatusReplays[i].statusReplayRecords = statusReplays[i].statusReplayRecords;
        }


        for (int i = 0; i < cloneActionReplays.Length; i++)
        {
            _Record.interactable = false;
            _StopRecord.interactable = false;
            _Replay.interactable = false;
            _StopReplay.interactable = true;

            cloneActionReplays[i].isInReplayMode = true;
            isInReplayMode = true;

            cloneActionReplays[i].replay = true;
        }

        for (int i = 0; i < cloneStatusReplays.Length; i++)
        {
            cloneStatusReplays[i].isInReplayMode = true;
            isInReplayMode = true;

            cloneStatusReplays[i].replay = true;
        }

        PlayReplayVoice();
    }

    public void StopReplay()
    { 
        _Record.interactable = true;
        _StopRecord.interactable = false;
        _Replay.interactable = true;
        _StopReplay.interactable = false;
  
        PhotonNetwork.Destroy(clone);

        cloneActionReplays =new ActionReplay[0] ;
        cloneStatusReplays =new StatusReplay[0] ;

        StopReplayVoice();
    }
    #endregion

    #region Voice Record & Replay

    private void StartVoiceRecord()
    {
        //Get the max frequency of a microphone, if it's less than 44100 record at the max frequency, else record at 44100
        int minFreq;
        int maxFreq;
        int freq = 44100;
        Microphone.GetDeviceCaps("", out minFreq, out maxFreq);
        if (maxFreq < 44100)
            freq = maxFreq;

        //Start the recording, the length of 300 gives it a cap of 5 minutes
        goAudioClip = Microphone.Start("", false, 3559, 44100);
        startRecordingTime = Time.time;
    }

    private void StopVoiceRecord()
    {
        Microphone.End(""); 
    }

    private void PlayReplayVoice()
    {

        //End the recording when the mouse comes back up, then play it
        //Microphone.End("");

        //Trim the audioclip by the length of the recording
        AudioClip recordingNew = AudioClip.Create(goAudioClip.name,(int)((Time.time - startRecordingTime)
            * goAudioClip.frequency), goAudioClip.channels, goAudioClip.frequency, false);

        float[] data = new float[(int)((Time.time - startRecordingTime) * goAudioClip.frequency)];
        goAudioClip.GetData(data, 0);
        recordingNew.SetData(data, 0);
        this.goAudioClip = recordingNew;

        //Play recording
        goAudioSource.clip = goAudioClip;
        goAudioSource.Play();
    }

    private void StopReplayVoice()
    {  
        goAudioSource.Stop();
    }

    #endregion
}
