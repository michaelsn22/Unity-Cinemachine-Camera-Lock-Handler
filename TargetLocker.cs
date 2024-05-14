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
    private int currentTargetIndex = 0;

    private void Update()
    {
        // Check if the Virtual Camera is actively controlling the Unity camera
        bool isActive = lockOnCam.gameObject.activeInHierarchy;

        //testing lock on.
        if (Input.GetKeyDown(KeyCode.X)) //attempt to lock onto something when key is pressed.
        {
            if (!lockedOn)
            {
                // Get position of a potential enemy.
                Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 20f, whatIsEnemy); //(center, radius, layermask)
                enemiesInRange.Clear(); //clear the array of transforms

                foreach (Collider col in hitEnemies)
                {
                    enemiesInRange.Add(col.transform);
                }

                if (enemiesInRange.Count > 0)
                {
                    enemiesInRange.Sort((a, b) => 
                        Vector3.Distance(transform.position, a.position).CompareTo(Vector3.Distance(transform.position, b.position))
                    );
                    currentTargetIndex = 0;
                    ourTrans = enemiesInRange[currentTargetIndex];
                    lockedOn = true;
                }
            }
            else
            {
                //clear memory, check again if targets are in range. THEN proceed to swap targets if there isnt more than one enemy around. So the player can make their choice to un-lock the camera if they want.
                Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 20f, whatIsEnemy); //(center, radius, layermask)
                enemiesInRange.Clear();
                
                foreach (Collider col in hitEnemies)
                {
                    enemiesInRange.Add(col.transform);
                }

                // Cycle to the next target.
                if (enemiesInRange.Count > 1)
                {
                    currentTargetIndex = (currentTargetIndex + 1) % enemiesInRange.Count;
                    ourTrans = enemiesInRange[currentTargetIndex];
                }
                else
                {
                    // If no other enemies are around besides that one enemy, unlock.
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

            if (ourTrans != null && ourTrans.GetComponent<Enemy>().ReturnValue())
            {
                Transform childTransform = ourTrans.Find("LockOnPoint");
                //if the enemy is still alive:
                if (childTransform != null)
                {
                    //Debug.Log("found child transform!");
                    lockOnCam.LookAt = childTransform;
                    
                    if(aimIcon)
                    {
                        //Debug.Log("icon should be moving to obj");
                        aimIcon.transform.position = mainCamera.WorldToScreenPoint(childTransform.position);
                    }
                }
                else{
                    //Debug.Log("didnt find child transform!");
                    lockOnCam.LookAt = ourTrans;

                    if(aimIcon)
                    {
                        //Debug.Log("icon should be moving to obj");
                        aimIcon.transform.position = mainCamera.WorldToScreenPoint(ourTrans.position);
                    }
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
