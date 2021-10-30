using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool onRange;
    public GameObject interactionButton;
    public UnityEvent OnInteractionActivate;

    MusicSystem music;

    public virtual void Start(){}

    public virtual void Update()
    {
        interactionButton.SetActive(onRange);

        if (Input.GetKeyDown(KeyCode.F) && onRange)
            ActivateInteractable();
    }

    public virtual void ActivateInteractable()
    {
        OnInteractionActivate.Invoke();
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.CompareTag("Player"))
            onRange = true;
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {   
        if (other.CompareTag("Player"))
            onRange = false;
    }
}
