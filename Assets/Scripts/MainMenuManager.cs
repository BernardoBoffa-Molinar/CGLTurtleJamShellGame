using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject[] Panels;


    public void GoToGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetPanelVisible(int index)
    {
        foreach(GameObject go in Panels)
        {
            go.SetActive(false);
        }

        Panels[index].SetActive(true);

    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in Panels)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
