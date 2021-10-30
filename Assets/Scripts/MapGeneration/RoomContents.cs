using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RoomContents : MonoBehaviour
{
    public Light2D[] lights;
    [Range(0f, 1f)]
    public float chanceOfTwitchingLights;

    void Start()
    {   
        if (lights.Length > 0)
            for (int i = 0; i < lights.Length; i++)
                if (Random.value <= chanceOfTwitchingLights)
                    lights[i].GetComponent<Animator>().SetTrigger("twitch");
    }
}
