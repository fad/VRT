using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RoomList : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class OnJoinRoomEvent : UnityEvent<string> { }
    public OnJoinRoomEvent OnJoinRoom;

    public RoomItem _PublicCellPrototype;
    public RoomItem _TrainingCellPrototype;

    public Text UpdateStatusText;
    public Text ContentFeedback;

    bool _firstUpdate = true;

    // room type
    [SerializeField]
    private int userType;
    public int UserType { get => userType; set => userType = value; }

    Dictionary<string, RoomItem> roomCellList = new Dictionary<string, RoomItem>();


    public override void OnEnable()
    {
        base.OnEnable();

        //GetRoomList();
        ResetList();

        _PublicCellPrototype.gameObject.SetActive(false);
        _TrainingCellPrototype.gameObject.SetActive(false);
        UpdateStatusText.text = string.Empty;
        ContentFeedback.text = string.Empty;
    }
    public void OnRoomCellJoinButtonClick(string roomName)
    {
        OnJoinRoom.Invoke(roomName);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate called");
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log(roomList[i].Name + "OnRoomListUpdate called");

        }
        UpdateStatusText.text = "Updated";

        if (roomList.Count == 0 && !PhotonNetwork.InLobby)
        {
            ContentFeedback.text = "No Room found in lobby ";
        }

        foreach (RoomInfo entry in roomList)
        {
            if (roomCellList.ContainsKey(entry.Name))
            {
                if (entry.RemovedFromList)
                {
                    // we delete the cell
                    roomCellList[entry.Name].RemoveFromList();
                    roomCellList.Remove(entry.Name);
                }
                else
                {
                    // we update the cell
                    roomCellList[entry.Name].RefreshInfo(entry);
                }

            }
            else
            {
                if (!entry.RemovedFromList)
                {
                    // we create the cell
                    AddToRoomList(entry);
                }
            }

            StartCoroutine("clearStatus");

            _firstUpdate = false;
        }

        void AddToRoomList(RoomInfo entry)
        {
            string res = entry.Name.Substring(0, 1);
            if (userType == 0) // actor
            { 
                if (res == "P")
                {
                    roomCellList[entry.Name] = Instantiate(_PublicCellPrototype);
                    roomCellList[entry.Name].gameObject.SetActive(true);
                    roomCellList[entry.Name].transform.SetParent(_PublicCellPrototype.transform.parent, false);
                    roomCellList[entry.Name].AddToList(entry);
                }
                else if (res == "T")
                {
                    roomCellList[entry.Name] = Instantiate(_TrainingCellPrototype);
                    roomCellList[entry.Name].gameObject.SetActive(true);
                    roomCellList[entry.Name].transform.SetParent(_PublicCellPrototype.transform.parent, false);
                    roomCellList[entry.Name].AddToList(entry);
                }
            }
            else if (userType == 1)
            {
                if (res == "P")
                {
                    roomCellList[entry.Name] = Instantiate(_PublicCellPrototype);
                    roomCellList[entry.Name].gameObject.SetActive(true);
                    roomCellList[entry.Name].transform.SetParent(_PublicCellPrototype.transform.parent, false);
                    roomCellList[entry.Name].AddToList(entry);
                }

            }

        }
    }

    IEnumerator clearStatus()
    {
        yield return new WaitForSeconds(1f);

        UpdateStatusText.text = string.Empty;
    }

    public void OnJoinedLobbyCallBack()
    {
        _firstUpdate = true;
        ContentFeedback.text = string.Empty;
    }

    public void GetRoomList()
    {
        ResetList();
        TypedLobby sqlLobby = new TypedLobby(null, LobbyType.Default);

        if (userType == 1) // audience
        {

            Debug.Log("GetCustomRoomList() matchmaking against '" + "MyLobby");


            PhotonNetwork.GetCustomRoomList(sqlLobby, "C0 = 'public'"); // "C0", "Hello To public" 
            ContentFeedback.text = "looking for Rooms in Lobby '" + "MyLobby" + "' Matching: '" + "C0 = 'public'";
        }
        else if (userType == 0) // actor
        {
            PhotonNetwork.GetCustomRoomList(sqlLobby, "C0 = 'Hello to training'"); // "C0", "Hello to training"   

            ContentFeedback.text = "looking for Rooms in Lobby '" + "MyLobby" + "' Matching: '" + "C0 = 'training'";

        }

    }

    public void ResetList()
    {
        _firstUpdate = true;
        foreach (KeyValuePair<string, RoomItem> entry in roomCellList)
        {

            if (entry.Value != null)
            {
                Destroy(entry.Value.gameObject);
            }

        }
        roomCellList = new Dictionary<string, RoomItem>();
    }
}
