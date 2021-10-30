using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultButton : Interactable
{
    MusicSystem music;

    public override void Start()
    {
        base.Start();

        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicSystem>();
        OnInteractionActivate.AddListener(music.StartAssault);
    }
}
