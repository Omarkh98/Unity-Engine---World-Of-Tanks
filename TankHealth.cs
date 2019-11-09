using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // Importatnt When Using UI.

public class TankHealth : MonoBehaviour {

    public float StartingHealth = 100f;
    public Slider slider;
    public Image FillImage;
    public Color FullHealth = Color.green;
    public Color ZeroHealth = Color.red;
    public GameObject ExplosionPrefab;

    private AudioSource ExplosionAudio;
    private ParticleSystem ExplosionParticles;
    private float CurrentHealth;
    private bool Dead;


    private void Awake()     // It's better to Spawn it in the game or use it whenever I want , instead of using the Destory() function which INFLATE the Memory , thus making the game Slower.
    {
        ExplosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();   // Instantiate || Spawn the Explosion Prefab and get a reference of the Particle System.

        ExplosionAudio = ExplosionParticles.GetComponent<AudioSource>();  // Get a Reference to the Audio Source of the Instantiated Prefab.

        ExplosionParticles.gameObject.SetActive(false);   // Make Sure the The Explosion doesn't happen at the start of the game.
    }

    private void OnEnable()   // At the Start of the Game.
    {
        CurrentHealth = StartingHealth;
        Dead = false;

        SetHealthUI();
    }

    public void TakeDamage (float Amount)  // When the Tank Takes Damage.
    {
        CurrentHealth -= Amount;

        SetHealthUI();

        if(CurrentHealth <= 0f && !Dead)  // Just to make sure the Tank is DEAD!
        {
            OnDeath();
        }
    }
     
    private void SetHealthUI()   // Set the Value and Color of the Slider.
    {
        slider.value = CurrentHealth;  // The Slider Value and Color Must Match  the Health of the Tank.

        FillImage.color = Color.Lerp(ZeroHealth, FullHealth, CurrentHealth / StartingHealth);  // Lerp Takes 3 parameters.
    }

    private void OnDeath()  // When Tank is Dead --> CurrentHealth = 0.
    {
        Dead = true;

        ExplosionParticles.transform.position = transform.position;  // Get Position of Particles.
        ExplosionParticles.gameObject.SetActive(true);

        ExplosionParticles.Play(); // Play Particles Sound.
        ExplosionAudio.Play(); // Play Explosion Sound.

        gameObject.SetActive(false);  // Finally The Tank is No Longer Active.
    }
}
