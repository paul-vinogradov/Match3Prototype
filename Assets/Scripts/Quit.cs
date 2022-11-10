using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour {

	//обычный выхожд, ниче особенного
	void Update ()
	{
		if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
	}
}
