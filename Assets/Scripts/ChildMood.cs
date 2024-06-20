using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMood : MonoBehaviour
{
    public bool interacted = false;
    public float moodIncrease = 2.5f;
    public GameObject interactFeedback;
    public GameObject characterModel;

    private Animator animator;
    private float currMood;

    private int happyThreshold = 30;


    void Awake()
    {
        animator = characterModel.GetComponent<Animator>();
        currMood = 0;
    }


    void Start()
    {
        StartCoroutine(IncreaseMood());
    }

    IEnumerator IncreaseMood()
    {
        while (true)
        {
            //Debug.Log("Mood: " + currMood);
            if (interacted)
            {
                //show that player is interacting with child
                interactFeedback.SetActive(true);

                currMood = Math.Min(currMood + moodIncrease, 100);
            } 
            else
            {
                interactFeedback.SetActive(false);
                //stop dancing after some time when player not looking
                if (currMood > happyThreshold + moodIncrease)
                {
                    currMood = Math.Max(currMood - 5, happyThreshold + moodIncrease);
                }
                
            }

            animator.SetFloat("Mood", currMood);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
