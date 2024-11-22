using System;
using UnityEngine;
using UnityEngine.Audio;

public class CCFXLib : MonoBehaviour
{
    public enum Sound_Identifier 
    {
        Player_Walk_A, Player_Attack_A, Ghost_Move, Ghost_Projectile, Ghost_Voc
    }

    [Serializable]
    public struct Sound_FX
    {
        public Sound_Identifier SoundID; // Lookup answer
        public Sound_Definition[] Clips;
    }

    [Serializable]
    public struct Sound_Definition
    {
        public AudioSource Source;
        public float StartTime;
    }

    [Serializable]
    public struct Sound_Options
    {
        public bool Looping;
        public bool PlayRandomly;
        [HideInInspector] public bool IsPausing;
        [HideInInspector] public bool IsResuming;
        public bool SkipInitialPlay;
        public float Interval;
        [HideInInspector] public string FadeMixerGroup;
        [HideInInspector] public string MuteMixerGroup;
        [HideInInspector] public int CurClipIdx;
    }

    [Serializable]
    public struct Sound_Clip
    {
        public Sound_Identifier SoundID; // Lookup question
        public Sound_Options SoundOptions;
    }

    public Sound_FX[] Sound_Library;

    private void Awake()
    {
        SoundManager.SetSndLib(gameObject);
    }
}
