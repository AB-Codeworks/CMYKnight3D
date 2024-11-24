using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float AttackDelay, MoveSpeed;
    public CCFXLib.Sound_Clip Vocalization;

    protected Animator anim;
    protected bool bGoToMelee_c, bReachedMelee_c;
    protected CharacterController controller;
    protected ThePlayer player;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    public virtual void AttackPlayer(ThePlayer p)
    {
        player = p;
        SoundManager.PlaySound_Clip(Vocalization);
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
