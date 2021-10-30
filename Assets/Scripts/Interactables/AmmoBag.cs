using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBag : Interactable
{
    public int uses;

    int currentUses;
    GameManager manager;

    public override void Start()
    {
        base.Start();

        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        OnInteractionActivate.AddListener(manager.AddAmmo);
    }

    public override void Update()
    {
        base.Update();

        if ((Input.GetKeyDown(KeyCode.F) && onRange) && currentUses < uses)
        {
            ActivateInteractable();
            currentUses++;
        }
    }
}
