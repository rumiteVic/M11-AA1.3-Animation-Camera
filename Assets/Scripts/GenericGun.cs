using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericGun : MonoBehaviour
{
    [Header("Weapon")]
    public int clipMax = 30;
    public int clipCurrent = 30;
    public bool automatic = true;
    [Min(1f/60f)]
    public float fireRate = 0.1f;
    public float reloadTime = 0.5f;
    [Header("Firing")]
    public UnityEvent onFire;
    public Transform firePoint;
    public GameObject bullet;
    [Header("Animation")]
    public float positionRecover;
    public float rotationRecover;
    public Vector3 knockbackPosition;
    public Vector3 knockbackRotation;
    Vector3 originalPosition;
    Quaternion originalRotation;

    private InputSystem_Actions input;
    float lastFire = 0f;
    float reloadingTime = 0f;
    bool isReloading = false;
    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update(){
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, positionRecover * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, rotationRecover * Time.deltaTime);
        //Se manda a hacer reload
        Reload();
        //Si esta reloading no se puede disparar y no llega nunca a los otros else if y else
        if (isReloading){
            return;
        }
        else if (automatic){
            //Si esta en automatic se hace que si presiona todo el rato el boton de ataque
            //Ataca indefinidamente (hasta quedarse sin balas)
            if (input.Player.Attack.IsPressed()){
                CanFire();
            }
        }
        else if (!automatic){
            //Si no es automatico entonces cada vez que se da al botón
            if (input.Player.Attack.WasPressedThisFrame()){
                CanFire();
            }
        }  
    }
    //Aqui se mira si puede disparar
    void CanFire(){
        //Aqui se mira si ha pasado el suficiente tiempo entre disparos, si la ultima
        //vez que se disparo fue hace 3 segundos entonces puede disparar y sino, se vuelve
        if(Time.time - lastFire < fireRate){
            return;
        }
        //Sino hay balas suficientes se manda a canReload
        if(clipCurrent <= 0){
            CanReload();
            return;
        }
        //Sino dispara y guardamos el momento del ultimo disparo
        Fire();
        lastFire = Time.time;
    }
    //Esto es para ver si se puede reloadear
    void CanReload(){
        //Si ya lo esta haciendo o el cargador esta al maximo no se puede relodear
        if (isReloading || clipCurrent == clipMax){
            return;
        }
        //Sino se puede y empieza el reload
        isReloading = true;
        reloadingTime = 0f;
    }
    //Si se puede recargar pues esta es la función
    void Reload(){
        //Si ya se esta reloading entonces se hace return
        if (!isReloading){
            return;
        }
        //Sino se pone el contador con el deltaTime
        reloadingTime += Time.deltaTime;
        //Si se llega al tiempo que requiere se recarga el cargador al máximo y deja de recargarse
        if (reloadingTime>= reloadTime){
            clipCurrent = clipMax;
            isReloading = false;
        }
    }

    public void Fire()
    {
        //Cada vez que dispara se quita una bala y se usa el metodo del pool object para activarlo
        clipCurrent--;
        GameObject bullet = ObjectPool.SharedInstance.GetPooledObject(); 
        if (bullet != null){
            bullet.SetActive(true);
            bullet.transform.position = firePoint.transform.position;
            bullet.transform.rotation = firePoint.transform.rotation;
        }
        onFire.Invoke();
        StartCoroutine(Knockback_Corutine());
    }

    IEnumerator Knockback_Corutine()
    {
        yield return null;
        transform.localPosition -= new Vector3(Random.Range(-knockbackPosition.x, knockbackPosition.x), Random.Range(0, knockbackPosition.y), Random.Range(-knockbackPosition.z, -knockbackPosition.z * .5f));
        transform.localEulerAngles -= new Vector3(Random.Range(knockbackRotation.x * 0.5f, knockbackRotation.x), Random.Range(-knockbackRotation.y, knockbackRotation.y), Random.Range(-knockbackRotation.z, knockbackRotation.z));
    }
}
