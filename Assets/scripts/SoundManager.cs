using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public UnityEngine.Audio.AudioMixer MainAudioMixer;
    public static CCFXLib SndLib;

    private struct Custom_Loop
    {
        public CCFXLib.Sound_Definition[] Clips;
        public bool RandomizeInterval;
        public CCFXLib.Sound_Clip SoundClip;
        public float Elapsed, Interval, IntervalMax;
    }

    private static float fTimeEl_s = 0.1f;
    private static List<Custom_Loop> lCustom_s = new();
    private static List<CCFXLib.Sound_FX> lRepeat_s = new();
    private static string sFIMG_s = "", sMMG_s = "";

    public static void SetSndLib(GameObject go) 
    { 
        SndLib = go.GetComponent<CCFXLib>(); 
    }

    public static void PlaySound_Clip(CCFXLib.Sound_Clip sc)
    {
        sFIMG_s = "";

        for (int i = 0; i < SndLib.Sound_Library.Length; i++)
        {
            CCFXLib.Sound_FX sfxRecord = SndLib.Sound_Library[i];

            if (sfxRecord.SoundID == sc.SoundID)
            {
                if (sc.SoundOptions.PlayRandomly)
                    sc.SoundOptions.CurClipIdx = (int)Mathf.Floor(Random.value * sfxRecord.Clips.Length);
                else
                {
                    if (sc.SoundOptions.CurClipIdx == sfxRecord.Clips.Length - 1)
                        sc.SoundOptions.CurClipIdx = 0;
                    else
                        sc.SoundOptions.CurClipIdx += 1;
                }

                // if looping andinterval == 0
                bool bRepeat = sc.SoundOptions.Looping && (sc.SoundOptions.Interval == 0);
                sfxRecord.Clips[sc.SoundOptions.CurClipIdx].Source.loop = bRepeat;

                if (bRepeat || !sc.SoundOptions.Looping) // Standard uncontrolled looping sound or one-shot play
                {
                    // Note that an AudioSource that is paused is technically still playing, which is why isPlaying needs to be checked
                    if (sc.SoundOptions.IsResuming && sfxRecord.Clips[sc.SoundOptions.CurClipIdx].Source.isPlaying)
                        sfxRecord.Clips[sc.SoundOptions.CurClipIdx].Source.UnPause();
                    else
                        sfxRecord.Clips[sc.SoundOptions.CurClipIdx].Source.PlayDelayed(sfxRecord.Clips[sc.SoundOptions.CurClipIdx].StartTime);

                    if (bRepeat)
                        lRepeat_s.Add(sfxRecord);

                    if ((sc.SoundOptions.FadeMixerGroup != null) && (sc.SoundOptions.FadeMixerGroup != "")) // Do not assign if not blank, that could interrupt one in progress
                        sFIMG_s = sc.SoundOptions.FadeMixerGroup;
                }
                else // Customized looping sound
                {
                    if (!sc.SoundOptions.SkipInitialPlay)
                        sfxRecord.Clips[0].Source.Play(); // Don't wait an initial interval before playing any sound, unless specified

                    lCustom_s.Add(new() {
                        Clips = sfxRecord.Clips,
                        Interval = sc.SoundOptions.Interval >= 0f ? sc.SoundOptions.Interval : Random.value * Mathf.Abs(sc.SoundOptions.Interval), 
                        IntervalMax = Mathf.Abs(sc.SoundOptions.Interval),
                        RandomizeInterval = sc.SoundOptions.Interval < 0f, 
                        SoundClip = sc});
                }

                break; // Clip was found and played, stop searching
            }

            SndLib.Sound_Library[i] = sfxRecord;
        }
    }

    public static bool StopSound_Clip(CCFXLib.Sound_Clip sc)
    {
        sMMG_s = "";

        foreach (CCFXLib.Sound_FX sfxLoopRec in lRepeat_s)
        {
            if (sc.SoundID == sfxLoopRec.SoundID)
            {
                if ((sc.SoundOptions.MuteMixerGroup != null) && (sc.SoundOptions.MuteMixerGroup != "")) // Do not assign if not blank, that could interrupt one in progress
                    sMMG_s = sc.SoundOptions.MuteMixerGroup;

                if (sc.SoundOptions.IsPausing)
                    sfxLoopRec.Clips[sc.SoundOptions.CurClipIdx].Source.Pause();
                else
                    sfxLoopRec.Clips[sc.SoundOptions.CurClipIdx].Source.Stop();

                lRepeat_s.Remove(sfxLoopRec);
                return true;
            }
        }

        foreach (Custom_Loop cl in lCustom_s)
        {
            if (sc.SoundID == cl.SoundClip.SoundID)
            {
                lCustom_s.Remove(cl);
                return true;
            }
        }

        return false;
    }

    public static void ReplaceSound_Clip(CCFXLib.Sound_Clip sfxOrigRef, CCFXLib.Sound_Clip sfxNewRef)
    {
        if (StopSound_Clip(sfxOrigRef))
            PlaySound_Clip(sfxNewRef);
    }

    private void Update()
    {
        for (int i = 0; i < lCustom_s.Count; i++)
        {
            Custom_Loop cl = lCustom_s[i];
            cl.Elapsed += Time.deltaTime;

            if (cl.Elapsed >= cl.Interval)
            {
                cl.Elapsed = 0;
                cl.Clips[cl.SoundClip.SoundOptions.CurClipIdx].Source.Stop();

                if (cl.SoundClip.SoundOptions.PlayRandomly)
                    cl.SoundClip.SoundOptions.CurClipIdx = (int)Mathf.Floor(Random.value * cl.Clips.Length);
                else
                {
                    if (cl.SoundClip.SoundOptions.CurClipIdx == cl.Clips.Length - 1)
                        cl.SoundClip.SoundOptions.CurClipIdx = 0;
                    else
                        cl.SoundClip.SoundOptions.CurClipIdx += 1;
                }

                cl.Clips[cl.SoundClip.SoundOptions.CurClipIdx].Source.PlayDelayed(cl.Clips[cl.SoundClip.SoundOptions.CurClipIdx].StartTime);

                if (cl.RandomizeInterval)
                    cl.Interval = Random.value * cl.IntervalMax;
            }

            lCustom_s[i] = cl;
        }

        // Note that these Mute and Fade In things do not work on multiple sounds concurrently. Do not use for other than menu music without modifying the code!
        if ((sMMG_s != null) && (sMMG_s != ""))
        {
            MainAudioMixer.SetFloat(sMMG_s, -80f);
            sMMG_s = "";
        }

        if ((sFIMG_s != null) && (sFIMG_s != ""))
        {
            MainAudioMixer.GetFloat(sFIMG_s, out float fMenuMusicVol);

            if (fMenuMusicVol < 0f)
            {
                if (fTimeEl_s >= 0.1f)
                {
                    fTimeEl_s = 0f;
                    MainAudioMixer.SetFloat(sFIMG_s, fMenuMusicVol + 4f); // The menu music volume starts at -80 dB, so an increase of 6 = 7.5% each 0.1 sec, reaching 102.5% volume in 1.5 sec
                }

                fTimeEl_s += Time.unscaledDeltaTime; // Since I am using the fade code for menu music which happens during pause mode (time scale = 0), I have to use unscaled time here
            }
            else
                sFIMG_s = "";
        }
    }
}
