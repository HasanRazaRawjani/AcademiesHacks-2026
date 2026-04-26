using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI_Start : MonoBehaviour
{
    public void Start_Game()
    {
        SceneManager.LoadScene("The King"); 
    }

    public void Quit_Game()
    {
        Application.Quit();
    }

}
