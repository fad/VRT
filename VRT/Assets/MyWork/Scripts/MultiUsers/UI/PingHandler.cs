using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PingHandler : MonoBehaviour
{
    public Text Text;

    int _cache = -1;

    void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.GetPing() != _cache)
            {
                _cache = PhotonNetwork.GetPing();
                if(_cache >= 120)
                {
                    Text.color = Color.red;
                    Text.text = _cache.ToString() + " ms";
                }
                else
                {
                    Text.color = Color.green;
                    Text.text = _cache.ToString() + " ms";
                }
            }
        }
        else
        {
            if (_cache != -1)
            {
                _cache = -1;
                Text.text = "n/a";
            }
        }
    }

    
}
