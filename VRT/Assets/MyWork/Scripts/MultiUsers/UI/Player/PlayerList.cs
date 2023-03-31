using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerList : MonoBehaviourPunCallbacks
{  
    public PlayerListItem ItemPrototype;

    public Text PlayerCountsText;
    public Text _RoomName;

    public Text UpdateStatusText;

    Dictionary<int, PlayerListItem> playerListItem = new Dictionary<int, PlayerListItem>();

    bool _MasterSwitched = false;
    int _ActorsNum = 0;
    void Awake()
    {
        ItemPrototype.gameObject.SetActive(false);
    }

    public override void OnEnable()
    {

        base.OnEnable();

        UpdateStatusText.text = string.Empty;

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        RefreshCount();

        foreach (KeyValuePair<int, Player> _entry in PhotonNetwork.CurrentRoom.Players)
        {
            if (playerListItem.ContainsKey(_entry.Key))
            {
                continue;
            }

            //Debug.Log("PlayerListView:adding player " + _entry.Key);
            playerListItem[_entry.Key] = Instantiate(ItemPrototype);
            playerListItem[_entry.Key].transform.SetParent(ItemPrototype.transform.parent, false);
            playerListItem[_entry.Key].gameObject.SetActive(true);
            playerListItem[_entry.Key].AddToList(_entry.Value);
        }
    }

  
    public override void OnPlayerEnteredRoom(Player newPlayer)
    { 
        // we create the cell
        if (!playerListItem.ContainsKey(newPlayer.ActorNumber))
        {
            playerListItem[newPlayer.ActorNumber] = Instantiate(ItemPrototype.gameObject).GetComponent<PlayerListItem>();
            playerListItem[newPlayer.ActorNumber].transform.SetParent(ItemPrototype.transform.parent, false);
            playerListItem[newPlayer.ActorNumber].gameObject.SetActive(true); 
            playerListItem[newPlayer.ActorNumber].AddToList(newPlayer);
        }
        else // rejoin
        {
            playerListItem[newPlayer.ActorNumber].RefreshInfo(null);
        }


        StartCoroutine("UpdateUIPing");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        foreach (KeyValuePair<int, Player> _entry in PhotonNetwork.CurrentRoom.Players)
        {
            playerListItem[_entry.Key].RefreshInfo(null); 
        }

        Debug.Log("otherPlayer.IsMasterClient  " + newMasterClient.IsMasterClient); 

        foreach (KeyValuePair<int, Player> _entry in PhotonNetwork.CurrentRoom.Players)
        {
            string res = _entry.Value.NickName.Substring(0, 2);

            Debug.Log("client name" + _entry.Value.NickName);

            if (res == "AC") // Actor
            {
                Debug.Log("Actor Found");
                if (!_MasterSwitched)
                {
                    PhotonNetwork.SetMasterClient(_entry.Value);
                    _ActorsNum++;
                    _MasterSwitched = true;
                }
            }

        } 
        if (_ActorsNum == 0 & !_MasterSwitched)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.PlayerTtl = 0;
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = 0;
            PhotonNetwork.LeaveRoom(false);
            PhotonNetwork.Disconnect();
        }

        _ActorsNum = 0;
        _MasterSwitched = false;
        
        Debug.Log(" OnMasterClientSwitched");

    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (playerListItem.ContainsKey(target.ActorNumber))
        {
            playerListItem[target.ActorNumber].RefreshInfo(changedProps);
        }
        else
        {
            Debug.LogWarning("PlayerListView: missing Player Ui Cell for " + target, this);
        }

        StartCoroutine("UpdateUIPing");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       
        if (!PhotonNetwork.PlayerListOthers.Contains(otherPlayer))
        { 
            playerListItem[otherPlayer.ActorNumber].RemoveFromList();
            playerListItem.Remove(otherPlayer.ActorNumber);
            Debug.Log("on player left room");
        }
        else
        { 
            playerListItem[otherPlayer.ActorNumber].RefreshInfo(null);
        } 

        StartCoroutine("UpdateUIPing");
    }

    void RefreshCount()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PlayerCountsText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString("00");
            _RoomName.text = PhotonNetwork.CurrentRoom.Name;
        }

    }
   
    IEnumerator UpdateUIPing()
    {
        UpdateStatusText.text = "Updated";

        yield return new WaitForSeconds(1f);

        UpdateStatusText.text = string.Empty;
    }

    public void ResetList()
    {
        foreach (KeyValuePair<int, PlayerListItem> entry in playerListItem)
        {
            if (entry.Value != null)
            {
                Destroy(entry.Value.gameObject);
            }
        }

        playerListItem = new Dictionary<int, PlayerListItem>();
    }

}
