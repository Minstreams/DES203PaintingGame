using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public Vector2 sensitivity;
    public float speed;
    Transform camPoint;
    // Start is called before the first frame update
    void Start()
    {
        camPoint = transform.GetChild(0);
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        var mx = Input.GetAxis("Mouse X");
        var my = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up, mx * sensitivity.x);
        camPoint.Rotate(Vector3.left, my * sensitivity.y);

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(x, 0, y) * speed * Time.deltaTime);
    }
}
