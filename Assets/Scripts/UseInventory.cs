using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UseInventory : MonoBehaviour
{
    public GameObject guide;
    public GameObject inventory;

    public void UseItem()
    {
        gameObject.GetComponentInChildren<CreateObject>().AddObject();
    }

    public void UseCustom()
    {
        gameObject.GetComponentInChildren<CreateObject>().AddCustomToy();
    }

    public void CloseGuide()
    {
        guide.SetActive(false);
        inventory.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        PlayerCam.paused = false;
        PlayerCam.guideOn = false;
    }

    public void OpenGuide()
    {
        guide.SetActive(true);
        PlayerCam.guideOn = true;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
