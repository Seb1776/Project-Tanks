using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultButton : Interactable
{
    public override void Start()
    {
        base.Start();

        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicSystem>();
        OnInteractionActivate.AddListener(music.StartAssault);
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.F) && onRange)
            ActivateInteractable();
    }
}
