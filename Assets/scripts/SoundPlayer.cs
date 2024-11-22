using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    public CCFXLib.Sound_Clip[] PlaySounds;

    private void OnEnable()
    {
        foreach (CCFXLib.Sound_Clip sc in PlaySounds)
            SoundManager.PlaySound_Clip(sc);
    }
}
