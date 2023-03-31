using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recordable : MonoBehaviour
{
    [SerializeField]
    private bool _IsRecordable = false;
    [SerializeField]
    private bool _Grabed = false;
  
    public double[] _RecordStartTimes;  
    public List<List<ActionReplayRecord>> _RecordsActionReplay = new List<List<ActionReplayRecord>>(); 


    public bool IsRecordable { get => _IsRecordable; set => _IsRecordable = value; } 


    ReplayManager replayManager;

    bool isSleep = false;
    bool unGrabed = false;
    bool grabed = false;

    double stopTime = 0;
    Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        isSleep = rigidbody.IsSleeping();

        if (isSleep && unGrabed)
        {
            stopTime += Time.deltaTime; 
            if(stopTime >= 1.0)
            {
                StopRecord(); 
            }
        }
    }

    /// <summary>
    /// 
    /// **** on grab object 
    /// - find the replay manager 
    /// - if it found thats mean the player is actor else the player is audience
    /// - then call StartRecordOnGrab with Recordable parameter
    /// 
    /// </summary>
    public void OnGrabed() 
    {
        grabed = true;
        unGrabed = false;

        if (GetComponent<Rigidbody>().IsSleeping())
        {
            replayManager = FindObjectOfType<ReplayManager>();
            if (replayManager != null)
                replayManager.StartRecordOnGrab(this);
        }
    }

    /// <summary>
    /// 
    /// when UnGrab stop the record after 5s 
    /// 
    /// </summary>
    public void OnUnGrabed()
    {
        stopTime = 0;
        unGrabed = true;
        grabed = false; 
    }

    void StopRecord()
    {
        ActionReplay actionReplay = GetComponent<ActionReplay>();
        if (actionReplay != null)
        {  
            _RecordsActionReplay.Add(actionReplay.actionReplayRecords);
            for (int i = 0; i < _RecordsActionReplay.Count; i++)
            {
                Debug.Log("dfbfdbdfb   " + i + "  dfbfdbdfb" +   _RecordsActionReplay[i]);
                for (int y = 0; y < _RecordsActionReplay[i].Count; y++)
                {
                    Debug.Log("Position   " + _RecordsActionReplay[i][y].position + " Rotation" + _RecordsActionReplay[i][y].rotation);
                }
            }

            Destroy(actionReplay);
            isSleep = false;
            unGrabed = false;
            grabed = false;
        }
    }
}
