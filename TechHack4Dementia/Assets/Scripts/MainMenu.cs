using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string petGameSceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayPetGame()
    {
       SceneManager.LoadScene(petGameSceneName);
    }
    public void makeCall()
    {
        SceneManager.LoadScene("VideoCallScene");
    }
    public void returnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
