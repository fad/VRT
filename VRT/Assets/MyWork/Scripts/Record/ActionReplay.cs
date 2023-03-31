using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionReplay : MonoBehaviour
{
    public bool isInReplayMode;
    //private Rigidbody rigidbody;
    public List<ActionReplayRecord> actionReplayRecords = new List<ActionReplayRecord>();
    public int currentReplayIndex;

    public bool IsRecord = false;
    public bool hasRecords = false;
    public bool replay = false;
  
    private void FixedUpdate()
    {
        if (IsRecord)
        {
            if (!isInReplayMode)
            { 
                actionReplayRecords.Add(new ActionReplayRecord {position = transform.position, rotation = transform.rotation });
            }

        }
        else if(isInReplayMode)
        {
            int nextIndex = currentReplayIndex + 1;

            if (nextIndex < actionReplayRecords.Count)
            { 
                SetTransform(nextIndex);
            }
        }
         
    }


    public void SetTransform(int index)
    {
        currentReplayIndex = index;

        ActionReplayRecord actionReplayRecord = actionReplayRecords[index];

        transform.position = actionReplayRecord.position;
        transform.rotation = actionReplayRecord.rotation;
    } 
}