using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeIA : MonoBehaviour {

    private Animator anim;
    public int HP;
    private bool isDie;

    public enemyState state;

    public const float idleWaitTime = 3f;
    public const float patrolWaitTime = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        ChangeState(state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Died() {

        isDie = true;
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
    
    }

    #region Meus Métodos
    void GetHit(int amount) {

        if (isDie == true) return;

        HP -= amount;

        if (HP > 0) {

            anim.SetTrigger("GetHit");

        } else {

            anim.SetTrigger("Die");
            StartCoroutine("Died");
            
        }
        
    }

    void StateManager()
    {
        switch (state)
        {
            case enemyState.IDLE:
                break;
            case enemyState.ALERT:
                break;
            case enemyState.EXPLORE:
                break;
            case enemyState.FOLLOW:
                break;
            case enemyState.FURY:
                break;
            case enemyState.PATROL:
                break;
        }
    }

    void ChangeState(enemyState newState)
    {
        StopAllCoroutines();
        state = newState;
        print(newState);
        switch (state)
        {
            case enemyState.IDLE:
                StartCoroutine("IDLE");
                break;
            case enemyState.ALERT:
                break;
            case enemyState.PATROL:
                StartCoroutine("PATROL");
                break;
        }
    }

    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(idleWaitTime);
        if(Rand() < 50)
        {
            ChangeState(enemyState.IDLE);
        }
        else
        {
            ChangeState(enemyState.PATROL);
        }
    }

    IEnumerator PATROL()
    {
        yield return new WaitForSeconds(patrolWaitTime);
        ChangeState(enemyState.IDLE);
    }

    int Rand()
    {
        int rand = Random.Range(0, 100);
        return rand;
    }
    #endregion
}
