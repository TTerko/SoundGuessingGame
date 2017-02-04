using UnityEngine;
using UnityEditor;
using System.Collections;

public class MenuItemsExtention 
{
	#region Unity Methods
	#endregion

	#region Public Methods

	#endregion

	#region Private Methods
	[MenuItem("GameObject/ActivatePanel #w")]
	private static void ActivatePanel()
	{
		var go = Selection.activeObject as GameObject;

		bool valid = (go != null) && (go.GetComponent<CanvasGroup>() != null) && (go.GetComponent<Animator>() != null);
		if(valid)
		{
			var thisCg = go.GetComponent<CanvasGroup>();
			Transform parent = go.transform.parent;
			CanvasGroup cg;
			foreach (Transform child in parent )
			{
				cg = child.gameObject.GetComponent<CanvasGroup>();
				if (cg != null && thisCg != cg)
				{
					cg.alpha = 0.0f;
					cg.interactable = false;
				}
			}

			thisCg = go.GetComponent<CanvasGroup>();
			if (thisCg != null)
			{
				if (thisCg.alpha == 0.0f)
				{
					thisCg.alpha = 1.0f;
					thisCg.interactable = true;
				}
				else
				{
					thisCg.alpha = 0.0f;
					thisCg.interactable = false;
				}
			}
		}
	}
	#endregion
}