using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Turns off Cursor
        Cursor.visible = false;
        //Keeps Cursor inside application
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        //If a weirdo wants inverted controls
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        //Rotate on Y axis
        transform.parent.Rotate(Vector3.up * mouseX);

    }
}
