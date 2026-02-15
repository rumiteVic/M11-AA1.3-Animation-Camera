using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float disappearTime = 5f;
    public Vector3 forceMin = new Vector3(-1, -1, 50);
    public Vector3 forceMax = new Vector3(1, 1, 100);
    public LayerMask layers;
    public float collisionForceMultiplier = 2f;
    public float radius = .2f;
    [HideInInspector]
    public Rigidbody rb;

    public GameObject bulletHole;
    Vector3 lastPos;
    float timeAlive = 0f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    //Cuando vuelve a estar activado de nuevo el objeto en escena
    //se resetea su velocidad tanto lineal como angular
    //Se le añade una nueva velocidad (una fuerza) y se pone que lleva vivo el objeto 0 seg
    void OnEnable(){
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddRelativeForce(new Vector3(Random.Range(forceMin.x, forceMax.x), Random.Range(forceMin.y, forceMax.y), Random.Range(forceMin.z, forceMax.z)), ForceMode.Impulse);
        lastPos = transform.position;
        timeAlive = 0f;
    }

    void FixedUpdate(){
        Vector3 dir = transform.position - lastPos;
        Debug.DrawRay(lastPos, dir, Color.blue, disappearTime);
        RaycastHit hit;
            
        if (Physics.SphereCast(lastPos, radius, dir, out hit, dir.magnitude, layers)){
            Hitted(hit);
        }

        lastPos = transform.position;
    }

    void Update(){
        //Esto es para hacer que se desactive en cuanto el timer llegue al tiempo de disappearTime
        //Se desactive el padre que es que está dentro del pool objects
        timeAlive += Time.deltaTime;
        if(timeAlive >= disappearTime){
            transform.parent.gameObject.SetActive(false);
        }
    }

    void Hitted(RaycastHit hit)
    {
        //Si se choca con algo se crea el bulletHollee en la posición que choque y mirando
        //de frente y si tiene rigidbody se vuelve su hijo
        //Y se desactiva
        GameObject bulletHollee = Instantiate(bulletHole, hit.point, Quaternion.identity);            
        Quaternion targetRotation = Quaternion.LookRotation(-hit.normal);
        bulletHollee.transform.rotation = targetRotation;
        if (hit.rigidbody){
            hit.rigidbody.AddForceAtPosition(rb.linearVelocity * rb.mass * collisionForceMultiplier, this.transform.position);
            bulletHollee.transform.SetParent(hit.transform);
        }
        transform.parent.gameObject.SetActive(false);
    }
}
