using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float DampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
    public float MinSize = 6.5f;                  // The smallest orthographic size the camera can be.
    [HideInInspector] public Transform[] Targets; // All the targets the camera needs to encompass. Array of Tanks.

    private Camera Cam;                          // Used for referencing the camera.
    private float ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 MoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 DesiredPosition;              // The position the camera is moving towards.

    private void Awake()
    {
        Cam = GetComponentInChildren<Camera>();    // Because the MainCamera is a child of "CameraRig".
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();  // Find Avg Position of the Targets.

        transform.position = Vector3.SmoothDamp(transform.position, DesiredPosition, ref MoveVelocity, DampTime);  // SmoothDamp Takes 4 parameters. And go to them.
    }

    private void FindAveragePosition()
    {
        Vector3 AvgPos = new Vector3();
        int NumTargets = 0;
        int i;

        for (i = 0; i < Targets.Length; i++)
        {
            if (!Targets[i].gameObject.activeSelf)   // Basically This Line tells us , that IF a Tank is no longer active as in OBLITERATED !; Continue and dont focus/ zoomIn on it.
                continue;

            AvgPos += Targets[i].position;   // take the Average Position and add it to the Position of the Winner's Tank to ZoomIn on it.
            NumTargets++;   // Count the Number of remaining targets.
        }

        if (NumTargets > 0)
            AvgPos /= NumTargets;  // To get the Position between the Two or More targets.

        AvgPos.y = transform.position.y;   // Do Not Change the Y position, keep it steady.
        DesiredPosition = AvgPos;    // The Desired Position is the Avergae position between The two TANKS.
    }

    private void Zoom() // Find the required size based on the desired position and smoothly transition to that size.
    {
        float RequiredSize = FindRequiredSize();  

        Cam.orthographicSize = Mathf.SmoothDamp(Cam.orthographicSize, RequiredSize, ref ZoomSpeed, DampTime);
    }

    private float FindRequiredSize()
    {
        Vector3 DesiredLocalPos = transform.InverseTransformPoint(DesiredPosition); // Find the position the camera rig is moving towards in its local space.
        float Size = 0f;  // Start CAMERA size calculation at 0F.
        int i;

        for(i=0;i<Targets.Length;i++)   // Go Through All Targets.
        {
            if (!Targets[i].gameObject.activeSelf)  // ... If they are not moving or destroyed , continue and go to the next one.
                continue;

            Vector3 TargetLocalPos = transform.InverseTransformPoint(Targets[i].position);   // Otherwise, find the position of the target in the camera's local space.
            Vector3 DesiredPosToTarget = TargetLocalPos - DesiredLocalPos;  // Find the position of the target from the desired position of the camera's local space.

            Size = Math.Max(Size, Math.Abs(DesiredPosToTarget.y));    // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
            Size = Math.Max(Size, Math.Abs(DesiredPosToTarget.x) / Cam.aspect);   // Zoom = Distance / Aspect.
        }
        Size += ScreenEdgeBuffer;   // Add the Size to the Edgebuffer to give it as much space as possible to Fit in the Screen.
        Size = Math.Max(Size, MinSize);    // Make sure it's not too zoomed in.
        return Size;
    }

    public void SetStartPositionAndSize()   // Used to Find the position for each NEW round.
    {
        FindAveragePosition();

        transform.position = DesiredPosition;

        Cam.orthographicSize = FindRequiredSize();

    }

}
