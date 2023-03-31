using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [Header("Managers")]
    [SerializeField] 
    SpawnManager spawnManager;
    
    [Header("Local Player")]
    [SerializeField] 
    GameObject localPlayer;
    [SerializeField] 
    Camera _PlayerCamera;
    [SerializeField] 
    TeleportationArea[] teleportationArea;

    [Header("Main Panels")]
    [SerializeField]
    private GameObject connectionPanel;
    [SerializeField]
    private GameObject lobbyPanel ;
    [SerializeField]
    private GameObject lobbyLeftPanel;
    [SerializeField]
    private GameObject generalPanel ;
    [SerializeField]
    private GameObject roomPanel ;


    [Header("Connection Panel ")]
    [SerializeField]
    private Dropdown userTypeDropdown;
    [SerializeField]
    private InputField userNameInputField;


    [Header("Lobby Panel (Room Options)")]
    [SerializeField]
    private InputField maxPlayersInputField;
    public InputField MaxPlayersInputField { get => maxPlayersInputField; set => maxPlayersInputField = value; }

    [SerializeField]
    private Dropdown roomTypeDropdown;

    [Header("Room Options")]
    [SerializeField]
    private int maxPlayers = 4;
    [SerializeField]
    private bool isVisible = true;
    [SerializeField]
    private bool isOpen = true ;
    [SerializeField]
    private int _Type = 0;  // 0 ==> public room , 1 ==> training room 

    [Header("Lobby Panel ")]
    [SerializeField]
    private Text userType;
    public Text UserType { get => userType; set => userType = value; }

    [Header("Room Panel ")]
    [SerializeField]
    private GameObject room;

    [SerializeField]
    private Text userName;
    [SerializeField]
    private string userId;
    public RoomList RoomListManager;
    
    
    [Header("Room List ")]
    public PlayerList PlayerListManager;
    public Button _GoTraining;

    [Header("Photon Settings")] 
    public string GameVersionOverride = string.Empty;
    public bool ResetBestRegionCodeInPreferences = false;

     

    private void Start()
    {
        connectionPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        generalPanel.SetActive(false);
        roomPanel.SetActive(false);
        room.SetActive(false);
         
    }


    #region Load Level

	public void LoadLevel(string level)
	{
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        SceneLoader.instance.SecneName = level;
        SceneLoader.instance.LoadScene(true);
    }

    #endregion




    #region Connect

    public void ConnectOffline()
    {
        SetDebug("PunCockpit:ConnectOffline()");
        PhotonNetwork.OfflineMode = true;
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(userNameInputField.text))
        {
            ConnectionScreensManager.instance.SetPopUpMessage("Enter user name.");
            return;
        }
        ConnectionScreensManager.instance.OpenLoadingScreen();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.AuthValues = new AuthenticationValues();
        userId = Random.Range(1, 1000).ToString();
        PhotonNetwork.AuthValues.UserId = userId;
        if(userTypeDropdown.value == 0)
        {
            PhotonNetwork.NickName = "AC" + userNameInputField.text; 
        }
        else if(userTypeDropdown.value == 1)
        {
            PhotonNetwork.NickName = "AU" + userNameInputField.text; 
        }

        PhotonNetwork.ConnectUsingSettings(); 
    }

    public void ReConnect()
    { 
        PhotonNetwork.AuthValues = new AuthenticationValues();
        userId = Random.Range(1, 1000).ToString();
        PhotonNetwork.AuthValues.UserId = userId;

        PhotonNetwork.Reconnect();
    }

    public void ReconnectAndRejoin()
    {
        connectionPanel.SetActive(false); 

        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = this.userId;

        ConnectionScreensManager.instance.OpenLoadingScreen();

        PhotonNetwork.ReconnectAndRejoin();
    }

    public void ConnectToBestCloudServer()
    {

        PhotonNetwork.NetworkingClient.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;

        connectionPanel.gameObject.SetActive(false); 

        PhotonNetwork.AuthValues = new AuthenticationValues();
        userId = Random.Range(1, 1000).ToString();
        PhotonNetwork.AuthValues.UserId = this.userId;

        ConnectionScreensManager.instance.OpenLoadingScreen();

        if (this.ResetBestRegionCodeInPreferences)
        {
            ServerSettings.ResetBestRegionCodeInPreferences();
        }

        PhotonNetwork.ConnectToBestCloudServer();
        if (GameVersionOverride != string.Empty)
        {
            PhotonNetwork.GameVersion = GameVersionOverride;
        }
    }

    public override void OnConnectedToMaster()
    {
        SetDebug("PunCockpit:OnConnectedToMaster()");

        if (userTypeDropdown.value == 0)
        {
            userType.text = "Actor";

            //var newMask = oldMask | (1 << 7); 
            //_PlayerCamera.cullingMask = newMask;
            // no need to set cullingMask
        }
        else if (userTypeDropdown.value == 1)
        {
            userType.text = "Audience";
                
            //// if audience remove training layer from cullingMask   
            //var newMask = oldMask & ~(1 << 8);
            //_PlayerCamera.cullingMask = newMask;
        }

        userName.text = "User Name: " + PhotonNetwork.NickName + " /UserId : " + PhotonNetwork.AuthValues.UserId;
         
        JoinLobby();
    }

    #endregion

    #region Disconnect

    public void Disconnect()
    { 
        SetDebug("PunCockpit:Disconnect()");

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetDebug("PunCockpit:OnDisconnected(" + cause + ")");


        /*
         * ConnectionScreensManager.instance.CloseLoadingScreen();

        userName.text = string.Empty;
        userType.text = string.Empty;



        if (localPlayer != null)
        {
            localPlayer.SetActive(true);
        }

        teleportationArea = FindObjectsOfType<TeleportationArea>();

        for (int i = 0; i < teleportationArea.Length; i++)
        {
            teleportationArea[i].teleportationProvider =
                localPlayer.gameObject.GetComponent<TeleportationProvider>();
        }

        roomPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        generalPanel.SetActive(false);
        connectionPanel.SetActive(true);
        */


        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    #endregion

    #region Lobby 

    void JoinLobby()
    {
        SetDebug("PunCockpit:JoinLobby()");
        bool _result = PhotonNetwork.JoinLobby();

        if (!_result)
        {
            Debug.LogError("PunCockpit: Could not joinLobby");
        } 
    }

    public override void OnJoinedLobby()
    {
        SetDebug("PunCockpit:OnJoinedLobby()");

        lobbyPanel.SetActive(true);
        if(userType.text == "Actor")
        {
            lobbyLeftPanel.SetActive(true);
        }
        else
        {
            lobbyLeftPanel.SetActive(false);
        }
        generalPanel.SetActive(true);
        connectionPanel.SetActive(false);

        RoomListManager.OnJoinedLobbyCallBack();
        RoomListManager.gameObject.SetActive(!PhotonNetwork.OfflineMode);
        ConnectionScreensManager.instance.CloseLoadingScreen(); 
    }

    public override void OnLeftLobby()
    {
        SetDebug("PunCockpit:OnLeftLobby()");
        //RoomListManager.GetRoomList();
        RoomListManager.ResetList();

        lobbyPanel.SetActive(false);
        generalPanel.SetActive(false);
        connectionPanel.SetActive(true);

        roomPanel.SetActive(false);
        room.SetActive(false);
    }

    #endregion

    #region Set Room Options

    public void SetUserType(int value)
    {
        RoomListManager.UserType = value;
    }
     
    public void SetMaxPlayers(string num)
    {
        maxPlayers =int.Parse(num); 
        SetDebug("PunCockpit:MaxPlayer = " + maxPlayers); 
    }

    public void SetIsVisible(bool value)
    {
        isVisible = value;
        SetDebug("PunCockpit:IsVisible = " + isVisible);
    }

    public void SetIsOpen(bool value)
    {
        isOpen = value;
        SetDebug("PunCockpit:IsOpen = " + isOpen);
    }
    
    public void SetRoomType(int value)
    {
        _Type = value;
        SetDebug("PunCockpit:SetRoomType = " + _Type);
    }

    #endregion

    #region Room 

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(maxPlayersInputField.text))
        {
            ConnectionScreensManager.instance.SetPopUpMessage("please set the max player field");
            return;
        }
        if (maxPlayers > 20)
        {
            ConnectionScreensManager.instance.SetPopUpMessage("MaxPlayer > 20 player"); 
            return;
        } 

        string roomName = CreateRoomName(5) +" "+ Random.Range(1, 1000);

        if (roomTypeDropdown.value == 0) // public
        {
            string name = "P"+ roomName;
            CreateRoom(name);
        }
        else if (roomTypeDropdown.value == 1) // training
        {
            string name = "T" + roomName;
            CreateRoom(name);
        }
        
    }

    private string CreateRoomName(int stringLength = 10)
    {
        int _stringLength = stringLength - 1;
        string randomString = "";
        string[] characters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        for (int i = 0; i <= _stringLength; i++)
        {
            randomString = randomString + characters[Random.Range(0, characters.Length)];
        }
       return randomString;
    }

    void CreateRoom(string roomName)
    {
        SetDebug("PunCockpit:CreateRoom roomName:" + roomName);
        //string lobbyName = "MyLobby";
        //LobbyType lobbyType = LobbyType.SqlLobby;

        //RoomListManager.GetRoomList();
        RoomListManager.ResetList();

        lobbyPanel.gameObject.SetActive(false);
        ConnectionScreensManager.instance.OpenLoadingScreen();

        RoomOptions _roomOptions = this.GetRoomOptions();
        TypedLobby sqlLobby = new TypedLobby(null, LobbyType.Default);

        SetDebug("PunCockpit:Room options  <" + _roomOptions + ">");

        bool _result = PhotonNetwork.CreateRoom(roomName, _roomOptions, sqlLobby);

        SetDebug("PunCockpit:CreateRoom() -> " + _result);

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetDebug("PunCockpit:OnCreateRoomFailed(" + returnCode + "," + message + ")");
        ConnectionScreensManager.instance.SetPopUpMessage("Faild to create room " + message);

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        SetDebug("PunCockpit:OnJoinRandomFailed(" + returnCode + "," + message + ")");
        ConnectionScreensManager.instance.SetPopUpMessage("Faild to create room " + message);

    }

    private string roomNameToEnter;
     
    public void JoinRoom(string roomName)
    {
        //RoomListManager.GetRoomList();
        RoomListManager.ResetList();
        lobbyPanel.gameObject.SetActive(false);
        ConnectionScreensManager.instance.OpenLoadingScreen();
        roomNameToEnter = roomName;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        SetDebug(" OnCreatedRoom()");

        string res = PhotonNetwork.LocalPlayer.NickName.Substring(0, 2);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            SetDebug("this player is already Master");
        }
        else if(res == "AC")
        {
            SetDebug("this player is an actor");
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

    }

    public override void OnJoinedRoom() 
    { 
        SetDebug("PunCockpit:OnJoinedRoom()");

        ConnectionScreensManager.instance.CloseLoadingScreen();

        PlayerListManager.ResetList();
        
        roomPanel.SetActive(true);
        room.SetActive(true);

        if (userType.text == "Actor")
        {
            _GoTraining.gameObject.SetActive(true);
        }
        else
        {
            _GoTraining.gameObject.SetActive(false);
        }
        spawnManager.InstantiateViaNetwork(UserType.text);
        //this.PlayerDetailsManager.SetPlayerTarget(PhotonNetwork.LocalPlayer);

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        switch (returnCode)
        {
            case ErrorCode.JoinFailedFoundInactiveJoiner:
                if (!string.IsNullOrEmpty(this.roomNameToEnter))
                {
                    PhotonNetwork.RejoinRoom(this.roomNameToEnter);
                    this.roomNameToEnter = null;
                }
                break;
        }
    }

    public void LeaveRoom()
    {
        PlayerListManager.ResetList();
        roomPanel.gameObject.SetActive(false);
        connectionPanel.SetActive(true);

        PhotonNetwork.LeaveRoom();

    }

    public override void OnLeftRoom()
    {
        SetDebug("PunCockpit:OnLeftRoom()");
        roomPanel.SetActive(false);
        room.SetActive(false);
        //RoomListManager.GetRoomList();
        //RoomListManager.ResetList();

        

        if (PhotonNetwork.OfflineMode)
        {
            ConnectionScreensManager.instance.CloseLoadingScreen();
            ConnectionScreensManager.instance.OpenLoadingScreen(); 
        }
    }


    RoomOptions GetRoomOptions()
    {
        RoomOptions _roomOptions = new RoomOptions();

        _roomOptions.MaxPlayers = (byte)maxPlayers;

        _roomOptions.IsOpen = this.isOpen;

        _roomOptions.IsVisible = this.isVisible;

        //_roomOptions.EmptyRoomTtl = this.EmptyRoomTtl;

        //_roomOptions.PlayerTtl = this.PlayerTtl;

        //_roomOptions.PublishUserId = this.PublishUserId;

        //_roomOptions.CleanupCacheOnLeave = this.CleanupCacheOnLeave;
        //_roomOptions.DeleteNullProperties = this.DeleteNullProperties; 

        _roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", "Hello" } };
        SetDebug("C0  Hello"); 
        _roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0" };

           

        return _roomOptions;
    }

    #endregion

    #region Global Methods

    void SetDebug(string message)
    {
        Debug.Log(message);
    }

    #endregion
}
