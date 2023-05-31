using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject goCameraPiviot;
    [SerializeField] float fCameraOffset;
    [SerializeField] float fSense;
    [SerializeField] float fMaxAngle, fMinAngle;
    [SerializeField] float fLerpSpeed;
    [SerializeField] float[] fDistances;

    private float fRotationY, fRotationX;

    private float fDistance;
    private int iCurrentlyDistanceSel = 3;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        CameraPosition();
        ChangeDistance();
    }

    private void ChangeDistance()
    {
        if (Input.mouseScrollDelta.y < 0)
        {
            if (iCurrentlyDistanceSel < fDistances.Length - 1)
            {
                iCurrentlyDistanceSel++;
            }
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            if (iCurrentlyDistanceSel > 0)
            {
                iCurrentlyDistanceSel--;
            }
        }

        fDistance = Mathf.Lerp(fDistance, fDistances[iCurrentlyDistanceSel], fLerpSpeed);
    }

    private void Rotate()
    {
        fRotationY += (Input.GetAxis("Mouse X") * fSense * Time.deltaTime);

        fRotationX -= (Input.GetAxis("Mouse Y") * fSense * Time.deltaTime);
        fRotationX = Mathf.Clamp(fRotationX, fMinAngle, fMaxAngle);

        goCameraPiviot.transform.rotation = Quaternion.Euler(new Vector3(fRotationX, fRotationY, 0));
        transform.rotation = Quaternion.Euler(new Vector3(fRotationX, fRotationY, 0));
    }

    private void CameraPosition()
    {
        RaycastHit hit;

        if (Physics.Raycast(goCameraPiviot.transform.position, -goCameraPiviot.transform.forward, out hit, -fDistance + 1))
        {
            transform.position = hit.point + (transform.forward * fCameraOffset);
        }
        else
        {
            transform.position = (goCameraPiviot.transform.forward * fDistance) + goCameraPiviot.transform.position;
        }
    }
}