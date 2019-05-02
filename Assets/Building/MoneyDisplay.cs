using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    public UnityEngine.UI.Text textObject;

	void Update ()
    {
        textObject.text = "Scrap: " + MoneyHandler.Money;	
	}
}