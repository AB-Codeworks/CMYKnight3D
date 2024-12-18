using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class EnemyGhost : Enemy
{
    public GameObject Projectile;

    private GameObject goProj1_c, goProj2_c;

    public override void AttackPlayer(ThePlayer p)
    {
        base.AttackPlayer(p);
        StartCoroutine(GhostAttack_1());
    }

    private IEnumerator GhostAttack_1()
    {
        // TODO combat tutorial
        yield return new WaitForSeconds(1f);
        goProj1_c = Instantiate(Projectile, transform.position, Quaternion.identity);
        SoundManager.PlaySound_Clip(new CCFXLib.Sound_Clip() { SoundID = CCFXLib.Sound_Identifier.Ghost_Projectile });
        anim.SetTrigger("dmg");
        goProj1_c.transform.DOMove(player.transform.position, 1f).SetEase(Ease.InOutQuad).onComplete = DestroyProjectile_1;
        StartCoroutine(GhostAttack_2());
    }

    private void DestroyProjectile_1()
    {
        Destroy(goProj1_c);
        player.ApplyBlockableDamage(1);
    }

    private void DestroyProjectile_2()
    {
        Destroy(goProj2_c);
        player.ApplyBlockableDamage(1);
    }

    private IEnumerator GhostAttack_2()
    {
        yield return new WaitForSeconds(0.177f);
        goProj2_c = Instantiate(Projectile, transform.position, Quaternion.identity);
        SoundManager.PlaySound_Clip(new CCFXLib.Sound_Clip() { SoundID = CCFXLib.Sound_Identifier.Ghost_Projectile });
        goProj2_c.transform.DOMove(player.transform.position, 1f).SetEase(Ease.InOutQuad).onComplete = DestroyProjectile_2;
        GoToMelee();
    }

    public override void InitMeleeMode()
    {
        base.InitMeleeMode();
        StartCoroutine(MeleeAttack());
    }

    private IEnumerator MeleeAttack()
    {
        anim.SetTrigger("atk");
        yield return new WaitForSeconds(AttackDelay);
        // TODO need check if ghost is alive before doing the below?
        // TODO play attack audio
        // TODO timing of actual hit on player may need to be mid anim, like player attack
        player.ApplyBlockableDamage(1);
        StartCoroutine(MeleeAttack());
    }
}