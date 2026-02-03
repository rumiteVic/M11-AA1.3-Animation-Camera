using UnityEngine;

public class RaycastUI : MonoBehaviour
{
    public Camera cam;
    public RaycastLookAt raycast;
    public RectTransform uiPoint;

    Vector3 whereLooking;
    Vector3 screenPosPoint;

    // Update is called once per frame
    void Update()
    {
        whereLooking = raycast.lookingAt;
        screenPosPoint = cam.WorldToScreenPoint(whereLooking);

        uiPoint.position = screenPosPoint;
    }
}
