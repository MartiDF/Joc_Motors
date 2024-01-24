using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mov2D : MonoBehaviour
{

    public float speed = 2;
    private BoxCollider2D _box;

    // Start is called before the first frame update
    void Start()
    {
        _box = GetComponentsInChildren<BoxCollider2D>()[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 pos = transform.position;
        if (!_box.isTrigger)
        {

            pos.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            pos.y += Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.position = pos;
        }



    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        speed = -0.1f;
    }

}
