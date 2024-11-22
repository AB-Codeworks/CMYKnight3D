using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThePlayer : MonoBehaviour
{
    private delegate void EnemyInRangeEvt(GameObject goEnemy);
    private EnemyInRangeEvt OnEnemyInRange;

    public Animator CharacterAnimator;
    public CharacterController Character;
    public float MoveSpeed = 5f;

    private bool bAttacking_c, bEnemies_c, bIdle_c;
    private Vector3 v3Movement_c;

    private static readonly string Attack02 = "Attack02_SwordAndShiled";
    private static readonly string Defend = "Defend_SwordAndShield";
    private static readonly string MoveF_Nrm = "MoveFWD_Normal_InPlace_SwordAndShield";
    private static readonly string Idle_Nrm = "Idle_Normal_SwordAndShield";

    private void Start()
    {
        OnEnemyInRange += EnemyInRange;
    }

    private void Update()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            if (bEnemies_c)
            {
                if (!bAttacking_c)
                {
                    bIdle_c = false;
                    bAttacking_c = true;
                    CharacterAnimator.Play(Attack02);
                    StartCoroutine(DoAttack(0.23f, 0.303f));
                }
            }
            else
            {
                if (bIdle_c)
                {
                    bIdle_c = false;
                    v3Movement_c = Vector3.forward;
                    CharacterAnimator.Play(MoveF_Nrm);
                }
            }
        }
        else if (!bIdle_c && !Keyboard.current.wKey.isPressed)
        {
            bIdle_c = true;
            v3Movement_c = Vector3.zero;

            if (bEnemies_c)
                CharacterAnimator.Play(Defend);
            else
                CharacterAnimator.Play(Idle_Nrm);
        }
    }

    private void FixedUpdate()
    {
        if (v3Movement_c != Vector3.zero)
            Character.SimpleMove(v3Movement_c * MoveSpeed);
    }

    private void EnemyInRange(GameObject goEnemy)
    {
        goEnemy.GetComponent<Enemy>().AttackPlayer(this);
        bEnemies_c = true;
        v3Movement_c = Vector3.zero;
    }

    private IEnumerator DoAttack(float fAttackDuration, float fRecoverDuration)
    {
        SoundManager.PlaySound_Clip(new CCFXLib.Sound_Clip() { SoundID = CCFXLib.Sound_Identifier.Player_Attack_A });
        yield return new WaitForSeconds(fAttackDuration);
        // TODO hit enemy if within range
        yield return new WaitForSeconds(fRecoverDuration);

        if (Keyboard.current.wKey.isPressed)
            StartCoroutine(DoAttack(0.23f, 0.303f));
        else
            bAttacking_c = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            OnEnemyInRange?.Invoke(other.gameObject);
    }

    public void ApplyBlockableDamage(int iDmg)
    {
        // TODO if not attacking (blocking), don't take damage
        // otherwise flash player and take dmg
        // play unique audio for either case
        Debug.Log("ow");
    }
}
