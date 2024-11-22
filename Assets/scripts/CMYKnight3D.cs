using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CMYKnight3D : MonoBehaviour
{
    public Animator CharacterAnimator;
    public CharacterController Character;
    public ThePlayer Player;
    public float MoveSpeed = 5f;

    private bool bAttacking_c, bEnemies_c, bIdle_c;
    private Vector2 v2Movement_c;
    private Vector3 v3Movement_c;

    private static readonly string Attack02 = "Attack02_SwordAndShiled";
    private static readonly string Defend = "Defend_SwordAndShield";
    private static readonly string MoveF_Nrm = "MoveFWD_Normal_InPlace_SwordAndShield";
    //private static readonly string Idle_Cmb = "Idle_Battle_SwordAndShield";
    private static readonly string Idle_Nrm = "Idle_Normal_SwordAndShield";

    private void Start()
    {
        Player.OnEnemyInRange += OnEnemyInRange;
    }

    private void OnMove(InputValue iv)
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
            return;
        }

        v2Movement_c = iv.Get<Vector2>();

        if (v2Movement_c == Vector2.zero)
            return;

        bIdle_c = false;
        CharacterAnimator.Play(MoveF_Nrm);
        Quaternion targetRotation = Quaternion.identity;

        if (v2Movement_c.y > 0) // Forward
        {
            if (v2Movement_c.x == 0)
                targetRotation = Quaternion.Euler(0f, 0f, 0f); // Only
            else if (v2Movement_c.x > 0)
                targetRotation = Quaternion.Euler(0f, 45f, 0f); // Plus Right
            else
                targetRotation = Quaternion.Euler(0f, 315f, 0f); // Plus Left
        }
        else if (v2Movement_c.y < 0)
        {
            if (v2Movement_c.x == 0)
                targetRotation = Quaternion.Euler(0f, 180f, 0f); // Only
            else if (v2Movement_c.x > 0)
                targetRotation = Quaternion.Euler(0f, 135f, 0f); // Plus Right
            else
                targetRotation = Quaternion.Euler(0f, 225f, 0f); // Plus Left
        }
        else // Sideways
        {
            if (v2Movement_c.x > 0)
                targetRotation = Quaternion.Euler(0f, 90f, 0f); // Only
            else
                targetRotation = Quaternion.Euler(0f, -90f, 0f); // Only
        }

        CharacterAnimator.transform.rotation = targetRotation;
    }

    private void Update()
    {
        if (!bIdle_c && !Keyboard.current.wKey.isPressed && !Keyboard.current.aKey.isPressed && !Keyboard.current.sKey.isPressed && !Keyboard.current.dKey.isPressed
            && !Keyboard.current.upArrowKey.isPressed && !Keyboard.current.leftArrowKey.isPressed && !Keyboard.current.downArrowKey.isPressed && !Keyboard.current.rightArrowKey.isPressed)
        {
            bIdle_c = true;
            v2Movement_c = Vector2.zero;

            if (bEnemies_c)
                CharacterAnimator.Play(Defend);
            else
                CharacterAnimator.Play(Idle_Nrm);
        }
        else
            v3Movement_c = new(v2Movement_c.x, 0f, v2Movement_c.y);
    }

    private void FixedUpdate()
    {
        if (v3Movement_c != Vector3.zero)
            Character.SimpleMove(v3Movement_c * MoveSpeed);
    }

    private void OnEnemyInRange(GameObject goEnemy)
    {
        goEnemy.GetComponent<Enemy>().AttackPlayer(Player);
        bEnemies_c = true;
        v2Movement_c = Vector2.zero;
        v3Movement_c = Vector3.zero;
        CharacterAnimator.Play(Attack02);
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
}
