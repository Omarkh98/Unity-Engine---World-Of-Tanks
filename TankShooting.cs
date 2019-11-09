using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour {

    public int PlayerNumber = 1;
    public Rigidbody Shell;
    public Transform FireTransform;
    public Slider AimSlider;
    public AudioSource ShootingAudio;
    public AudioClip ChargingClip;
    public AudioClip FireClip;
    public float MinLaunchForce = 15f;
    public float MaxLaunchForce = 30f;
    public float MaxChargeTime = 0.75f;

    private string FireButton;
    private float CurrentLaunchForce;
    private float ChargeSpeed;
    private bool Fired;

    private void OnEnable()  // When the tank is turned on, reset the launch force and the UI to the Starting or Minimum Launch Force.
    {
        CurrentLaunchForce = MinLaunchForce;
        AimSlider.value = MinLaunchForce;
    }

    private void Start () // Distance = Speed * Time.
    { 
        FireButton = "Fire" + PlayerNumber;  // The fire axis is based on the player number.

        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;  // The rate that the launch force charges up is the range of possible forces by the max charge time.
    }
	
	private void Update ()  // Track the Current State of the Fire Button and make decisions based on the Current Launch Force.
    {
        AimSlider.value = MinLaunchForce;

        if(CurrentLaunchForce >= MaxLaunchForce && !Fired)   // At Max Charge But Not Fired!
        {
            CurrentLaunchForce = MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(FireButton))   // Otherwise, if the fire button has just started being pressed...
        {
            Fired = false;
            CurrentLaunchForce = MinLaunchForce;
            ShootingAudio.clip = ChargingClip;  // Get the Charging Clip and Play it.
            ShootingAudio.Play();
        }
        else if(Input.GetButton(FireButton) && !Fired)    // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        {
            CurrentLaunchForce += (ChargeSpeed * Time.deltaTime);     // Increment the launch force.
            AimSlider.value = CurrentLaunchForce;   // And update the slider.
        }
        else if(Input.GetButtonUp(FireButton) && !Fired)  // We Released the button , not Yet Fired.
        {
            Fire();  // Release The Shell.
        }
	}

    private void Fire()  // Firing the Shell
    {
        Fired = true;

        Rigidbody ShellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;  // Create an instance of the shell and store a reference to it's rigidbody.

        ShellInstance.velocity = CurrentLaunchForce * FireTransform.forward;  // Set the shell's velocity to the launch force in the fire position's forward direction.

        ShootingAudio.clip = FireClip;  // Change the clip to the firing clip and play it.
        ShootingAudio.Play();

        CurrentLaunchForce = MinLaunchForce;  // Saftey. Precaution in case of missing button events.
    }
}
