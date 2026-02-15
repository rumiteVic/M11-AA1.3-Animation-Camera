
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    public GroundDetector gd;
    public CharacterMover cm;
    public Camera cam;
    Animator anim;
    public float rotationScale;
    public Transform gunPivot;
    public Transform gunRightHand;
    public Transform gunLeftHand;

    public Transform mira;

    float fallSpeed = 0f;

    [Range(0f, 1f)]
    public float lookAtMaxAngle = 0.5f;
    public RaycastLookAt cameraLookAt;
    public float lookAtSpeed = 10;
    Vector3 lookat;
    private void Start()
    {
        anim = GetComponent<Animator>();
        if(cam == null)
        {
            cam = Camera.main;
        }

        lookat = cameraLookAt.lookingAt;
    }
    void Update()
    {
        anim.SetFloat("Sideways", cm.velocity.x);
        anim.SetFloat("Forward", cm.velocity.z);
        anim.SetFloat("Rotation", cm.velocityAngular * rotationScale);
        anim.SetBool("Grounded", gd.grounded);

        FixLookat();
        //Si el personaje está en el suelo (tocandolo) y la velocidad de caída es mayor a 0
        if (!gd.grounded && cm.velocity.y < 0f){
            //Guarda en valor absoluto la velocidad de caída
            //En absoluto porque si está cayendo es negativa
            fallSpeed = Mathf.Abs(cm.velocity.y);
        }
        else{
            //Sino es 0
            fallSpeed = 0f;
        }
        //Esto calcula la posición entre un valor de 0 a 1 dentro de un minimo y un máximo
        //devuelve un porcentaje y no un valor interpolado
        //Luego en la velocidad de caída se pone ese porcentaje para su velocidad de caída
        float animFallSpeed = Mathf.InverseLerp(0f, 10f, fallSpeed);
        anim.SetFloat("Upwards", animFallSpeed);

        gunPivot.LookAt(lookat);
    }

    private void FixLookat()
    {
        lookat = Vector3.Lerp(lookat, cameraLookAt.lookingAt, lookAtSpeed * Time.deltaTime);

        Vector3 forwardLookAt = (lookat - cameraLookAt.transform.position).normalized;
        float dot = Vector3.Dot(forwardLookAt, transform.forward);
        if (dot < lookAtMaxAngle)
        {
            Vector3 axis = Vector3.Cross(forwardLookAt, transform.forward);
            float angle = Vector3.SignedAngle(forwardLookAt, transform.forward, axis);
            float distance = Vector3.Distance(lookat, cameraLookAt.transform.position);
            float maxAngle = Mathf.Acos(lookAtMaxAngle) * Mathf.Rad2Deg;
            forwardLookAt = Quaternion.AngleAxis(angle > 0 ? -maxAngle : maxAngle, axis) * transform.forward;
            lookat = cameraLookAt.transform.position + forwardLookAt * distance;
        }
    }

    //Una función especial dedicada los IKs del animator
    void OnAnimatorIK(){
        //Hacemos que el ik del cuello mire al 100% el punto de mira.position
        anim.SetLookAtWeight(1);
        anim.SetLookAtPosition(mira.position);

        //Con lo siguiente se hace que tanto la posicion como rotacion de las manos esten en el transform de gunRightHand y gunLeftHand
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.RightHand, gunRightHand.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, gunRightHand.rotation);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, gunLeftHand.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, gunLeftHand.rotation);
    }
}
