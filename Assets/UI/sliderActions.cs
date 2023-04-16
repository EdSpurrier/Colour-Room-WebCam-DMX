using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class sliderActions : MonoBehaviour {

	public InputField textField;
	public Slider slider;

	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider>();

		updateInputValue();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateSliderValue()
	{

		textField.text = Convert.ToString(slider.value);

	}

	public void updateInputValue () {

		if (textField.text != "") {
			int thisValue = int.Parse(textField.text);
			if (thisValue <= 255 && thisValue >= 0) {
				slider.value = thisValue;
			};
		}
		
	}
}
