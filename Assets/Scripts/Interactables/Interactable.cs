using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool onRange;
    public GameObject interactionButton;
    public UnityEvent OnInteractionActivate;
    public bool destroyOnInteraction;

    bool destroy;

    [HideInInspector]
    public MusicSystem music;

    public virtual void Start(){}

    public virtual void Update()
    {
        interactionButton.SetActive(onRange);
        
        if (destroy)
        {
            Vector2 localTrs = transform.localScale;

            localTrs.x -= Time.deltaTime * 2.5f;
            localTrs.y -= Time.deltaTime * 2.5f;

            if (localTrs.x <= 0f && localTrs.y <= 0f)
                Destroy(this.gameObject);

            transform.localScale = localTrs;
        }
    }

    public virtual void ActivateInteractable()
    {
        OnInteractionActivate.Invoke();

        destroy = destroyOnInteraction;
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
