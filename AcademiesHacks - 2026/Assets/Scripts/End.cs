using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class End : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Start()
    {
        text.SetText("You Lost!");
    }

    public void Quit_Game()
    {
        Application.Quit();
    }
}
