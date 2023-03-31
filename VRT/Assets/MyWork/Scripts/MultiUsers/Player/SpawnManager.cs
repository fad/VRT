using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{  
    
    [Header("Player Types")]
    [SerializeField]
    private GameObject _ActorClonePrefab;
    [SerializeField]
    private GameObject _AudianceClonePrefab;

    [Header("remote Player positions")]
    [SerializeField]
    private Transform _LocalPlayerPosition;

    [Header("Player Info")]
    [SerializeField]
    private PlayerInfo _PlayerInfo;

    [Header("Room Panel ")]
    [SerializeField]
    private GameObject _Stage;
    [SerializeField]
    private GameObject _BackStage;

    [SerializeField]
    private GameObject _Player;

    public GameObject GetPlayer()
    {
        if(_Player != null)
            return _Player;

        return null;
    }

    public void InstantiateViaNetwork(string type)
    {
        if (type == "Actor")
        {
            _Player  = PhotonNetwork.Instantiate(_ActorClonePrefab.name, _LocalPlayerPosition.position,
                _LocalPlayerPosition.rotation);

            _PlayerInfo = _Player.GetComponent<PlayerInfo>(); 
            _PlayerInfo.PlayerType = "Actor";
            _PlayerInfo.PlayerName = PhotonNetwork.LocalPlayer.NickName;
            _PlayerInfo.PlayerId = PhotonNetwork.LocalPlayer.UserId;
        }
        else if (type == "Audience")
        { 
            _Player = PhotonNetwork.Instantiate(_AudianceClonePrefab.name, _LocalPlayerPosition.position,
               _LocalPlayerPosition.rotation);

            _PlayerInfo = _Player.GetComponent<PlayerInfo>();
            _PlayerInfo.PlayerType = "Audience";
            _PlayerInfo.PlayerName = PhotonNetwork.LocalPlayer.NickName;
            _PlayerInfo.PlayerId = PhotonNetwork.LocalPlayer.UserId;
        } 

        
    }

    public GameObject InstantiateReplayViaNetwork(GameObject clone)
    {
        

        return PhotonNetwork.Instantiate(clone.name, _LocalPlayerPosition.position,
                _LocalPlayerPosition.rotation);


    }

    #region Stage <--> backstage

    public void GoToStage_BackStage(Transform position)
    {
        _Player.transform.position = position.position; 
        _Player.transform.localRotation = position.localRotation; 
    } 

    #endregion
}
