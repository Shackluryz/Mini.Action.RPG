 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;

    [Header("Config Player")]
    public float movementSpeed = 3f;
    private Vector3 direction;
    private bool isWalk;

    //Inputs
    private float horizontal;
    private float vertical;

    [Header("Attack Config")]
    public ParticleSystem fxAttack;
    [SerializeField]
    private bool isAttack;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        MoveCharacter();
        UpdateAnimator();        
    }

    #region Meus Métodos
    void Inputs() {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && isAttack == false) {
            Attack();
            
        }

        direction = new Vector3(horizontal, 0f, vertical).normalized;
    }

    void Attack() {
        isAttack = true;
        anim.SetTrigger("Attack");
        fxAttack.Emit(1);
    }

    void MoveCharacter() {
        if (direction.magnitude > 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            isWalk = true;
        } else {
            isWalk = false;
        }

        controller.Move(direction * movementSpeed * Time.deltaTime);
    }

    void UpdateAnimator() {
        anim.SetBool("isWalk", isWalk);
    }

    void AttackIsDone() {
        isAttack = false;
    }
    #endregion
}
