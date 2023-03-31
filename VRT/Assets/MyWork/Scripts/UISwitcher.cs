using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UISwitcher : MonoBehaviour
{
    [Header("Controller References")]
    public InputActionReference UIController_Refrenece = null;
    public InputActionReference UITeleprompter_Refrenece = null;
    public InputActionReference On_Off_Teleprompter_Refrenece = null;


    [Header("UI")]
    public GameObject _UI_Controller;
    public GameObject _Telerompter;
    public Camera _MainCamera;
    public ScrollingManager scrollingManager;
   

    private void OnEnable()
    {
        // set up input refrenece
        UIController_Refrenece.action.started += ToggleUI;
        UITeleprompter_Refrenece.action.started += PlayPauseTeleprompter;
        On_Off_Teleprompter_Refrenece.action.started += OnOffTeleprompter;

        //_Telerompter = GameObject.FindGameObjectWithTag("TeleprompterUI");
        if(_Telerompter != null)
        {
            _Telerompter.GetComponent<Canvas>().worldCamera = _MainCamera;
            scrollingManager = _Telerompter.GetComponentInChildren<ScrollingManager>(); 
        }
    }

    private void OnDisable()
    {
        UIController_Refrenece.action.started -= ToggleUI;
        UITeleprompter_Refrenece.action.started -= PlayPauseTeleprompter;
        On_Off_Teleprompter_Refrenece.action.started -= OnOffTeleprompter;
    }
 
 
    private void ToggleUI(InputAction.CallbackContext context)
    {
        if (_UI_Controller.activeInHierarchy)
            _UI_Controller.SetActive(false);
        else
            _UI_Controller.SetActive(true);
    }

    private void PlayPauseTeleprompter(InputAction.CallbackContext context)
    { 
        if (!scrollingManager.isStop)
        {
            Debug.Log("here 1");

            scrollingManager.scrollingSpeed = 0;
            scrollingManager.isStop = true;
            scrollingManager.speedScrollbar.interactable = false;
        }
        else
        {
            Debug.Log("here 2");

            scrollingManager.speedScrollbar.interactable = true;
            scrollingManager.isStop = false;
            scrollingManager.ChangeSpeed();
        }
    }   
    
    public void PlayPause()
    {
        Debug.Log("here"); 

        if (!scrollingManager.isStop)
        {
            Debug.Log("here 1");

            scrollingManager.scrollingSpeed = 0;
            scrollingManager.isStop = true;
            scrollingManager.speedScrollbar.interactable = false;
        }
        else
        {
            Debug.Log("here 2");

            scrollingManager.speedScrollbar.interactable = true;
            scrollingManager.isStop = false;
            scrollingManager.ChangeSpeed(); 
        }
    }

    private void OnOffTeleprompter(InputAction.CallbackContext context)
    {
        if (_Telerompter.activeInHierarchy) 
            _Telerompter.SetActive(false); 
        else 
            _Telerompter.SetActive(true); 
    }
}
