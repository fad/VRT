using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionScreensManager : MonoBehaviour
{
    public static ConnectionScreensManager instance;

    [SerializeField]
    private GameObject loadingScreen, messagePopUp;

    [SerializeField]
    private Text messageText;

    Animator animator;
    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }

    public void SetPopUpMessage(string message)
    {
        messagePopUp.SetActive(true);
        animator.Play("MessagePopUp");
        messageText.text = message;
        Invoke("BackToIdle", 2f);
    }

    void BackToIdle()
    {
        messagePopUp.SetActive(false);
        animator.Play("Idle");
    }
    
    public void OpenLoadingScreen()
    {
        loadingScreen.SetActive(true);
        animator.Play("Loading");
    }

    public void CloseLoadingScreen()
    {
        loadingScreen.SetActive(false);
        animator.Play("Idle");
    }
}
