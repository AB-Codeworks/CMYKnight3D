using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThePlayer : MonoBehaviour
{
    // Save progress idea
    //Hash128 h = new();
    //h.Append("Gamsie the Miffed,12,244,A23,G44,TFFTTTFTTFFTTFT,TFTFFTFTT,12,27,5,14,..."); -> pro points for storing an int value of the binary representation of those TFs
    //Debug.Log("Save hash = " + h.ToString());


    private delegate void EnemyInRangeEvt(GameObject goEnemy);
    private EnemyInRangeEvt OnEnemyInRange;

    public Animator CharacterAnimator;
    public CharacterController Character;
    [ColorUsage(true, true)] public Color[] CharPalette;
    public float MoveSpeed = 5f;
    public Material CharacterMaterial, UIBorderMaterial;

    private bool bAttacking_c, bEnemies_c, bIdle_c;
    private Vector3 v3Movement_c;

    private static readonly string Attack02 = "Attack02_SwordAndShiled"; // not my typo
    private static readonly string Defend = "Defend_SwordAndShield";
    private static readonly string DefendHit = "DefendHit_SwordAndShield";
    private static readonly string Hit = "GetHit01_SwordAndShield";
    private static readonly string Idle_Nrm = "Idle_Normal_SwordAndShield";
    private static readonly string MoveF_Nrm = "MoveFWD_Normal_InPlace_SwordAndShield";

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
                    // TODO walk sound
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
        if (bAttacking_c)
        {
            StartCoroutine(FlashPlayer());
            CharacterAnimator.Play(Hit);
            StartCoroutine(ReturnFromAnim(0.467f));
            SoundManager.PlaySound_Clip(new CCFXLib.Sound_Clip() { SoundID = CCFXLib.Sound_Identifier.Player_Hit_A });
            // TODO subtract hp
        }
        else
        {
            CharacterAnimator.Play(DefendHit);
            SoundManager.PlaySound_Clip(new CCFXLib.Sound_Clip() { SoundID = CCFXLib.Sound_Identifier.Player_Block_A });
            StartCoroutine(ReturnFromAnim(0.333f));
        }
    }

    private IEnumerator ReturnFromAnim(float fSeconds)
    {
        yield return new WaitForSeconds(fSeconds);

        if (bAttacking_c)
            CharacterAnimator.Play(Attack02);
        else if (bEnemies_c)
            CharacterAnimator.Play(Defend);
        else if (bIdle_c)
            CharacterAnimator.Play(Idle_Nrm);
        else
            CharacterAnimator.Play(MoveF_Nrm);
    }

    private IEnumerator FlashPlayer()
    {
        CharacterMaterial.SetColor("CutoffColour", CharPalette[1]);
        CharacterMaterial.SetColor("OutColour2", CharPalette[1]);
        CharacterMaterial.SetColor("OutColour3", CharPalette[1]);
        UIBorderMaterial.SetColor("OutColour2", CharPalette[1]);
        UIBorderMaterial.SetColor("OutColour3", CharPalette[1]);
        yield return new WaitForSeconds(0.1f);
        CharacterMaterial.SetColor("CutoffColour", CharPalette[0]);
        CharacterMaterial.SetColor("OutColour2", CharPalette[2]);
        CharacterMaterial.SetColor("OutColour3", CharPalette[3]);
        UIBorderMaterial.SetColor("OutColour2", CharPalette[2]);
        UIBorderMaterial.SetColor("OutColour3", CharPalette[0]);
    }
}
