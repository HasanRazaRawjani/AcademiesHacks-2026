using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class End1 : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Start()
    {
        text.SetText("You Won!");
    }

    public void Quit_Game()
    {
        Application.Quit();
    }
}
