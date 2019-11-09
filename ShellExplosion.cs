using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{

    public LayerMask TankMask;   // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem ExplosionParticles;
    public AudioSource ExplosionAudio;
    public float MaxDamage = 100f;
    public float ExplosionForce = 1000f;
    public float MaxLifeTime = 2f;
    public float ExplosionRadius = 5f;


   private  void Start()  // Destory thhe Shell Gameobject if it is still alive after 2 secs.
    {
        Destroy(gameObject, MaxLifeTime);
    }

    private void OnTriggerEnter(Collider Other)  // Find All TANKS in an area around a Shell and Damage Them.
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);   // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        int i;

        for(i=0;i<Colliders.Length;i++)  // Go through all Colliders.
        {
            Rigidbody TargetRigidBody = Colliders[i].GetComponent<Rigidbody>();

            if (!TargetRigidBody)
                continue;

            TargetRigidBody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);  // Add an explosion force to the FOUND Shell.

            TankHealth TargetHealth = TargetRigidBody.GetComponent<TankHealth>();   // Find the TankHealth script associated with the rigidbody.

            if (!TargetHealth)
                continue;

            float Damage = CaculateDamage(TargetRigidBody.position);  // Calculate the Amount of damage.

            TargetHealth.TakeDamage(Damage);  // Apply the Damage To the TANK.
        }

        ExplosionParticles.transform.parent = null;   // Unparent the particles from the shell.

        ExplosionParticles.Play();

        ExplosionAudio.Play();

        Destroy(ExplosionParticles.gameObject, ExplosionParticles.main.duration);     // Once the particles have finished, destroy the gameobject they are on.

        Destroy(gameObject);    // Destroy the shell.
    }

    private float CaculateDamage(Vector3 TargetPosition)   // Calculate the Amount of Damage a tank should Take Based on it's Position.
    {
        Vector3 ExplosionToTarget = TargetPosition - transform.position;

        float ExplosionDistance = ExplosionToTarget.magnitude;

        float RelativeDistance = (ExplosionRadius - ExplosionDistance) / ExplosionRadius;   // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.

        float Damage = RelativeDistance * MaxDamage;

        Damage = Mathf.Max(0f, Damage); // Make Sure Damage is Not Negative;

        return Damage;
    }
}
