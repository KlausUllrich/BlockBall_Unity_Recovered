using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BlockBall;

public class WorldStateManager : MonoBehaviour
{

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		GamePaused = true;
	}

    // -------------------------------------------------------------------------------------------
	public static bool GamePaused
	{
		get
		{
			return Time.timeScale == 0.0f;
		}

		set
		{
			Time.timeScale = value ? 0.0f : 1.0f;
		}
	}

    // -------------------------------------------------------------------------------------------
}
