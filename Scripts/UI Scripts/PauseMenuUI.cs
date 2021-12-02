using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    /// <summary>
    /// Checks to see if the game is paused or not
    /// </summary>
    public static bool isPaused;

    public GameObject quitMenu;

    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        // Starts the game not paused
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);

        quitMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ScubaController.scuba.baitCraftUI.activeSelf || ScubaController.scuba.fishingHelpUI.activeSelf)
                return;

            // If false becomes true or if true becomes false
            isPaused = !isPaused;

            // If isPaused is true, freeze things on screen
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
            }
            // If isPaused is false, time is normal
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;
                NoQuit();
            }

            pauseMenu.SetActive(isPaused);
        }
    }
    /// <summary>
    /// When the game is not paused, the pause menu is not visible and
    /// the game speed is normal
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }


    public void ConfirmMenu()
    {
        quitMenu.SetActive(true);

        pauseMenu.SetActive(false);
    }

    /// <summary>
    /// If no is clicked, turn the menu off
    /// </summary>
    public void NoQuit()
    {
        quitMenu.SetActive(false);

        pauseMenu.SetActive(true);
    }
}