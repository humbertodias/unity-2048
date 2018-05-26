using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour {

	public GameObject on;
	public GameObject off;

	private Toggle toggle;

	void Start()
	{
		toggle = GetComponent<Toggle> ();
		OnChangeValue ();
	}

	public void OnChangeValue(){
		on.SetActive (toggle.isOn);
		off.SetActive (!toggle.isOn);
	}
}
