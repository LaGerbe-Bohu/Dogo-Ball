using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void beginGameIA()
    {
        InterfaceGameState.J1set = 0;
        InterfaceGameState.J2set = 0;
        InterfaceGameState.IA = true;

        SceneManager.LoadScene(1);
    }
    
    public void beginGameRandom()
    {
        InterfaceGameState.J1set = 0;
        InterfaceGameState.J2set = 0;
        InterfaceGameState.IA = false;

        SceneManager.LoadScene(1);
    }


    public void quitGame() {
        Application.Quit();
    }

    public void returnMenu() {
        SceneManager.LoadScene(0);
    }
}
