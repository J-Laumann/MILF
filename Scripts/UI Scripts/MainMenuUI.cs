using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    public GameObject mainUI, introUI;
    public Button continueButton;

    private void Start()
    {
        if (continueButton)
        {
            if(PlayerPrefs.GetInt("Dex008", 0) == 0)
            {
                continueButton.interactable = false;
            }
        }
    }

    /// <summary>
    /// Loads the next scene
    /// </summary>
    /// <param name="levelName"></param>
    public void LoadLevel(string levelName)
    {
        StartCoroutine(InvokePause(levelName));
    }

    IEnumerator InvokePause(string levelName)
    {
        yield return new WaitForSecondsRealtime(.1f);

        //PauseMenu.isPaused = false;

        SceneManager.LoadScene(levelName);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OpenNewGame()
    {
        mainUI.SetActive(false);
        introUI.SetActive(true);
    }

    public void CloseNewGame()
    {
        mainUI.SetActive(true);
        introUI.SetActive(false);
    }

    public void WipeData()
    {
        PlayerPrefs.DeleteAll();
    }
}
