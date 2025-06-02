using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenStateLoading : ScreenStateBase 
{
	public ScreenStateLoading(ScreenStateManager.ScreenStates xType, ScreenStateManager xScreenStateManager)
		: base(xType, xScreenStateManager)
	{
	}
		
	// -------------------------------------------------------------------------------------------
	// Use this for initialization
	public override void Show ()
	{
		WorldStateManager.GamePaused = true;
		var t = GameObject.Find("BlockMerger_TEST").GetComponent<BlockMerger>();
		t.LoadLevel();
	}

	// -------------------------------------------------------------------------------------------
	public override void Hide()
	{

	}

	// -------------------------------------------------------------------------------------------
	// Update is called once per frame
	public override void Update ()
	{

		m_xScreenStateManager.ChangeToScreenState(ScreenStateManager.ScreenStates.Game);
	}

	// -------------------------------------------------------------------------------------------
}
