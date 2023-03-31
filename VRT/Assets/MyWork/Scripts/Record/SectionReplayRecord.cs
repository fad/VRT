using UnityEngine;

[System.Serializable]
public class ActionReplayRecord
{ 
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class StatusReplayRecord
{
    public bool _MishRendererStatus; 
}