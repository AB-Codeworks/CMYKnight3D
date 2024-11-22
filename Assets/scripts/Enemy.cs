using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float AttackDelay, MoveSpeed;

    protected Animator anim;
    protected bool bGoToMelee_c, bReachedMelee_c;
    protected CharacterController controller;
    protected Transform tPlayer_c;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    public virtual void AttackPlayer(ThePlayer p)
    {
        tPlayer_c = p.transform;
        // child classes take over
    }

    public virtual void InitMeleeMode()
    {
        bReachedMelee_c = true;
        anim.SetBool("move", false);
        // child classes take over
    }

    protected void GoToMelee()
    {
        bGoToMelee_c = true;
        anim.SetBool("move", true);
    }

    private void FixedUpdate()
    {
        if (bGoToMelee_c && !bReachedMelee_c)
            controller.SimpleMove(Vector3.back * MoveSpeed);
    }
}
