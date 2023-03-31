using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public RoomList ListManager;

    public Text RoomNameText; 
    public Text PlayerCountText;
    public Text OpenText;
    public Button JoinButton;
    public LayoutElement LayoutElement;

    public RoomInfo info;


    public void RefreshInfo(RoomInfo info)
    {
            this.info = info;

        
        RoomNameText.text = info.Name;
        //RoomTypeText.text = info.CustomProperties["C0"].ToString();
        PlayerCountText.text = info.PlayerCount + "/" + info.MaxPlayers;
        if (info.IsOpen)
        {
            OpenText.text = "Open";
            OpenText.color = Color.green;
            JoinButton.interactable = true; 
        }
        else
        {
            OpenText.text = "Closed";
            OpenText.color = Color.red;
            JoinButton.interactable = false;
        }

    }

    public void OnJoinRoomButtonClick()
    {
        ListManager.OnRoomCellJoinButtonClick(info.Name);
    } 
    public void AddToList(RoomInfo info)
    {
        RefreshInfo(info); 
        LayoutElement.minHeight = 30f; 
    }

    public void RemoveFromList()
    {
        Destroy(this.gameObject); 
    } 
}
