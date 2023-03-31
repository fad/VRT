using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class OutfitManager : MonoBehaviour
{
    public static OutfitManager instance;

    [Header("SpawnManager")]
    public SpawnManager spawnManager;

    [Header("Player")]  
    public GameObject  _Player;  
    public GameObject[] _PlayerOutfits;

    [Header("Remote Player")] 
    public GameObject remotePlayer;

    [Header("Costumes")]
    [SerializeField]
    private GameObject[] _Outfits;

    [Header("Positions")]
    [SerializeField]
    private Vector3[] _OutfitsOrginPostions;

    [Header("Selected")]
    [SerializeField]
    private GameObject selectedOutfit;

 
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _OutfitsOrginPostions = new Vector3[_Outfits.Length];
        for (int i = 0; i < _Outfits.Length; i++)
        {
            _OutfitsOrginPostions[i] = _Outfits[i].transform.localPosition;  
        } 
    }

    public void OnSelectOutfit(GameObject selected)
    {
        _Player =  spawnManager.GetPlayer();
        selectedOutfit = selected; 
    }

    public void OnDeselectOutfilt()
    {
        for (int i = 0; i < _Outfits.Length; i++)
        {
            if(_Outfits[i].name == selectedOutfit.name)
            { 
                //StartCoroutine(Lerp(_Outfits[i].transform.localPosition, _OutfitsOrginPostions[i], 3f));

                _Outfits[i].transform.localPosition = _OutfitsOrginPostions[i];
                _Outfits[i].transform.localRotation = new Quaternion(0f,0f,0f,0f);
                //selectedOutfit = null;
            }
        }
    }

    IEnumerator Lerp(Vector3 startPos, Vector3 endPos , float duration)
    {
        float timeToStart = Time.time;
        while (Vector3.Distance(startPos, endPos) > 0.05f)
        {
            startPos = Vector3.Lerp(startPos, endPos, (Time.time - timeToStart) * 1);

            yield return null;
        }

        print("Reached the target.");

        yield return new WaitForSeconds(duration); // THis is just for how Coroutine works with delay

        print("MyCoroutine is now finished.");
    }

    public void OnCollisionOutfit(string outfitName)
    {
        for (int i = 0; i < _PlayerOutfits.Length; i++)
        {
            if(_PlayerOutfits[i].name == outfitName)
            {
                Debug.Log("here inside  OnCollisionOutfit" + outfitName);

                for (int y = 0; y < _PlayerOutfits.Length; y++)
                    _PlayerOutfits[y].GetComponent<SkinnedMeshRenderer>().enabled = false;

                _PlayerOutfits[i].GetComponent<SkinnedMeshRenderer>().enabled = true;
                OnDeselectOutfilt();
            }
        }
    }

    public void OnCollisionOutfit_Remote(string outfitName , int _PhotonViewId)
    {
        remotePlayer =  PhotonView.Find(_PhotonViewId).gameObject;
        GameObject[] _RemoteOutfits = remotePlayer.GetComponent<PlayerNetworkSetup>()._PlayerOutfits;

        for (int i = 0; i < _RemoteOutfits.Length; i++)
        {
            if (_RemoteOutfits[i].name == outfitName)
            {
                Debug.Log("here inside  OnCollisionOutfit" + outfitName);

                for (int y = 0; y < _RemoteOutfits.Length; y++)
                    _RemoteOutfits[y].GetComponent<SkinnedMeshRenderer>().enabled = false;

                _RemoteOutfits[i].GetComponent<SkinnedMeshRenderer>().enabled = true;
                
            }
        }
    }

}
