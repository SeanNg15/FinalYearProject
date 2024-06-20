using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ChildMovement : MonoBehaviour
{
    public float rotatespeed = 3f;
    public float movespeed = 1.5f;
    public int width = 5;

    private Animator animator;
    private ChildMood childMood;
    private List<Vector3> choices;
    private int currChoice = 0;

    private bool wandering = false;

    private int happyThreshold = 30;

    private int danceThreshold = 70;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        childMood = GetComponentInChildren<ChildMood>();

        var d1 = transform.position + new Vector3(width, 0, width);
        var d2 = transform.position + new Vector3(-width, 0, width);
        var d3 = transform.position + new Vector3(width, 0, -width);
        var d4 = transform.position + new Vector3(-width, 0, -width);
        choices = new()
        {
            d1, d2, d3, d4
        };
        StartCoroutine(Wander());
    }

    void FixedUpdate()
    {
        float mood = animator.GetFloat("Mood");
        if (childMood.interacted)
        {
            wandering = false;
            animator.SetBool("isWalking", wandering);
        }


        if (mood > happyThreshold && mood < danceThreshold && wandering)
        {
            GoTowards(currChoice);
        }
        else
        {
            wandering = false;
        }
        
    }

    IEnumerator Wander()
    {
        while (true)
        {
            if (childMood.interacted)
            {
                yield return null;
            }
            else
            {
                MakeChoice();
                int waitTime = Random.Range(3, 6);
                yield return new WaitForSeconds(waitTime);
            }
        }

    }

    void MakeChoice()
    {
        int choice = Random.Range(0, 2);
        if (choice == 0)
        {
            wandering = false;
        } 
        else 
        {
            wandering = true;
            currChoice = Random.Range(0, 4);
        }
        animator.SetBool("isWalking", wandering);

    }

    void GoTowards(int choice)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = choices[choice] - transform.position;

        //Rotate towards the target by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotatespeed * Time.deltaTime, 0.0f);
        newDirection = new Vector3(newDirection.x, 0, newDirection.z);

        //Get movement
        Vector3 currSpeed = Vector3.MoveTowards(transform.position, choices[choice], movespeed * Time.deltaTime);

        //apply rotation and movement
        transform.SetPositionAndRotation(currSpeed, Quaternion.LookRotation(newDirection));

        if (Vector3.Distance(transform.position, choices[choice]) < 0.5f)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                int noise = Random.Range(-2, 3);
                choices[i] = choices[i] + new Vector3(noise, 0, noise);
            }
            currChoice = Random.Range(0, 4);
        }
    }

}
