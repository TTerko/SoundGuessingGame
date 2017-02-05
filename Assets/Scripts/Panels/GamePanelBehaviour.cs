using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task
{
	public string OptionA;
	public string OptionB;

	public List<AudioClip> OptionAClips;
	public List<AudioClip> OptionBClips;

	public Task(string optionA, string optionB)
	{
		this.OptionA = optionA;
		this.OptionB = optionB;

		this.OptionAClips = new List<AudioClip>();
		this.OptionBClips = new List<AudioClip>();
	}

	public void AddAClip(AudioClip audioClip)
	{
		OptionAClips.Add(audioClip);
	}

	public void AddBClip(AudioClip audioClip)
	{
		OptionBClips.Add(audioClip);
	}

}

public class GamePanelBehaviour : GenericPanelBehaviour 
{
	public AudioClip AClip;
	public AudioClip BClip;

	private Task simpleTask;

	void OnOpen()
	{
		simpleTask = new Task("DOG", "BOG");
		simpleTask.AddAClip(AClip);
		simpleTask.AddBClip(BClip);
	}
}
