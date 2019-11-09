using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TankMovement : MonoBehaviour {

    public int PlayerNumber = 1;   // Used to identify which TANK belongs to which player.
    public float Speed = 12f;     // Used to identify How Fast the TANK moves Forward and Backwards.
    public float TurnSpeed = 180f;  // Used to identify How Fast the TANK turns in degrees per second.
    public AudioSource MovementAudio; // Audio Source to play Engine Sound.
    public AudioClip EngineIdling;  // Audio to play when the TANK is NOT moving.
    public AudioClip EngineDriving;  // Audio to play when the TANK is moving.
    public float PitchRange = 0.2f;  // Used to identify the Amount by which the Pitch of the engine noices can vary.


    private string MovementAxisName;  // Name of Input Axix for moving ( F and B ).
    private string TurnAxisName;   // Name of Input Axis for Turning.
    private Rigidbody RBody;    // Reference used to move the TANK.
    private float MovementInputValue; // Current Value of the movement input.
    private float TurnInputValue;   // Current value of turn input.
    private float OriginPitch;   // Pitch of audio source at the start of the scene.


    private void Awake()
    {
        RBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()   // IsKinematic : If you want Physics , but you dont want physics Forces to act.
    {
        RBody.isKinematic = false;
        MovementInputValue = 0f;
        TurnInputValue = 0f;
    }

    private void OnDisable()
    {
        RBody.isKinematic = true;
    }

	private void Start ()
    {
        MovementAxisName = "Vertical" + PlayerNumber;
        TurnAxisName = "Horizontal" + PlayerNumber;
        OriginPitch = MovementAudio.pitch;
	}
	
	// Update is called once per frame
	private void Update ()   // Store the Player's Input ; Make sure the Engine sound is working.
    {
        MovementInputValue = Input.GetAxis(MovementAxisName);
        TurnInputValue = Input.GetAxis(TurnAxisName);

        EngineAudio();
	}

    private void EngineAudio()  // Play the Correct Audi Clip , depending on wheather or not the TANK is actually Moving.
    {
        if(Mathf.Abs(MovementInputValue) < 0.1f && Mathf.Abs(TurnInputValue) < 0.1f)
        {
            if(MovementAudio.clip == EngineDriving)   // ... and if the audio source is currently playing the driving clip... 
            {// Change Clip to IDLING and play it.
                MovementAudio.clip = EngineIdling;
                MovementAudio.pitch = Random.Range(OriginPitch - PitchRange, OriginPitch + PitchRange);   // Randomise the Pitch.
                MovementAudio.Play();  // Keep Playing.
            }
        }
        else
        {
            if (MovementAudio.clip == EngineIdling)   // Otherwise if the tank is moving and if the idling clip is currently playing...
            {// Change Clip to Driving and play it.
                MovementAudio.clip = EngineDriving;
                MovementAudio.pitch = Random.Range(OriginPitch - PitchRange, OriginPitch + PitchRange);
                MovementAudio.Play();  // Keep Playing.
            }
        }
    }

    private void FixedUpdate()  // Move and Turn the Tank.
    {
        Move();
        Turn();
    }

    private void Move()  // Adjust the position of the TANK based on the player's input.
    {
        Vector3 Movement = transform.forward * MovementInputValue * Speed * Time.deltaTime;

        RBody.MovePosition(RBody.position + Movement); 
    }

    private void Turn()  // Adjust the Roatation of the TANK based on the Player's input.
    {
        float Turn = TurnInputValue * TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, Turn, 0f);   // Stores the rotation internally in form of a Vector3 (x , y & z).

        RBody.MoveRotation(RBody.rotation * turnRotation);
    }
}
