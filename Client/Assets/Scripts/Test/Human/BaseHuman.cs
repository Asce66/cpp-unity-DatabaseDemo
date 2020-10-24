using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    [SerializeField]
    protected float speed = 1.2f;
    protected bool isRun = false;
    protected bool isAttack;
    protected Vector3 target;
    protected Animator animator;
    protected int runAniHashCode;
    protected int attackAniHashCode;
    protected float attackTime = float.MinValue;
    public string desc = "";//描述

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        runAniHashCode = Animator.StringToHash("IsRun");
        attackAniHashCode = Animator.StringToHash("IsAttack");
    }

    public void Attack()
    {
        isAttack = true;
        animator.SetBool(attackAniHashCode, isAttack);
        attackTime = Time.time;
    }

    protected void AttackUpdate()
    {
        if (isAttack == false) return;
        if (Time.time - attackTime < 1.2f) return;
        isAttack = false;
        animator.SetBool(attackAniHashCode, isAttack);
    }

    protected virtual void Update()
    {
        MoveUpdate();
        AttackUpdate();

    }

    public void MoveTo(Vector3 position)
    {
        target = position;
        isRun = true;
        animator.SetBool(runAniHashCode, isRun);
    }

    protected void MoveUpdate()
    {
        if (isRun == false)
            return;

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);
        transform.LookAt(target);
        if(Vector3.Distance(transform.position,target)<0.05f)
        {
            isRun = false;
            animator.SetBool(runAniHashCode, isRun);
        }
    }
}
