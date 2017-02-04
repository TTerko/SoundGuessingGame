using UnityEngine;
using System.Collections;

public enum PanelType
{
	Undefined,
	Panel,
	Popup,
	NotDependentPanel
}

[RequireComponent(typeof(CanvasGroup), typeof(Animator))]
public class GenericPanelBehaviour : MonoBehaviour
{
	public PanelType PanelType = PanelType.Panel;

	public bool OpenWithParent = false;
	public bool OpenWithChild = false;

	public bool OpenOnAwake = false;

	public bool IsBillboard = false;

	public GameObject BillBoardTarget;

	[HideInInspector]
	public GenericPanelBehaviour PreviousPanel = null;

	private const string IS_OPENED = "IsOpened";
	private const string ON_OPEN = "OnOpen";
	private const string ON_CLOSE = "OnClose";
	private const string ON_BASE_OPEN = "OnBaseOpen";
	private const string ON_BASE_CLOSE = "OnBaseClose";

	private const string PANEL_UPDATE = "PanelUpdate";

	void Awake()
	{
		RegisterIfNecessary();

		if (OpenOnAwake)
		{
			Open();
		}
	}

	private void RegisterIfNecessary()
	{
		BasicObjects.PanelManager.RegisterPanel(this);
	}

	private void OnDestroy()
	{
		if (BasicObjects.PanelManager != null)
		{
			BasicObjects.PanelManager.UnRegisterPanel(this);
		}
	}

	public void Trigger()
	{
		if (!IsOpened())
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	public void Open()
	{
		GenericPanelBehaviour[] sameLevelPanels = transform.parent.GetComponentsInChildren<GenericPanelBehaviour>();

		foreach (GenericPanelBehaviour sameLevelPanel in sameLevelPanels)
		{
			if (sameLevelPanel.transform.parent != this.transform.parent)
			{
				continue;
			}

			var sameLevelPanelAnimator = sameLevelPanel.GetComponent<Animator>();

			if (sameLevelPanelAnimator.GetBool(IS_OPENED) && (PanelType == sameLevelPanel.PanelType && PanelType != PanelType.NotDependentPanel) && sameLevelPanel != this)
			{
				sameLevelPanelAnimator.SetBool(IS_OPENED, false);
				sameLevelPanelAnimator.SendMessage(ON_CLOSE, SendMessageOptions.DontRequireReceiver);
				sameLevelPanelAnimator.SendMessage(ON_BASE_CLOSE, SendMessageOptions.DontRequireReceiver);
				PreviousPanel = sameLevelPanel;
			}
		}

		var panelAnimator = GetComponent<Animator>();

		if (!panelAnimator.GetBool(IS_OPENED))
		{
			panelAnimator.SetBool(IS_OPENED, true);
			panelAnimator.SendMessage(ON_OPEN, SendMessageOptions.DontRequireReceiver);
			panelAnimator.SendMessage(ON_BASE_OPEN, SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnBaseOpen()
	{
		GenericPanelBehaviour[] childPanels = transform.GetComponentsInChildren<GenericPanelBehaviour>();

		foreach (GenericPanelBehaviour childPanel in childPanels)
		{
			if (!CheckIfChildPanel(childPanel))
			{
				continue;
			}

			var childPanelAnimator = childPanel.GetComponent<Animator>();

			if (!childPanelAnimator.GetBool(IS_OPENED) && childPanel.OpenWithParent)
			{
				childPanel.Open();
			}
		}

		var cursor = this.transform.parent;

		while (cursor.parent != null)
		{
			GenericPanelBehaviour[] parentPanels = cursor.parent.GetComponentsInChildren<GenericPanelBehaviour>();

			foreach (GenericPanelBehaviour parentPanel in parentPanels)
			{
				if (!CheckIfParentPanel(parentPanel))
				{
					continue;
				}

				var childPanelAnimator = parentPanel.GetComponent<Animator>();

				if (!childPanelAnimator.GetBool(IS_OPENED) && parentPanel.OpenWithChild)
				{
					parentPanel.Open();
				}
			}
			cursor = cursor.parent;
		}

	}

	public void Close()
	{
		var panelAnimator = GetComponent<Animator>();

		if (panelAnimator.GetBool(IS_OPENED))
		{
			panelAnimator.SetBool(IS_OPENED, false);
			panelAnimator.SendMessage(ON_CLOSE, SendMessageOptions.DontRequireReceiver);
			panelAnimator.SendMessage(ON_BASE_CLOSE, SendMessageOptions.DontRequireReceiver);
		}
	}

	public bool IsOpened()
	{
		return GetComponent<Animator>().GetBool(IS_OPENED);
	}

	void OnBaseClose()
	{
		GenericPanelBehaviour[] childPanels = transform.GetComponentsInChildren<GenericPanelBehaviour>();

		foreach (GenericPanelBehaviour childPanel in childPanels)
		{
			if (!CheckIfChildPanel(childPanel))
			{
				continue;
			}

			childPanel.Close();
		}
	}
	bool CheckIfParentPanel(GenericPanelBehaviour panel)
	{
		var parent = this.transform.parent;

		while (parent != null)
		{
			if (parent != panel.transform && parent.GetComponent<GenericPanelBehaviour>() == null)
			{
				parent = parent.transform.parent;
			}
			else if (parent != panel.transform && parent.GetComponent<GenericPanelBehaviour>() != null)
			{
				return false;
			}
			else if (parent == panel.transform)
			{
				return true;
			}
		}

		return false;
	}
	bool CheckIfChildPanel(GenericPanelBehaviour panel)
	{
		var parent = panel.transform.parent;

		while (parent != null)
		{
			if (parent != this.transform && parent.GetComponent<GenericPanelBehaviour>() == null)
			{
				parent = parent.transform.parent;
			}
			else if (parent != this.transform && parent.GetComponent<GenericPanelBehaviour>() != null)
			{
				return false;
			}
			else if (parent == this.transform)
			{
				return true;
			}
		}

		return false;
	}

	void BaseUpdate()
	{
		if (IsBillboard)
		{
			var target = BillBoardTarget == null ? Camera.main.transform : BillBoardTarget.transform;

			transform.LookAt(transform.position + target.rotation  * Vector3.forward,
				target.rotation * Vector3.up);
		}

		if (IsOpened())
		{
			SendMessage(PANEL_UPDATE, SendMessageOptions.DontRequireReceiver);
		}
	}
}
