using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class BodyCollisionManager :  MonoBehaviour
{ 
    GameObject _Player;
    /// <summary>
    /// call ChangeOutfit --> PlayerNetworkSetup  to excute RPC "will sync to all players"
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.tag);
        
        if(collision.gameObject.tag  == "PlayerBody")
        {
            //OutfitManager.instance.OnCollisionOutfit(transform.name); 
            _Player = OutfitManager.instance._Player;
            _Player.GetComponent<PlayerNetworkSetup>().ChangeOutfit(transform.name);
        } 
    }
}


