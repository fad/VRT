using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusReplay : MonoBehaviour
{
    public bool isInReplayMode;
    //private Rigidbody rigidbody;
    public List<StatusReplayRecord> statusReplayRecords = new List<StatusReplayRecord>();
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
                statusReplayRecords.Add(new StatusReplayRecord { _MishRendererStatus = transform.GetComponent<SkinnedMeshRenderer>().enabled});
            }

        }
        else if (isInReplayMode)
        {
            int nextIndex = currentReplayIndex + 1;

            if (nextIndex < statusReplayRecords.Count)
            {
                SetStatus(nextIndex);
            }
        }

    }


    public void SetStatus(int index)
    {
        currentReplayIndex = index;

        StatusReplayRecord statusReplayRecord = statusReplayRecords[index];

        transform.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = statusReplayRecord._MishRendererStatus; 
    }
}
