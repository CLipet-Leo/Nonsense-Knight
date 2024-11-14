using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class lightTimer : MonoBehaviour
{
    bool isMorning;
    private int maxRotation = 45;
    private int rotation;
    private Light obLight;
    // Update is called once per frame
    private void Awake()
    {
        obLight = GetComponent<Light>();
        isMorning = false;
        timer();
        rotation = maxRotation;
    }


    private void timer()
    {
        if(rotation>= maxRotation)
            isMorning = false;
        if (rotation <= -maxRotation)
            isMorning = true;

        if (isMorning)
        {
            rotation += 1;
            obLight.intensity += 0.01f;
        }
        else
        {
            rotation -= 1;
            obLight.intensity -= 0.01f;
        }
        transform.rotation = Quaternion.Euler(rotation, 0, 0);

        Invoke(nameof(timer), 1);
    }
}
