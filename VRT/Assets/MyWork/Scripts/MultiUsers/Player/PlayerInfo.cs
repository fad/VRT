using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{ 

    [Header("Player Info")]
    [SerializeField]
    private string _PlayerName;
    [SerializeField]
    private string _PlayerId;
    [SerializeField]
    private string _PlayerType;

    public string PlayerName { get => _PlayerName; set => _PlayerName = value; }
    public string PlayerId { get => _PlayerId; set => _PlayerId = value; }
    public string PlayerType { get => _PlayerType; set => _PlayerType = value; }

    private void Start()
    {
    }
}
