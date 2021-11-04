using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicSystem : MonoBehaviour
{
    public enum Context {Normal, Assault}
    public Context currentContext;
    public enum Phase {Control, Anticipation, Assault}
    public Phase currentPhase;
    public enum MusicPhase {Control, Anticipation, Assault}
    public MusicPhase currentMusicPhase;
    public string songIDToLoad;
    public SongData[] soundtrack;
    public float beats;
    public UnityEvent OnControlStart, OnAnticipationStart, OnAssaultStart;

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
    bool errorLoading;
    public bool doneLoadingMusic;
    GameManager manager;
    public AudioSource source;
    SpawnSystem spawn;

    void Start()
    {
        source = GetComponent<AudioSource>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        spawn = GameObject.FindGameObjectWithTag("SpawnSystem").GetComponent<SpawnSystem>();
        /*GetMusic();
        ChangePhase(currentPhase, true);*/
    }

    void Update()
    {   
        if (!errorLoading && doneLoadingMusic)
            CountMusic();
    }

    public void StartAssault()
    {
        currentPhase = Phase.Anticipation;
        StopAllCoroutines();
        ChangePhase(Phase.Anticipation, true);
        spawn.canSpawn = true;
    }

    public void StopAssault()
    {
        manager.playerIsInRoom.OpenDoors();
        currentPhase = Phase.Control;
        ChangePhase(Phase.Control, false);
        spawn.canSpawn = false;
    }

    void SetPhase(Phase _phase)
    {
        currentPhase = _phase;
    }

    public void ChangePhase(Phase _phase, bool playStart)
    {
        switch (_phase)
        {
            case Phase.Control:
                OnControlStart.Invoke();

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
                OnAnticipationStart.Invoke();

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
                OnAssaultStart.Invoke();

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

    public void GetMusic()
    {
        SongData sd = manager.currentDifficulty.difficultySong;

        if (sd != null)
        {
            controlStart = sd.controlStart;
            control = sd.control;
            anticipationStart = sd.anticipationStart;
            anticipation = sd.anticipation;
            assaultStart = sd.assaultStart;
            assault = sd.assault;
            doneLoadingMusic = true;
        }
    }
}
