using UnityEngine;
using System;
using System.Net;
using System.Collections;

public partial class ScreenStateBase
{
	// -------------------------------------------------------------------------------------------
	protected ScreenStateManager m_xScreenStateManager = null;

	// -------------------------------------------------------------------------------------------
	public ScreenStateManager.ScreenStates m_Type;

	// -------------------------------------------------------------------------------------------
	public ScreenStateBase(ScreenStateManager.ScreenStates xType, ScreenStateManager xScreenStateManager)
	{
		m_xScreenStateManager = xScreenStateManager;
		m_Type = xType;
	}

	// -------------------------------------------------------------------------------------------
}

//--------------------------------------------------------------------------------------------------------
// Hier werden nur die abstrakten Funktionen definiert 
//--------------------------------------------------------------------------------------------------------
public abstract partial class ScreenStateBase : MarshalByRefObject
{
	// -------------------------------------------------------------------------------------------
	// Use this for initialization
	public abstract void Show();

	// -------------------------------------------------------------------------------------------
	public abstract void Hide();

	// -------------------------------------------------------------------------------------------
	public abstract void Update();

	// -------------------------------------------------------------------------------------------
}
