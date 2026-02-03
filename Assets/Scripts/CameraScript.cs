using UnityEngine;
using UnityEngine.InputSystem;
public class CameraScript : MonoBehaviour
{
    private InputSystem_Actions input;
    public Camera cam;
    public Transform player;
    public float sensibility = 0.15f;
    public float distance = 2f;
    float speed = 3f;

    float x;
    float y;

    Vector3 cameraPlace;
    Quaternion rotation;
    Vector3 offset;
    Vector3 dir;

    public LayerMask layerMask;

    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
        cam = Camera.main;
        x = cam.transform.rotation.x;
        y = cam.transform.rotation.y;
    }

    void Update()
    {
        Vector2 mouse = Mouse.current.delta.ReadValue() * sensibility;

        x += mouse.x;
        y -= mouse.y;

        rotation = Quaternion.Euler(y, x, 0f);

        offset = rotation * new Vector3(0f, 0f, -distance);

        cameraPlace = player.position + offset;       

        RaycastHit hit;
        dir = (cameraPlace - player.position).normalized;

        if (Physics.SphereCast(player.position, 0.2f, dir, out hit, distance, layerMask)){
            cameraPlace = hit.point;
        }
        else{
            cameraPlace = Vector3.Lerp(transform.position, cameraPlace, Time.deltaTime * speed);
        }

        transform.position = cameraPlace;
        transform.LookAt(player.position);
    }
}
