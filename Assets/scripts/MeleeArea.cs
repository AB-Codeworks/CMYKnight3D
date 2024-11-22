using UnityEngine;

public class MeleeArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            other.GetComponent<Enemy>().InitMeleeMode();
    }
}
