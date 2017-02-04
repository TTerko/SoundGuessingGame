using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PanelManager : MonoBehaviour
{
	[HideInInspector]
	public Animator OpenedPanelAnimator;
	[HideInInspector]
	public Animator OpenedPopupAnimator;

	private GenericPanelBehaviour previousPanel;

	private GenericPanelBehaviour[] panels;

	private void Awake()
	{
		panels = GameObject.FindObjectsOfType<GenericPanelBehaviour>();

		var defaultAnimator = Resources.Load("Animators/GenericPanelController") as RuntimeAnimatorController;

		foreach (var panel in panels)
		{
			if (panel.GetComponent<Animator>().runtimeAnimatorController == null)
			{
				panel.GetComponent<Animator>().runtimeAnimatorController = defaultAnimator;
			}
		}
	}

	public void UnRegisterPanel(GenericPanelBehaviour panelToUnRegister)
	{
		List<GenericPanelBehaviour> panelsList = panels.ToList();
		panelsList.Remove(panelToUnRegister);
		panels = panelsList.ToArray();
	}

	public void RegisterPanel(GenericPanelBehaviour panelToRegister)
	{
		foreach (var panel in panels)
		{
			if (panel == panelToRegister)
			{
				return;
			}
		}

		List<GenericPanelBehaviour> panelsList = panels.ToList();
		panelsList.Add(panelToRegister);
		panels = panelsList.ToArray();
	}

	private int GetNumberOfOpenedParents(GenericPanelBehaviour panel)
	{
		var parent = panel.transform.parent;
		var result = 0;

		while (parent != null)
		{
			if (parent.GetComponent<GenericPanelBehaviour>() != null)
			{
				var parentPanel = parent.GetComponent<GenericPanelBehaviour>();
				result += (parentPanel.IsOpened() && parentPanel.GetType() != panel.GetType()) ? 1 : 0;
			}

			parent = parent.transform.parent;
		}

		return result;
	}

	private int GetNumberOfParents(GenericPanelBehaviour panel)
	{
		var parent = panel.transform.parent;
		var result = 0;

		while (parent != null)
		{
			result++;
			parent = parent.transform.parent;
		}

		return result;
	}

	private bool HasParent(GenericPanelBehaviour panel, MonoBehaviour parentToCheck = null)
	{
		var parent = panel.transform.parent;
		if (parentToCheck == null)
		{
			return true;
		}

		while (parent != null)
		{
			if (parent == parentToCheck.transform)
			{
				return true;
			}

			parent = parent.transform.parent;
		}

		return false;
	}

	public T GetPanel<T>() where T : GenericPanelBehaviour
	{
		GenericPanelBehaviour panel = null;
		var maxOpenedParents = int.MinValue;
		var minParents = int.MaxValue;
		List<GenericPanelBehaviour> panelsWithMaxOpenedParents = new List<GenericPanelBehaviour>();

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && openedParents > maxOpenedParents)
			{
				maxOpenedParents = openedParents;
			}
		}

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && openedParents == maxOpenedParents)
			{
				panelsWithMaxOpenedParents.Add(panels[i]);
			}
		}


		for (int i = 0; i < panelsWithMaxOpenedParents.Count; i++)
		{
			var parents = GetNumberOfParents(panelsWithMaxOpenedParents[i]);
			if (panelsWithMaxOpenedParents[i] is T && parents < minParents)
			{
				panel = panelsWithMaxOpenedParents[i];
				minParents = parents;
			}
		}

		return panel as T;
	}

	public T GetChildPanel<T>(MonoBehaviour parent) where T : GenericPanelBehaviour
	{
		GenericPanelBehaviour panel = null;
		var maxOpenedParents = int.MinValue;
		var minParents = int.MaxValue;
		List<GenericPanelBehaviour> panelsWithMaxOpenedParents = new List<GenericPanelBehaviour>();

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && HasParent(panels[i], parent) && openedParents > maxOpenedParents)
			{
				maxOpenedParents = openedParents;
			}
		}

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && HasParent(panels[i], parent) && openedParents == maxOpenedParents)
			{
				panelsWithMaxOpenedParents.Add(panels[i]);
			}
		}

		for (int i = 0; i < panelsWithMaxOpenedParents.Count; i++)
		{
			var parents = GetNumberOfParents(panelsWithMaxOpenedParents[i]);
			if (panelsWithMaxOpenedParents[i] is T && parents < minParents)
			{
				panel = panelsWithMaxOpenedParents[i];
				minParents = parents;
			}
		}

		return panel as T;
	}

	public Animator GetAnimator<T>() where T : GenericPanelBehaviour
	{
		for (int i = 0; i < panels.Length; i++)
		{
			if (panels[i] is T)
			{
				return panels[i].GetComponent<Animator>();
			}
		}

		return null;
	}

	private Animator GetAnimator(GenericPanelBehaviour panel)
	{
		for (int i = 0; i < panels.Length; i++)
		{
			if (panels[i] == panel)
			{
				return panels[i].GetComponent<Animator>();
			}
		}

		return null;
	}

	public bool IsOpen<T>() where T : GenericPanelBehaviour
	{
		return GetPanel<T>().IsOpened();
	}

	public void OnOpenPanelClick(GenericPanelBehaviour panel) 
	{
		panel.Open();
	}

	public void OnClosePanelClick(GenericPanelBehaviour panel) 
	{
		panel.Close();
	}

	public void OnTriggerPanelClick(GenericPanelBehaviour panel) 
	{
		panel.Trigger();
	}

	public GenericPanelBehaviour TriggerPanel(GenericPanelBehaviour panel) 
	{
		panel.Trigger();

		return panel;
	}

	public T TriggerPanel<T>() where T : GenericPanelBehaviour
	{
		GetPanel<T>().Trigger();

		return GetPanel<T>();
	}

	public GenericPanelBehaviour OpenPanel(GenericPanelBehaviour panel) 
	{
		panel.Open();

		return panel;
	}

	public T OpenPanel<T>() where T : GenericPanelBehaviour
	{
		GetPanel<T>().Open();

		return GetPanel<T>();
	}

	public GenericPanelBehaviour ClosePanel(GenericPanelBehaviour panel) 
	{
		Animator panelAnimator = GetAnimator(panel);

		panel.Close();

		return panel;
	}

	public T ClosePanel<T>() where T : GenericPanelBehaviour
	{
		Animator panelAnimator = GetAnimator<T>();

		GetPanel<T>().Close();

		return panelAnimator.GetComponent<T>();
	}

	void Update()
	{
		foreach (var panel in panels)
		{
			panel.SendMessage("BaseUpdate", SendMessageOptions.DontRequireReceiver);
		}
	}
}