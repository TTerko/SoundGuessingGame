using UnityEngine;
using System.Collections;

public static class BasicObjects
{
	private static PanelManager panelManager = null;

	public static PanelManager PanelManager
	{
		get
		{
			if (panelManager == null)
			{
				panelManager = GameObject.FindObjectOfType<PanelManager>();
			}

			return panelManager;
		}

		private set
		{
			panelManager = value;
		}
	}

	public static void Reinitialize()
	{
		panelManager = null;
	}
}