using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
               
    }

    public void CloseHand()
    {
        playerAnimator.SetBool("OpenHand", false);
        playerAnimator.SetBool("CloseHand" , true);
    }

    public void OpenHand()
    {
        playerAnimator.SetBool("CloseHand", false);
        playerAnimator.SetBool("OpenHand", true);
    }

    public void Idel()
    {
        playerAnimator.SetBool("OpenHand", false);
        playerAnimator.SetBool("CloseHand", false);
    }

}
