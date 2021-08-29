using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeIA : MonoBehaviour {

    private GameManager _GameManager;

    private Animator anim;
    public int HP;
    private bool isDie;

    public enemyState state;

    public const float idleWaitTime = 3f;

    //IA
    private bool isWalk;
    private bool isAlert;
    private bool isAttack;
    private bool isPlayerVisible;
    private NavMeshAgent agent;
    private int idWayPoint;
    private Vector3 destination;

    // Start is called before the first frame update
    void Start() {
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        //ChangeState(state);
    }

    // Update is called once per frame
    void Update() {
        StateManager();
        if (agent.desiredVelocity.magnitude >= 0.1f) {
            isWalk = true;
        } else {
            isWalk = false;
        }

        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isAlert", isAlert);
    }

    IEnumerator Died() {

        isDie = true;
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
    
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {

            isPlayerVisible = true;

            if (state == enemyState.IDLE || state == enemyState.PATROL) {
                ChangeState(enemyState.ALERT);
            } else if (state == enemyState.FOLLOW) {
                StopCoroutine("FOLLOW");
                ChangeState(enemyState.FOLLOW);
            }
                   
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            isPlayerVisible = false;
        }
    
    }

    #region Meus Métodos
    void GetHit(int amount) {

        if (isDie == true) return;

        HP -= amount;

        if (HP > 0) {
            ChangeState(enemyState.FURY);
            anim.SetTrigger("GetHit");
        } else {
            anim.SetTrigger("Die");
            StartCoroutine("Died");  
        }
        
    }

    void StateManager() {
        switch (state) {
            case enemyState.FOLLOW:
                // Comportamento quando estiver seguindo
                destination = _GameManager.player.position;
                agent.destination = destination;

                if (agent.remainingDistance <= agent.stoppingDistance) {
                    Attack();            
                }
                break;
            case enemyState.FURY:
                // Comportamento quando estiver em furia
                destination = _GameManager.player.position;
                agent.destination = destination;

                if (agent.remainingDistance <= agent.stoppingDistance) {
                    Attack();
                }
                break;
            case enemyState.PATROL:
                // Comportamento quando estiver em patrulha
                break;
        }
    }

    void ChangeState(enemyState newState) {
        StopAllCoroutines();
        isAlert = false;

        switch (newState) {
            case enemyState.IDLE:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine("IDLE");
                break;
            case enemyState.ALERT:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine("ALERT");
                break;
            case enemyState.PATROL:
                agent.stoppingDistance = 0;
                idWayPoint = Random.Range(0, _GameManager.slimeWayPoints.Length);
                destination = _GameManager.slimeWayPoints[idWayPoint].position;
                agent.destination = destination;
                StartCoroutine("PATROL");
                break;
            case enemyState.FOLLOW:
                agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
                StartCoroutine("FOLLOW");
                break;
            case enemyState.FURY:
                destination = transform.position;
                agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
                agent.destination = destination;
                break;
        }

        state = newState;
    }

    IEnumerator IDLE() {
        yield return new WaitForSeconds(_GameManager.slimeIdleWaitTime);
        StayStill(50); // 50% de chance de ficar parado ou entrar em patrulha
    }

    IEnumerator PATROL() {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        StayStill(30); //30% de chance de ficar parado e 70% de ficar em patrulha
    }

    IEnumerator ALERT() {
        yield return new WaitForSeconds(_GameManager.slimeAlertTime);

        if (isPlayerVisible == true) {
            ChangeState(enemyState.FOLLOW);
        } else {
            StayStill(10);
        }
    }

    IEnumerator FOLLOW() {
        yield return new WaitUntil(() => !isPlayerVisible);
        print("perdi você");

        yield return new WaitForSeconds(_GameManager.slimeAlertTime);
        StayStill(50);
    }

    IEnumerator ATTACK() {
        yield return new WaitForSeconds(_GameManager.slimeAttackDelay);
        isAttack = false;
    }

    void StayStill(int yes) {
        if (Rand() < yes) {
            ChangeState(enemyState.IDLE);
        } else {
            ChangeState(enemyState.PATROL);
        }
    }

    int Rand() {
        int rand = Random.Range(0, 100);
        return rand;
    }

    void Attack() {
        if (!isAttack) {
            isAttack = true;
            anim.SetTrigger("Attack");
        }
    }

    void AttackIsDone() {
        StartCoroutine("ATTACK");
    }

    #endregion
}
