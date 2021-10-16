using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool onRange;
    public UnityEvent OnInteractionActivate;

    MusicSystem music;

    public virtual void Start()
    {
        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicSystem>();

        OnInteractionActivate.AddListener(music.StartAssault);
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && onRange)
            ActivateInteractable();
    }

    public virtual void ActivateInteractable()
    {
        OnInteractionActivate.Invoke();
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        onRange = true;
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        onRange = false;
    }
}
