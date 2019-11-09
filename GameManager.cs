using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    public int NumRoundsToWin = 5;  // Number of Rounds a Single Player has to Win in order to win the game.
    public float StartDelay = 3f;  // The Delay between The START of the Round and Playing the Round.
    public float EndDelay = 3f;   // The Delay between the END of the Round and Ending the Round.
    public CameraControl CamControl; // Reference to the CameraControl Script.
    public Text MessageText;      // Reference to the Overlay Text to display Winning Text , etc.
    public GameObject TankPrefab;  // Reference to the Prefab the Players will Control.
    public TankManager[] Tanks;  // Collection of Managers that Enable and Disable different Functionalitites of the Tank.

    private int RoundNumber;   // Which Round the Game is Currently in.
    private WaitForSeconds StartWait; // Delay While the Round Starts.
    private WaitForSeconds EndWait;  // Delay while the Round Ends.
    private TankManager RoundWinner;   // Reference to the Winner of the Current Round.
    private TankManager GameWinner;  // Reference to the Winner of the whole GAME.


	private void Start ()  // Create the Delays so they are made once at the START() Func.
    {
        StartWait = new WaitForSeconds(StartDelay);
        EndWait = new WaitForSeconds(EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(Gameloop());   // Once all tanks have been Created and the Camera is using them as Targets , START THE GAME !
	}


    private void SpawnAllTanks()
    {
        int i;
        for(i=0;i<Tanks.Length;i++)
        {
            Tanks[i].Instance = Instantiate(TankPrefab, Tanks[i].SpawnPlayer.position, Tanks[i].SpawnPlayer.rotation) as GameObject; // ... create them, set their player number and references needed for control.
            Tanks[i].PlayerNumber = i + 1;  // A minimum of 1 Player.
            Tanks[i].SetUp();   // SetUp Function from The TankManager Script.
        }
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[Tanks.Length]; //Create a collection of transforms the same size as the number of tanks.
        int i;
        for(i=0;i<targets.Length;i++)
        {
            targets[i] = Tanks[i].Instance.transform;  // ... set it to the appropriate tank transform.
            CamControl.Targets = targets;   // Finally These are the Targets that the Camera Must FOLLOW.
        }
    }

    /*
     StartCoroutine(GameLoop());  - Coroutine ->  When you pause your Code for a certain amount of time at YIELD -> and then come back to the point before stoping and continue the code.
         */

    private IEnumerator Gameloop()  // Game Loop Pattern   1) RoundSTARTING --> 2) RoundPLAYING --> 3) RoundENDING.
    {
        yield return StartCoroutine(RoundStarting());  // While in the Code .... Stop for a moment to START the round(return) ...... then Wait at the next YIELD for the PLAYING(return) ..... then finally Wait the next YIELD for ENDING.
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if(GameWinner != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            StartCoroutine(Gameloop());   // If there is No Winner.
        }
    }


    private IEnumerator RoundStarting()  // As soon as the round starts reset the tanks and make sure they can't move.
    {
        ResetAllTanks();
        DisableTankControl();

        CamControl.SetStartPositionAndSize();  // Snap the camera's zoom and position to something appropriate for the reset tanks.

        RoundNumber++;  // Increment Round Number
        MessageText.text = "ROUND" + RoundNumber;  // Display a Text with the new Round Number;

        yield return StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();   // As Soon as the Round Starts the Player must be able to control the Tank.

        MessageText.text = string.Empty;  // Clear the Text from the Screen.

        while(!OneTankLeft())  // While there is not one tank left..
        {
            yield return null;  // ... return on the next frame.
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();  // Disable All Controls.

        RoundWinner = null;   // Clear the Winner of the PREVIOUS Round.
        RoundWinner = GetRoundWinner();   // See the Winner of THIS Round.

        if(RoundWinner != null)    // If there is a Winner...
            RoundWinner.Wins++;  // Increment Their Total Score.

        GameWinner = GetGameWinner();  // Check if Someone has won the Game.

        string Message = EndMessage();
        MessageText.text = Message;

        yield return EndWait;

    }

    private bool OneTankLeft()
    {
        int NumTanksLeft = 0; 
        int i;

        for(i=0;i<Tanks.Length;i++)   // Go through all the Tanks...
        {
            if (Tanks[i].Instance.activeSelf)  // Check to see if any of them is ACTIVE.
                NumTanksLeft++;
        }

        return NumTanksLeft <= 1;  // If one or Fewer are Active Return TRUE , otherwise return False.
    }

    private TankManager GetRoundWinner() // This function is to find out if there is a winner of the round.                                      
                                        // This function is called with the assumption that 1 or fewer tanks are currently active.
    {
        int i;
        for(i=0;i<Tanks.Length;i++) // Go Through all the Tanks....
        {
            if (Tanks[i].Instance.activeSelf)   // If one is Still Active...
                return Tanks[i];   // Then Return the Winner.
        }
        return null;    // If None of the Tanks is Still Active , then Return Nothing.
    }

    private TankManager GetGameWinner()     // This function is to find out if there is a winner of the game.
    {
        int i;
        for(i=0;i<Tanks.Length;i++)  // Go through all the Tanks...
        {
            if (Tanks[i].Wins == NumRoundsToWin)    // If one of the Tanks has the Appropriate Number of Rounds to Win the Game...
                return Tanks[i];  // Return It.
        }
        return null;  // Else return Nothing.
    }

    private string EndMessage()     // Returns a string message to display at the end of each round.
    {
        string MSG = "DRAW!";  // The Default Message.
        int i;
        
        if(RoundWinner != null)         // If there is a winner then change the message to reflect that.
            MSG = RoundWinner.ColoredPlayerText + " WINS THE ROUND !";
            MSG += "\n \n \n \n";
        
        for(i=0;i<Tanks.Length;i++)   // Go through all the Tanks...
        { 
            MSG += Tanks[i].ColoredPlayerText + " : " + Tanks[i].Wins + " WINS !\n";  // Add Each of their Scores to the Message.
        }

        if (GameWinner != null)   // If there is a GAME Winner....
            MSG = GameWinner.ColoredPlayerText + " WINS THE GAME !";

        return MSG;
    }

    private void ResetAllTanks() // This function is used to turn all the tanks back on and reset their positions and properties.
    {
        int i;
        for(i=0;i<Tanks.Length;i++)
        {
            Tanks[i].Reset();   // Function from the TANKMANAGER Class.
        }
    }

    private void EnableTankControl()
    {
        int i;
        for(i=0;i<Tanks.Length;i++)
        {
            Tanks[i].EnableControls();
        }
    }

    private void DisableTankControl()
    {
        int i;
        for (i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].DisableControls();
        }
    }
}