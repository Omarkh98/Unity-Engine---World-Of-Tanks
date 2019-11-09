using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirectionalControl : MonoBehaviour
{   
    // This class is used to make sure world space UI elements such as the health bar face the correct direction.

    public bool UseRelativeRotation = true;

    private Quaternion RelativeRotation;
	
	void Start ()
    {
        RelativeRotation = transform.parent.localRotation;   // Local Rotation at the start of the Scene.
	}
	
	
	void Update ()
    {
	   if(UseRelativeRotation)
        {
            transform.rotation = RelativeRotation;
        }	
	}
}
