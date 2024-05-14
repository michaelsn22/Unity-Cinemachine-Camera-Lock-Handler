This script handles swapping between two camera's in Unity, allowing lock-on functionality similar to games such as Zelda BoTW or Dark Souls. Please note that the main camera must have its cinemachine brain component attached to a different camera initially to work, since this script will override that brain components assigned value. The second cam (lockOnCam) must be a cinemachine virtual cameara with its initial SetActive value to false on runtime. The virtual lockOnCam must also have its 'Body' field set to "framing transposer", follow set to the player, and LookAt to be assigned to the target you wish to lock onto. Make sure to set the 'Aim' field of the virtual camera to 'Composer' to avoid jittery movements of the camera. Lastly, please note that this does not support transitioning between different targets in range yet.