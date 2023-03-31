using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class PlayerIndexHandler : MonoBehaviourPunCallbacks
{
    public static PlayerIndexHandler instance;
    public static Player[] SortedPlayers;

    public delegate void PlayerIndexChanged(); 
    public static event PlayerIndexChanged OnPlayerIndexChanged;
    public const string RoomPlayerIndexedProp = "pNr";
    public bool dontDestroyOnLoad = false;


    public void Awake()
    {

        if (instance != null && instance != this && instance.gameObject != null)
        {
            GameObject.DestroyImmediate(instance.gameObject);
        }

        instance = this;
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        RefreshData();
    }

    #region PunBehavior Overrides

    public override void OnJoinedRoom()
    {
        RefreshData();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Remove(RoomPlayerIndexedProp);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshData();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshData();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps != null && changedProps.ContainsKey(RoomPlayerIndexedProp))
        {
            RefreshData();
        }
    }

    #endregion
     
    public void RefreshData()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        if (PhotonNetwork.LocalPlayer.GetPlayerIndex() >= 0)
        {
            SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerIndex()).ToArray();
            if (OnPlayerIndexChanged != null)
            {
                OnPlayerIndexChanged();
            }
            return;
        } 
        HashSet<int> usedInts = new HashSet<int>();
        Player[] sorted = PhotonNetwork.PlayerList.OrderBy((p) => p.ActorNumber).ToArray();

        string allPlayers = "all players: ";
        foreach (Player player in sorted)
        {
            allPlayers += player.ActorNumber + "=pNr:" + player.GetPlayerIndex() + ", ";

            int number = player.GetPlayerIndex(); 

            if (player.IsLocal)
            {
                Debug.Log("PhotonNetwork.CurrentRoom.PlayerCount = " + PhotonNetwork.CurrentRoom.PlayerCount);
                 
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    if (!usedInts.Contains(i))
                    {
                        player.SetPlayerIndex(i);
                        break;
                    }
                } 
                break;
            }
            else
            {
                if (number < 0)
                {
                    break;
                }
                else
                {
                    usedInts.Add(number);
                }
            }
        } 
        SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerIndex()).ToArray();
        if (OnPlayerIndexChanged != null)
        {
            OnPlayerIndexChanged();
        }
    }
}

public static class PlayerIndexExtensions
{
    public static int GetPlayerIndex(this Player player)
    {
        if (player == null)
        {
            return -1;
        }

        if (PhotonNetwork.OfflineMode)
        {
            return 0;
        }
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            return -1;
        }

        object value;
        if (player.CustomProperties.TryGetValue(PlayerIndexHandler.RoomPlayerIndexedProp, out value))
        {
            return (byte)value;
        }
        return -1;
    }

    public static void SetPlayerIndex(this Player player, int playerIndex)
    {
        if (player == null)
        {
            return;
        }

        if (PhotonNetwork.OfflineMode)
        {
            return;
        }

        if (playerIndex < 0)
        {
            Debug.LogWarning("Setting invalid playerIndex: " + playerIndex + " for: " + player.ToStringFull());
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("SetPlayerIndex was called in state: " + PhotonNetwork.NetworkClientState + ". Not IsConnectedAndReady.");
            return;
        }

        int current = player.GetPlayerIndex();
        if (current != playerIndex)
        {
            Debug.Log("PlayerIndex: Set number " + playerIndex);
            player.SetCustomProperties(new Hashtable() { { PlayerIndexHandler.RoomPlayerIndexedProp, (byte)playerIndex } });
        }
    }
}

