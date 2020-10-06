using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject Panel;
    public Text PauseText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            togglePauseMenu();
        }
    }

    public void Pause()
    {
        Panel.SetActive(true);
    }

    private void togglePauseMenu()
    {
        Panel.SetActive(!Panel.activeSelf);
    }

    public void UpdateText(string s)
    {
        PauseText.text = s;
    }

    public void ResetText()
    {
        PauseText.text = "PAUSE";
    }


    #region Button Functions

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

}
