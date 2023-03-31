using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class LoadText : MonoBehaviour
{

    [SerializeField]
    private GameObject telepromterText , connectingText , reloadBtn;
        
    // Start is called before the first frame update
    void Start()
    {
        connectingText.gameObject.SetActive(true);
        telepromterText.gameObject.SetActive(false);
        reloadBtn.gameObject.SetActive(false);

        Invoke("StartLoadTextFromServer", 2);
    }

    public void StartLoadTextFromServer()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://textdoc.co//home/download/a6w15imRXltrvbq3");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            connectingText.gameObject.SetActive(false);
            reloadBtn.gameObject.SetActive(true);
        }
        else if (www.result == UnityWebRequest.Result.Success)
        {
            // Show results as text
            connectingText.gameObject.SetActive(false);
            telepromterText.gameObject.SetActive(true);
            Debug.Log(www.downloadHandler.text);
            telepromterText.GetComponent<Text>().text = www.downloadHandler.text;
        }
    }


}

