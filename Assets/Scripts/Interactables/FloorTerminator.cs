using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTerminator : Interactable
{
    public GameObject requireMore;

    GameManager manager;

    public override void Start()
    {
        base.Start();

        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        OnInteractionActivate.AddListener(manager.EndFloor);
    }

    public override void Update()
    {
        base.Update();

        if (onRange)
        {
            if (manager.currentCompletedAssaults < manager.requiredAssaultsToEndFloor)
                requireMore.SetActive(true);
            
            else if (manager.currentCompletedAssaults >= manager.requiredAssaultsToEndFloor)
                if (Input.GetKeyDown(KeyCode.F))
                    ActivateInteractable();
        }

        else
        {
            requireMore.SetActive(false);
            interactionButton.SetActive(false);
        }
    }
}
