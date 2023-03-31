using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public PlayerList ListManager;
    public ConnectionManager connectionManager;

    public Text NumberText;
    public Text _PlayerType;
    public Text NameText;

    public Text isMineText;
    public Image isMasterFlag;

    public LayoutElement LayoutElement;

    Player _player;

    public bool isInactiveCache;


    public override void OnEnable()
    {
        base.OnEnable(); 

        PlayerIndexHandler.OnPlayerIndexChanged += OnPlayerIndexChanged;
    }

    public override void OnDisable()
    {
        base.OnDisable(); // as this inherits from MonoBehaviourPunCallbacks, we need to call base
        PlayerIndexHandler.OnPlayerIndexChanged -= OnPlayerIndexChanged;
    }


    public void RefreshInfo(ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateInfo();
    }

    public void AddToList(Player info)
    {
        //Debug.Log("AddToList " + info.ToStringFull());

        _player = info;

        UpdateInfo();
 
        LayoutElement.minHeight = 30f; 
    }

    public void RemoveFromList()
    { 
        Destroy(this.gameObject);

    }

    public void OnClick()
    {
        //ListManager.SelectPlayer(_player);
    }


    void UpdateInfo()
    {
        if (string.IsNullOrEmpty(_player.NickName))
        {
            NameText.text = _player.ActorNumber.ToString();
        }

        int _index = _player.GetPlayerNumber();
        NumberText.text = "#" + _index.ToString("00");

        // set player Type "Actor / Audience"
        string res = _player.NickName.Substring(0, 2);
        if(res == "AC") // Actor
        {
            _PlayerType.text = "Actor"; 
        }
        else if(res == "AU") // Audience
        {
            _PlayerType.text = "Audience"; 
        }

        // set player name
        string playerName = _player.NickName;
        playerName = playerName.Substring(2, playerName.Length - 2); 
        NameText.text = playerName;

        isMineText.gameObject.SetActive(_player.IsLocal); 

        isMasterFlag.gameObject.SetActive(_player.IsMasterClient);


        // reorder the list to match player number
        if (_index >= 0 && this.transform.GetSiblingIndex() != _index)
        {
            this.transform.SetSiblingIndex(_index + 1);
        }
    }


    
    private void OnPlayerIndexChanged()
    {
        if (_player != null)
        { // we might be called before player is setup
            Debug.Log("#" + _player.GetPlayerNumber().ToString("00"));
        }

    }

}
