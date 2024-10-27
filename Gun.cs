using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletImpact;  // Bullet fragment effect
    ParticleSystem bulletEffect;    // Bullet fragment particle system
    AudioSource bulletAudio;        // Bullet firing sound
    public Transform crosshair;     // Crosshair object

    void Start()
    {
        // Import the bullet effect particle system component
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        // Get the bullet effect audio source component
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
    }

    void Update()
    {
        // Get the left hand's position and direction
        // 왼쪽 손의 위치와 방향을 가져온다.
        Vector3 leftHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Quaternion leftHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

        // Set a reasonable distance for the crosshair to be away from the camera (e.g., 5 units away)
        float crosshairDistance = 100f; // Adjust this value to control the distance of the crosshair from the camera

        // Set the crosshair position at a distance from the left hand's forward direction
        crosshair.position = Camera.main.transform.TransformPoint(leftHandPosition + (leftHandRotation * Vector3.forward * crosshairDistance));  // Convert local position to world position with offset distance
        crosshair.forward = Camera.main.transform.TransformDirection(leftHandRotation * Vector3.forward);  // Convert local direction to world direction

        // When the user presses the IndexTrigger button on the left controller
        if (Input.GetButtonDown("Fire3") ||OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            // Play bullet audio
            bulletAudio.Stop();
            bulletAudio.Play();

            // Create a ray from the left hand's position and direction
            Ray ray = new Ray(Camera.main.transform.TransformPoint(leftHandPosition),
                              Camera.main.transform.TransformDirection(leftHandRotation * Vector3.forward));
            RaycastHit hitInfo;

            // Exclude the player and tower layers from the raycast
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
            int layerMask = playerLayer | towerLayer;

            // Perform the raycast
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                // Handle bullet fragment effect
                bulletEffect.Stop();
                bulletEffect.Play();

                // Set the bullet's effect direction to match the point of impact
                bulletImpact.forward = hitInfo.normal;
                // Set the effect to appear directly at the point of impact
                bulletImpact.position = hitInfo.point;
            }
        }
    }
}
