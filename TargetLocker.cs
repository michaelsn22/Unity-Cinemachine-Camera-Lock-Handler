using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class TargetLocker : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; //main cam with cinemachine brain
    [SerializeField] private CinemachineVirtualCamera lockOnCam; //cinemachine free look cam
    [SerializeField] private LayerMask whatIsEnemy; //layermask for enemy distance checking
    private Transform ourTrans; //transform for enemy if theyre in range and locked onto
    private bool lockedOn = false; //lock on bool

    [SerializeField] private Image aimIcon;  // ui image of aim icon

    //vars for cycling between targets
    private List<Transform> enemiesInRange = new List<Transform>();
    private int currentTargetIndex = 0; //track current target
    private int cyclesCount = 0; //track how many times we've swapped between targets
    

    private void Update()
    {
        // Check if the Virtual Camera is actively controlling the Unity camera
        bool isActive = lockOnCam.gameObject.activeInHierarchy;

        // Get positions of potential enemies (including the current target)
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 20f, whatIsEnemy);
        enemiesInRange.Clear();

        foreach (Collider col in hitEnemies)
        {
            enemiesInRange.Add(col.transform);
        }

        // Sort the enemies based on distance
        enemiesInRange.Sort((a, b) =>
            Vector3.Distance(transform.position, a.position).CompareTo(Vector3.Distance(transform.position, b.position))
        );

        // Find the index of the current target in the sorted list
        currentTargetIndex = enemiesInRange.IndexOf(ourTrans);

        //testing lock on.
        if (Input.GetKeyDown(KeyCode.X)) // Attempt to lock onto something when key is pressed.
        {
            if (!lockedOn)
            {
                if (enemiesInRange.Count > 0)
                {
                    currentTargetIndex = 0;
                    ourTrans = enemiesInRange[currentTargetIndex];
                    lockedOn = true;
                    cyclesCount = 0;
                }
            }
            else
            {
                // Cycle to the next target.
                if (enemiesInRange.Count > 0)
                {
                    currentTargetIndex = (currentTargetIndex + 1) % enemiesInRange.Count;
                    ourTrans = enemiesInRange[currentTargetIndex];
                    cyclesCount++;

                    // If we've cycled through all targets, unlock.
                    if (cyclesCount >= enemiesInRange.Count)
                    {
                        lockedOn = false;
                        enemiesInRange.Clear();
                    }
                }
                else
                {
                    // If no other enemies are around, unlock.
                    lockedOn = false;
                    enemiesInRange.Clear();
                }
            }
        }

        if (lockedOn) //change where the camera is looking and where the aim icon is every frame.
        {
            //enable the lock on cam, disable the old one.
            if (!isActive)
            {
                lockOnCam.gameObject.SetActive(true);
            }

            if (aimIcon)
            {
                aimIcon.gameObject.SetActive(lockedOn);
            }

            if (ourTrans != null && ourTrans.GetComponent<Enemy>()?.ReturnValue() == true)
            {
                Transform childTransform = ourTrans.Find("LockOnPoint");
                //if the enemy is still alive:
                if (childTransform != null)
                {
                    lockOnCam.LookAt = childTransform;

                    if (aimIcon)
                    {
                        aimIcon.transform.position = mainCamera.WorldToScreenPoint(childTransform.position);
                    }
                }
                else
                {
                    lockOnCam.LookAt = ourTrans;

                    if (aimIcon)
                    {
                        aimIcon.transform.position = mainCamera.WorldToScreenPoint(ourTrans.position);
                    }
                }
            }
            else
            {
                //Debug.Log("targeted enemy is dead. doing the camera swap!");
                lockedOn = false;
                lockOnCam.LookAt = null;
                if (isActive)
                {
                    lockOnCam.gameObject.SetActive(false);
                }

                if (aimIcon && aimIcon.gameObject.activeInHierarchy)
                {
                    aimIcon.gameObject.SetActive(lockedOn);
                }
            }
        }
        else
        {
            lockOnCam.LookAt = null;
            //disable that cam and enable the other.
            if (isActive)
            {
                lockOnCam.gameObject.SetActive(false);
            }

            if (aimIcon && aimIcon.gameObject.activeInHierarchy)
            {
                aimIcon.gameObject.SetActive(lockedOn);
            }
        }
    }
}
