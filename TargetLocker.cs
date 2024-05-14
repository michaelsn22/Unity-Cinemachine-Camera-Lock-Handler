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
    


    private void Update()
    {
        // Check if the Virtual Camera is actively controlling the Unity camera
        bool isActive = lockOnCam.gameObject.activeInHierarchy;


        //testing lock on.
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!lockedOn)
            {
                // Get position of a potential enemy.
                Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 20f, whatIsEnemy); //(center, radius, layermask)

                if (hitEnemies.Length > 0)
                {
                    Transform closestTransform = null;
                    float closestDistance = Mathf.Infinity;

                    foreach (Collider col in hitEnemies)
                    {
                        float distanceToCollider = Vector3.Distance(transform.position, col.transform.position);
                        if (distanceToCollider < closestDistance)
                        {
                            closestDistance = distanceToCollider;
                            closestTransform = col.transform;
                        }
                    }

                    if (closestTransform != null)
                    {
                        ourTrans = closestTransform;
                        lockedOn = true;
                        //Debug.Log("locking onto target!");
                    }
                }
            }
            else if(lockedOn)
            {
                lockedOn = false;
            }
        }

        if (lockedOn)
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

            if (ourTrans != null && ourTrans.GetComponent<Enemy>().ReturnValue())
            {
                //if the enemy is still alive:
                lockOnCam.LookAt = ourTrans;

                //icon test
                if(aimIcon)
                {
                    //Debug.Log("icon should be moving to obj");
                    aimIcon.transform.position = mainCamera.WorldToScreenPoint(ourTrans.position);
                }
            }
            else{
                //Debug.Log("targeted enemy is dead. doing the camera swap!");
                lockedOn = false;
                lockOnCam.LookAt = null;
                if (isActive)
                {
                    lockOnCam.gameObject.SetActive(false);
                }

                if (aimIcon && aimIcon.gameObject.activeInHierarchy)
                {
                   // Debug.Log("icon should be gone since enemy died!");
                    aimIcon.gameObject.SetActive(lockedOn);
                }
            }
        }
        else{
            lockOnCam.LookAt = null;
            //disable that cam and enable the other.
            if (isActive)
            {
                lockOnCam.gameObject.SetActive(false);
            }

            if (aimIcon && aimIcon.gameObject.activeInHierarchy)
            {
                //Debug.Log("icon should be gone!");
                aimIcon.gameObject.SetActive(lockedOn);
            }
        }
    }
}
