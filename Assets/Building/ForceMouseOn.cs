using UnityEngine;

public class ForceMouseOn : MonoBehaviour
{
	void Awake ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
