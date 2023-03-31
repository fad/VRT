using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Examples;
using UnityEngine.InputSystem.XR;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public static PlayerNetworkSetup instance;

    [Header("Player Outfits")] 
    public GameObject[] _PlayerOutfits;

    public TeleportationArea[] teleportationAreas;
    public TeleportationAnchor[] teleportationAnchors;
    public bool dontDestroyOnLoad = false;

    [SerializeField]
    private Text _Name;

    [Header("UI Controller Buttons")]
    public Button _Record;
    public Button _StopRecord;
    public Button _Replay;
    public Button _StopReplay;
    public Text _SessionsText;

    private void Awake()
    {   
        instance = this;
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
            SetUpOutfits();
         SetUpTeleportProviders();
         SetupRemotePlayers();
    }
    
    public void SetUpOutfits()
    {
        if (photonView.IsMine)
        {
            OutfitManager.instance._PlayerOutfits = _PlayerOutfits;
           // OutfitManager.instance.RemotePlayerID = photonView.ViewID;
        }
    }

    public void SetUpTeleportProviders()
    { 
        if (photonView.IsMine)
        {
            teleportationAreas = FindObjectsOfType<TeleportationArea>();
            teleportationAnchors = FindObjectsOfType<TeleportationAnchor>();

            GameObject _LocalPlayer = GameObject.FindGameObjectWithTag("LocalXRPlayer");

            if (_LocalPlayer != null)
            {
                _LocalPlayer.SetActive(false);
            }
        }

    }

    public void SetupRemotePlayers()
    {
        if (photonView.IsMine)
        {
            _Name.text = photonView.Controller.NickName;


            for (int i = 0; i < teleportationAreas.Length; i++)
            {
                teleportationAreas[i].teleportationProvider = 
                    photonView.gameObject.GetComponent<TeleportationProvider>();
            }
            for (int i = 0; i < teleportationAnchors.Length; i++)
            {
                teleportationAnchors[i].teleportationProvider = 
                    photonView.gameObject.GetComponent<TeleportationProvider>();
            }

            photonView.gameObject.GetComponent<XROrigin>().enabled = true;
            photonView.gameObject.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;
            photonView.gameObject.GetComponent<TeleportationProvider>().enabled = true;
            photonView.gameObject.GetComponent<LocomotionSystem>().enabled = true;
            photonView.gameObject.GetComponent<LocomotionSchemeManager>().enabled = true;
            photonView.gameObject.GetComponent<CharacterController>().enabled = true;
            photonView.gameObject.GetComponent<CharacterControllerDriver>().enabled = true;

        }
        else
        {
            _Name.text =  photonView.Controller.NickName;

            photonView.gameObject.GetComponent<XROrigin>().enabled = false;
            photonView.gameObject.GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
            photonView.gameObject.GetComponent<TeleportationProvider>().enabled = false;
            photonView.gameObject.GetComponent<LocomotionSystem>().enabled = false;
            photonView.gameObject.GetComponent<LocomotionSchemeManager>().enabled = false;
            photonView.gameObject.GetComponent<CharacterController>().enabled = false;
            photonView.gameObject.GetComponent<CharacterControllerDriver>().enabled = false;


            photonView.gameObject.GetComponentInChildren<Camera>().enabled = false; 
            photonView.gameObject.GetComponentInChildren<TrackedPoseDriver>().enabled = false;

            for (int i = 0; i < photonView.gameObject.GetComponentsInChildren<ActionBasedControllerManager>().Length; i++)
            {
                photonView.gameObject.GetComponentsInChildren<ActionBasedControllerManager>()[i].enabled = false;
            }

            for (int i = 0; i < photonView.gameObject.GetComponentsInChildren<XRRayInteractor>().Length; i++)
            {
                photonView.gameObject.GetComponentsInChildren<XRRayInteractor>()[i].enabled = false;
            }
            
            for (int i = 0; i < photonView.gameObject.GetComponentsInChildren<LineRenderer>().Length; i++)
            {
                photonView.gameObject.GetComponentsInChildren<LineRenderer>()[i].enabled = false;
            } 
            
            for (int i = 0; i < photonView.gameObject.GetComponentsInChildren<XRInteractorLineVisual>().Length; i++)
            {
                photonView.gameObject.GetComponentsInChildren<XRInteractorLineVisual>()[i].enabled = false;
            }

            for (int i = 0; i < photonView.gameObject.GetComponentsInChildren<ActionBasedController>().Length; i++)
            {
                photonView.gameObject.GetComponentsInChildren<ActionBasedController>()[i].enabled = false;
            } 
            
            for (int i = 0; i < photonView.gameObject.GetComponentsInChildren<XRDirectInteractor>().Length; i++)
            {
                photonView.gameObject.GetComponentsInChildren<XRDirectInteractor>()[i].enabled = false;
            }  
        }
    }

    public void GoToStage_BackStage(Transform position)
    {
        photonView.gameObject.transform.localPosition = position.localPosition;
    }


    public void ChangeOutfit(string _Name)
    {
        if (photonView.IsMine)
        {
            Debug.Log("here inside player network" + _Name);
            int id = photonView.ViewID;
            photonView.RPC("PunRPC_ChangeOutfit", RpcTarget.AllBuffered, _Name , id);
        }
    }

    [PunRPC]
    void PunRPC_ChangeOutfit(string outFitName , int id)
    {
        Debug.Log("here inside RPC" + outFitName);

        OutfitManager.instance.OnCollisionOutfit_Remote(outFitName , id);
    }

}
