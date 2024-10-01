using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 3f;
    float inputV;
    float inputH;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inputV != Input.GetAxisRaw("Vertical"))
        {
            inputV = Input.GetAxisRaw("Vertical");
        }

        if(inputH != Input.GetAxisRaw("Horizontal"))
        {
            inputH = Input.GetAxisRaw("Horizontal");
        }
    }

    void FixedUpdate()
    {
        transform.position += Vector3.forward * inputV * speed * Time.deltaTime;
        transform.position += Vector3.right * inputH * speed * Time.deltaTime;
    }
}