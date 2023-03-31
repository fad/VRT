using Photon.Pun; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    private string _SecneName; 
    public string SecneName { get => _SecneName; set => _SecneName = value; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadScene(bool isAllPlayers)
    {
        if (isAllPlayers)
        {
            PhotonNetwork.LoadLevel(_SecneName);
        }
        else
        { 
            SceneManager.LoadScene(_SecneName);
        }
        
    }
      
}
