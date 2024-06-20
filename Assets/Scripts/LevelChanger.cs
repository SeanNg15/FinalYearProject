using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public GameObject[] textExercises;

    public void EnterTextExercise(int i)
    {
        Debug.Log("Setting: " + textExercises[i]);
        textExercises[i].SetActive(true);
    }

    public void ExitTextExercise(int i)
    {
        Debug.Log("Quitting: " + textExercises[i]);
        textExercises[i].SetActive(false);
    }

    public void CustomAsset()
    {
        SceneManager.LoadScene("CustomAsset");
    }
    public void Outdoors()
    {
        SceneManager.LoadScene("Outdoors");
    }

    public void Room()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Basement()
    {
        SceneManager.LoadScene("Basement");
    }

    public void Exercise8Nature()
    {
        SceneManager.LoadScene("Nature");
    }

    public void Exercise7()
    {
        SceneManager.LoadScene("Exercise7");
    }

    public void Exercise10()
    {
        SceneManager.LoadScene("Exercise10");
    }

    public void Exercise16()
    {
        SceneManager.LoadScene("Exercise16");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
