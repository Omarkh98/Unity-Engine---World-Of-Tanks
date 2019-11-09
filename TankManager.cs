using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TankManager
{   
      // This class is to manage various settings on a tank.
     // It works with the GameManager class to control how the tanks behave
    // and whether or not players have control of their tank in the 
   // different phases of the game.

    public Color PlayerColor;     // Color of Tank.
    public Transform SpawnPlayer;   // Spwan Position of Each Tank.
    [HideInInspector] public int PlayerNumber;
    [HideInInspector] public string ColoredPlayerText;
    [HideInInspector] public GameObject Instance;
    [HideInInspector] public int Wins;

    private TankMovement Movement;    // Reference to the Tank's Movement Script.
    private TankShooting Shooting;    // Reference to the Tank's Shooting Script.
    private GameObject CanvasGameObject;  

    public void SetUp()   // Get references to the components.
    {
        int i;
        Movement = Instance.GetComponent<TankMovement>();
        Shooting = Instance.GetComponent<TankShooting>();
        CanvasGameObject = Instance.GetComponentInChildren<Canvas>().gameObject;

        Movement.PlayerNumber = PlayerNumber;
        Shooting.PlayerNumber = PlayerNumber;

        ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(PlayerColor) + ">PLAYER " + PlayerNumber + "</color>";   // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.

        MeshRenderer[] Renderers = Instance.GetComponentsInChildren<MeshRenderer>(); // Get all of the renderers or Parts of the tank.

        for (i=0;i<Renderers.Length;i++)   // .. Go through Them all...
        {
            Renderers[i].material.color = PlayerColor;   // Color them all with the Color Selected by the Player Earlier.
        }
    }

    public void DisableControls()   // Used during phases where the player SHOULDN'T , or Isn't Allowed to Do anything.
    {
        Movement.enabled = false;
        Shooting.enabled = false;

        CanvasGameObject.SetActive(false);

    }

    public void EnableControls()   // Used during phases where the Player SHOULD be able to Control his Tank.
    {
        Movement.enabled = true;
        Shooting.enabled = true;

        CanvasGameObject.SetActive(false);
    }

	public void Reset ()   // Used to Reset all Positions and Spawning Points to their Origins.  DEFAULT State.
    {
        Instance.transform.position = SpawnPlayer.position;
        Instance.transform.rotation = SpawnPlayer.rotation;

        Instance.SetActive(false);
        Instance.SetActive(true);
    }
}
