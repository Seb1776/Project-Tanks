using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    public enum Context {Normal, Assault}
    public Context currentContext;
    public enum Phase {Control, Anticipation, Assault}
    public Phase currentPhase;
    public enum MusicPhase {Control, Anticipation, Assault}
    public MusicPhase currentMusicPhase;
    public string songName;
    public float beats;

    float currentBeats;
    AudioClip controlStart;
    AudioClip control;
    AudioClip anticipationStart;
    AudioClip anticipation;
    AudioClip assaultStart;
    AudioClip assault;
    bool beat;
    bool playingControl;
    bool playingAnticipation;
    bool playingAssault;
    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        GetMusic();
        ChangePhase(currentPhase, true);
    }

    void Update()
    {
        CountMusic();
    }

    public void StartAssault()
    {
        currentPhase = Phase.Anticipation;
    }

    void SetPhase(Phase _phase)
    {
        currentPhase = _phase;
    }

    void ChangePhase(Phase _phase, bool playStart)
    {
        switch (_phase)
        {
            case Phase.Control:
                if (controlStart != null && playStart)
                {
                    source.clip = controlStart;
                    source.loop = false;
                    source.Play();
                    currentPhase = _phase;
                    currentMusicPhase = MusicPhase.Control;
                    StartCoroutine(PlayAfterStart(control, controlStart.length, currentPhase, true));
                }

                else
                {
                    source.clip = control;
                    source.loop = true;
                    source.Play();
                    currentPhase = _phase;
                    currentMusicPhase = MusicPhase.Control;
                    playingControl = true;
                    playingAnticipation = false;
                    playingAssault = false;
                }
            break;

            case Phase.Anticipation:
                if (anticipationStart != null)
                {
                    source.clip = anticipationStart;
                    source.loop = false;
                    source.Play();
                    currentPhase = _phase;
                    currentMusicPhase = MusicPhase.Anticipation;
                    StartCoroutine(PlayAfterStart(anticipation, anticipationStart.length, currentPhase, false));
                }

                else
                {
                    source.clip = anticipation;
                    source.loop = true;
                    source.Play();
                    currentPhase = _phase;
                    currentMusicPhase = MusicPhase.Anticipation;
                    playingControl = false;
                    playingAnticipation = true;
                    playingAssault = false;
                }
            break;

            case Phase.Assault:
                if (assaultStart != null)
                {
                    source.clip = assaultStart;
                    source.loop = false;
                    source.Play();
                    currentPhase = _phase;
                    currentMusicPhase = MusicPhase.Assault;
                    StartCoroutine(PlayAfterStart(assault, assaultStart.length, currentPhase, true));
                }

                else
                {
                    source.clip = assault;
                    source.loop = true;
                    source.Play();
                    currentPhase = _phase;
                    currentMusicPhase = MusicPhase.Assault;
                    playingControl = false;
                    playingAnticipation = false;
                    playingAssault = true;
                }
            break;
        }
    }

    void CountMusic()
    {
        if (playingControl)
        {
            if (currentPhase != Phase.Control && currentPhase == Phase.Anticipation)
            {
                if (!source.isPlaying)
                    ChangePhase(Phase.Anticipation, true);

                source.loop = false;
            }

            else if (currentPhase == Phase.Control)
                source.loop = true;
        }

        if (playingAnticipation)
        {
            if (currentPhase == Phase.Anticipation)
            {
                if (!source.isPlaying)
                    ChangePhase(Phase.Assault, true);
                
                source.loop = false;
            }
        }

        if (playingAssault)
        {
            if (currentPhase != Phase.Assault && currentPhase == Phase.Control)
            {
                if (!source.isPlaying)
                    ChangePhase(Phase.Control, false);
                
                source.loop = false;
            }

            else if (currentPhase == Phase.Assault)
                source.loop = true;
            
            if (currentBeats >= beats)
            {
                beat = !beat;
                currentBeats = 0f;
            }

            else
                currentBeats += Time.deltaTime;
        }
    }

    IEnumerator PlayAfterStart(AudioClip _clip, float delay, Phase _phase, bool setLoop)
    {
        yield return new WaitForSeconds(delay);
        source.clip = _clip;
        source.loop = setLoop;
        source.Play();

        switch (_phase)
        {
            case Phase.Control:
                playingControl = true;
                playingAnticipation = false;
                playingAssault = false;
            break;

            case Phase.Anticipation:
                playingControl = false;
                playingAnticipation = true;
                playingAssault = false;
            break;

            case Phase.Assault:
                playingControl = false;
                playingAnticipation = false;
                playingAssault = true;
            break;
        }
    }

    void GetMusic()
    {
        controlStart = Resources.Load<AudioClip>("Music/" + songName + "_controlstart");
        control = Resources.Load<AudioClip>("Music/" + songName + "_control");
        anticipationStart = Resources.Load<AudioClip>("Music/" + songName + "_anticipationstart");
        anticipation = Resources.Load<AudioClip>("Music/" + songName + "_anticipation");
        assaultStart = Resources.Load<AudioClip>("Music/" + songName + "_assaultstart");
        assault = Resources.Load<AudioClip>("Music/" + songName + "_assault");
    }
}
