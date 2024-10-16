using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrMainMenu : MonoBehaviour
{
    public void StartSolo ()
    {
        Debug.Log("Starting solo");
        PlayerPrefs.SetInt("Randomise", 0);
        SceneManager.LoadScene("SoloPlay");
    }

    public void StartRandom()
    {
        Debug.Log("Starting solo");
        PlayerPrefs.SetInt("Randomise", 1);
        SceneManager.LoadScene("SoloPlay");
    }

    public void StartVersus()
    {
        Debug.Log("Starting versus");
        SceneManager.LoadScene("VersusPlay");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
