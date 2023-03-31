using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingManager : MonoBehaviour
{
    public float scrollingSpeed = 20;
    public ScrollRect scrollRect;

    [SerializeField]
    public Scrollbar speedScrollbar , fontScrollbar;
    [SerializeField]
    public Toggle stopToggle , bestFitToggle , reverseToggle;
    [SerializeField]
    private Text teleprompterText; 


    public bool isStop = false;
    public bool isReverse = false;
    private void Start()
    {
        if (stopToggle.isOn)
        {
            scrollingSpeed = 0;
            isStop = true;
            speedScrollbar.interactable = false;
        }
        else
        {
            scrollingSpeed = speedScrollbar.value * 50  + 50;
        }

        if (bestFitToggle.isOn)
        {
            teleprompterText.fontSize = 14;
            fontScrollbar.interactable = false; 
        }
        else
        {
            teleprompterText.fontSize =(int) (fontScrollbar.value * 70);
        }

    }
     
    void Update()
    {
        if (!isStop)
        {
            if (isReverse)
            {
                scrollRect.content.localPosition -= new Vector3(0f, Time.deltaTime * scrollingSpeed, 0f);
            }
            else
            {
                scrollRect.content.localPosition += new Vector3(0f, Time.deltaTime * scrollingSpeed, 0f);
            }
        }
    }


    #region Speed
    public void ChangeSpeed()
    {
        scrollingSpeed = speedScrollbar.value * 100 + 20;
    }
    public void StopToggleOn_Off()
    {
        if (stopToggle.isOn)
        {
            scrollingSpeed = 0;
            isStop = true;
            speedScrollbar.interactable = false; 
        }
        else
        {
            speedScrollbar.interactable = true;
            isStop = false;
            ChangeSpeed();
        }
    }

    public void ReverseToggleOn_Off()
    {
        if (reverseToggle.isOn)
        { 
            isReverse = true; 
        }
        else
        {
            isReverse = false; 
        }
    }

    #endregion

    #region font
    public void changeFont()
    { 
     if(fontScrollbar.value < 0.1)
        teleprompterText.fontSize = (int)(fontScrollbar.value * 70 + 7 ); 
     else
        teleprompterText.fontSize = (int)(fontScrollbar.value * 70); 

    }
    public void FontToggleOn_Off()
    { 
        if (bestFitToggle.isOn)
        {
            teleprompterText.fontSize = 14;
            fontScrollbar.interactable = false;
        }
        else
        {
            fontScrollbar.interactable = true;
            changeFont();
        } 

    }
    #endregion

    #region Restart
    
    public void RestartTeleprompter()
    {
        scrollRect.content.localPosition = new Vector3(0f, 0f, 0f);
    }

    #endregion
}
