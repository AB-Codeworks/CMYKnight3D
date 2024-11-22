using UnityEngine;

public class ThePlayer : MonoBehaviour
{
    public delegate void EnemyInRangeEvt(GameObject goEnemy);
    public EnemyInRangeEvt OnEnemyInRange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            OnEnemyInRange?.Invoke(other.gameObject);
    }
}
