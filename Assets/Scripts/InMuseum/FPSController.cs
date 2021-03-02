using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class FPSController : MonoBehaviour
{
    [Label] public Vector2 sensitivity;
    [Label] public float speed;
    [Label] Transform camPoint;
    [Label] public float interactionDistance;
    [Label] public TextMesh tipText;
    [Label] public Animator anim;
    [Label] public float turnFactor;
    [Label] public float turnRate;

    public Transform paintingParticle;
    public Material partMat;
    public Vector2 distanceRange;
    public Vector2 distortionRange;

    void Start()
    {
        camPoint = transform.GetChild(0);
        Cursor.visible = false;
        anim.SetBool("OnGround", true);
    }
    float turn = 0;

    void Update()
    {
        var mx = Input.GetAxis("Mouse X");
        var my = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up, mx * sensitivity.x);
        camPoint.Rotate(Vector3.left, my * sensitivity.y);

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(x, 0, y) * speed * Time.deltaTime);
        anim.SetFloat("SpeedX", y * speed);
        turn += (mx * turnFactor - turn) * turnRate;
        anim.SetFloat("Turn", turn);

        RaycastHit info;
        if (Physics.Raycast(camPoint.position, camPoint.forward, out info, interactionDistance, 1 << LayerMask.NameToLayer("Interactable")))
        {
            tipText.text = $"[{InputSystem.Setting.MainKeys[InputKey.Action].ToString()}] " + info.collider.GetComponent<MInteractable>().GetTipText();
            if (InputSystem.GetKeyDown(InputKey.Action))
            {
                info.collider.GetComponent<MInteractable>().OnInteracted();
            }
        }
        else
        {
            tipText.text = "";
        }
        float dis = Vector3.Distance(transform.position, paintingParticle.position);
        float res = Mathf.Lerp(distortionRange.x, distortionRange.y, (dis - distanceRange.x) / (distanceRange.y - distanceRange.x));
        partMat.SetFloat("_DistortionStrength", res);
    }



}
