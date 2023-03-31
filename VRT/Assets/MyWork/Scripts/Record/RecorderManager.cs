using UnityEngine;
using UnityEngine.UI;

public class RecorderManager : MonoBehaviour
{
    [Header("Replay Manager")]
    [SerializeField]
    private ReplayManager replayManager;
    [SerializeField]
    private Text sessions;

    // Start is called before the first frame update
    void Start()
    {
        replayManager = FindObjectOfType<ReplayManager>();
        if (replayManager != null)
            replayManager._SessionsText = sessions;
    }

    public void StartRecord()
    {
        replayManager.StartRecord();
    }
    public void StopRecord()
    {
        replayManager.StopRecord();

    }
    public void StartReplay()
    {
        replayManager.StartReplay();

    }
    public void StopReplay()
    {
        replayManager.StopReplay();

    }

}
