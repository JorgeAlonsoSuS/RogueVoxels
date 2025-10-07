using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOpener : MonoBehaviour
{
    public void OpenEasy()
    {
        SceneManager.LoadScene("Main XR");
    }

    public void OpenMedium()
    {
        SceneManager.LoadScene("Main XR 50");
    }

    public void OpenHard()
    {
        SceneManager.LoadScene("Main XR 100");
    }

    public void Close()
    {
        
    }
}
