using UnityEngine;
using UnityEngine.InputSystem;
public class CameraScript : MonoBehaviour
{
    private InputSystem_Actions input;
    public Camera cam;
    public Transform player;
    public float sensibility = 0.15f;
    public float distance = 8f;
    float speed = 3f;

    float x;
    float y;

    Vector3 cameraPlace;
    Quaternion rotation;
    Vector3 offset;
    Vector3 dir;
    public LayerMask layerMask;

    Vector3 cameraOffset;

    void Start()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
        cam = Camera.main;
        x = cam.transform.rotation.x;
        y = cam.transform.rotation.y;
    }

    void Update()
    {
        //Obtiene la posición del mouse en la pantalla y se multiplica por la sensibilidad
        Vector2 mouse = Mouse.current.delta.ReadValue() * sensibility;

        //Se cambia la rotación de la x y las y en base de la posición del mouse
        x += mouse.x;
        y -= mouse.y;

        //Aquí se guarda la rotación de x e y
        rotation = Quaternion.Euler(y, x, 0f);
        //Se multiplca la rotación por el offset a ponerse para tener la cámara puesta ligeramente a la derecha
        offset = rotation * new Vector3(0f, 0f, -distance);
        //Se pone la cámara en la posición del player y con el offset concreto
        cameraPlace = player.position + offset;

        //Se prepara un raycast entre el jugador y la cámara, si el raycast toca algo entre medio
        //La cámara se pondrá en ese punto y se guardará ese punto
        //Si deja de chocar con algo del layermask entonces se hace un lerp entre la posición que está y el punto
        //donde chocó la última vez y se mueve entre esos 2 puntos a cierta velocidad y con el deltaTime
        //guardada la posición se cambia su transform.position y se pone que la camara mire siempre en
        //cameraOffset (ese punto)
        RaycastHit hit;
        dir = (cameraPlace - player.position).normalized;
        if (Physics.SphereCast(player.position, 0.2f, dir, out hit, distance, layerMask)){
            cameraPlace = hit.point;
        }
        else{
            cameraPlace = Vector3.Lerp(transform.position, cameraPlace, Time.deltaTime * speed);
        }
        cameraOffset = new Vector3(player.position.x + 0.5f, player.position.y, player.position.z);
        transform.position = cameraPlace;
        transform.LookAt(cameraOffset);
    }
}
