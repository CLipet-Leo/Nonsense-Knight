using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform playerPos;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = playerPos.localPosition;
        pos.y += 14.8f;
        pos.z += 3f;
        pos.x -= 1;
        transform.localPosition = pos;

    }
}