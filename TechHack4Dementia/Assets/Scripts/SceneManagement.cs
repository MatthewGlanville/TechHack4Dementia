using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void VideoCallScene()
    {
        SceneManager.LoadScene("VideoCallScene");
    }

    public void PetCareScene()
    {
        SceneManager.LoadScene("VirtualPetScene");
    }
}
