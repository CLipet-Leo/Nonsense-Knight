using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muzzleFlashRender : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform posOfMuzzle;

    // Update is called once per frame
    void Update()
    {
        if (gameObject != null && gameObject.transform != null && posOfMuzzle != null)
            gameObject.transform.position = posOfMuzzle.position;
    }
}
