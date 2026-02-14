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
        
        Reload();

        if (isReloading){
            return;
        }
        if (automatic){
            if (input.Player.Attack.IsPressed()){
                CanFire();
            }
        }
        else{
            if (input.Player.Attack.WasPressedThisFrame()){
                CanFire();
            }
        }  
    }

    void CanFire(){
        if(Time.time < lastFire + fireRate){
            return;
        }
        if(clipCurrent <= 0){
            CanReload();
            return;
        }
        Fire();
        lastFire = Time.time;
    }

    void CanReload(){
        if (isReloading || clipCurrent == clipMax){
            return;
        }
        isReloading = true;
        reloadingTime = 0f;
    }

    void Reload(){
        if (!isReloading){
            return;
        }
        reloadingTime += Time.deltaTime;

        if (reloadingTime>= reloadTime){
            clipCurrent = clipMax;
            isReloading = false;
        }
    }

    public void Fire()
    {
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
