using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {

        transform.forward = cameraTransform.forward;
    }
}


