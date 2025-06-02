using UnityEngine;
using System.Collections;

public class ScreenStateManager : Singleton<ScreenStateManager>
{
	protected ScreenStateManager() { } // guarantee this will be always a singleton only - can't use the constructor!

	// -------------------------------------------------------------------------------------------
	public StandardUIManager m_xStandardUIManager;
	private ScreenStateBase m_xCurrentScreenState = null;
#if UNITY_EDITOR
	public ScreenStates xEditorStartScreenState;
#endif
	public enum ScreenStates
	{
		None = 0,
		Intro,
		Menu,
		Loding,
		Game,
		Editor,
	}

	// -------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start ()
	{
		WorldStateManager.GamePaused = true;
#if UNITY_EDITOR
		m_xCurrentScreenState = CreateScreenState(xEditorStartScreenState);
#else
		m_xCurrentScreenState = CreateScreenState(ScreenStates.Intro);
#endif
		m_xCurrentScreenState.Show();
	}
	
	// -------------------------------------------------------------------------------------------
	void Update () 
	{
		if (m_xCurrentScreenState != null)
			m_xCurrentScreenState.Update();
	}

	// -------------------------------------------------------------------------------------------
	public void ChangeToScreenState(ScreenStates eNextState)
	{
		if (m_xCurrentScreenState != null)
		{
			m_xCurrentScreenState.Hide();
		}

		// No need to clear UI explicitly anymore, each screen state handles its own UI

		m_xCurrentScreenState = CreateScreenState(eNextState);

		m_xCurrentScreenState.Show();
	}

	// -------------------------------------------------------------------------------------------
	private ScreenStateBase CreateScreenState(ScreenStates eScreenState)
	{
		switch (eScreenState)
		{
			case ScreenStates.Menu:
				return new ScreenStateMenu(ScreenStates.Menu, this);

			case ScreenStates.Game:
				return new ScreenStateGame(ScreenStates.Game, this);

			case ScreenStates.Loding:
				return new ScreenStateLoading(ScreenStates.Loding, this);

			case ScreenStates.Intro:
				return new ScreenStateIntro(ScreenStates.Intro, this);

			case ScreenStates.Editor:
			case ScreenStates.None:
			default:
				BlockBall.Debug.ASSERT(false);
				throw new System.Exception("Unknown ScreenState");
		}
	}

	// -------------------------------------------------------------------------------------------
	public bool GamePaused()
	{
		return m_xCurrentScreenState.m_Type != ScreenStates.Game;  
	}
	// -------------------------------------------------------------------------------------------
}
