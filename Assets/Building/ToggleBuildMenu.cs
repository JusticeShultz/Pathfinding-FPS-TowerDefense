using UnityEngine;

public class ToggleBuildMenu : MonoBehaviour
{
    public static bool TabOpen = false;

    public GameObject Menu;

    private void Awake()
    {
        TabOpen = false;    
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Menu.SetActive(!Menu.activeSelf);

            if(Menu.activeSelf)
            {
                TabOpen = true;
                Cursor.visible = TabOpen;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                TabOpen = false;
                Cursor.visible = TabOpen;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
	}
}
