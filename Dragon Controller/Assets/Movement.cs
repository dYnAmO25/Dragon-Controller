using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Ground")]
    [SerializeField] float fSpeed;
    [SerializeField] float fRunModifyer;
    [SerializeField] float fLerpSpeed;
    [SerializeField] float fRotationSpeed;
    [SerializeField] AudioClip[] steps;
    [SerializeField] AudioSource aud;

    [Header("Flying")]
    [SerializeField] float fGroundCheckDistance;
    [SerializeField] float fLerpSpeedFlying;
    [SerializeField] float fFlySpeed;
    [SerializeField] float fSlowDownSpeed;
    [SerializeField] float fSpeedUp;
    [SerializeField] AudioClip[] slaps;
    [SerializeField] AudioSource aud1;

    [Header("Scream")]
    [SerializeField] AudioSource aud2;
    [SerializeField] float maxWaitTime;
    [SerializeField] float minWaitTime;
    
    private Animator ani;
    private Rigidbody rig;

    private bool isFlying, isStarting;

    private float waitTime;
    private float timeWaited;

    void Start()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();

        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {
        ManageFlying();

        if (isStarting)
        {
            MoveUp();
        }

        if (isFlying)
        {
            UpdateRotationFlying();
            Fly();
        }
        else
        {
            MoveForward();
            UpdateAnimator();
            UpdateRotation();
            MoveSidewards();
        }

        DragonScream();
    }

    private void DragonScream()
    {
        timeWaited += Time.deltaTime;

        if (timeWaited >= waitTime)
        {
            timeWaited = 0;
            waitTime = Random.Range(minWaitTime, maxWaitTime);

            if (!aud2.isPlaying)
            {
                aud2.Play();
            }
        }
    }

    private void MoveUp()
    {
        Vector3 move = Vector3.up;

        move = move * fSpeedUp * Time.deltaTime;
        
        rig.AddForce(move, ForceMode.Acceleration);
    }

    private void StartFlying()
    {
        isStarting = false;
        isFlying = true;
    }

    private void ManageFlying()
    {
        if (isFlying == false && isStarting == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isStarting = true;
                Invoke("StartFlying", 1.28f);
                //isFlying = true;
                ani.SetBool("isFlying", true);
                rig.useGravity = false;
            }
        }

        if (isFlying == true)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -transform.up, out hit, fGroundCheckDistance))
            {
                isFlying = false;
                ani.SetBool("isFlying", false);
                rig.useGravity = true;
                ani.SetBool("isFlyingFor", false);
            }
        }
    }

    private void UpdateAnimator()
    {
        
        if (Input.GetAxis("Vertical") > 0)
        {
            ani.SetBool("isForwards", true);
            ani.SetBool("isWalking", true);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                ani.SetBool("isRunning", true);
            }
            else
            {
                ani.SetBool("isRunning", false);
            }
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            ani.SetBool("isWalking", false);
            ani.SetBool("isRunning", false);

        }

        if (Input.GetAxis("Vertical") < 0)
        {
            ani.SetBool("isForwards", false);
            ani.SetBool("isWalking", true);
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            ani.SetBool("isWalking", false);
        }
    }

    private void MoveSidewards()
    {
        if (Input.GetAxis("Vertical") == 0)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                ani.SetBool("movingRight", true);
                ani.SetBool("movingLeft", false);
                transform.Rotate(Vector3.up * fRotationSpeed * Time.deltaTime);
                Move();
            }

            if (Input.GetAxis("Horizontal") < 0)
            {
                ani.SetBool("movingLeft", true);
                ani.SetBool("movingRight", false);
                transform.Rotate(-Vector3.up * fRotationSpeed * Time.deltaTime);
                Move();
            }

            if (Input.GetAxis("Horizontal") == 0)
            {
                ani.SetBool("movingLeft", false);
                ani.SetBool("movingRight", false);
            }
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            ani.SetBool("movingLeft", false);
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            ani.SetBool("movingRight", false);
        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            ani.SetBool("movingLeft", false);
            ani.SetBool("movingRight", false);
        }
    }

    private void Move()
    {
        Vector3 move = transform.forward * Time.deltaTime * fSpeed;

        rig.AddForce(move, ForceMode.Acceleration);
    }

    private void MoveForward()
    {
        float fX = Input.GetAxis("Vertical");
        float fZ = Input.GetAxis("Horizontal");


        //Macht die Horizontale Achse unbenutzt
        fZ = 0;

        Vector3 move = fX * transform.forward + fZ * transform.right;
        move.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            move = move * Time.deltaTime * fSpeed * fRunModifyer;
        }
        else
        {
            move = move * Time.deltaTime * fSpeed;
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            rig.AddForce(move, ForceMode.Acceleration);
        }
    }

    private void UpdateRotation()
    {
        if (rig.velocity.magnitude > 0.5)
        {
            Quaternion qu = Quaternion.LookRotation(Camera.main.transform.forward);
            qu = Quaternion.Euler(0, qu.eulerAngles.y, 0);


            transform.rotation = Quaternion.Lerp(transform.rotation, qu, fLerpSpeed);
        }
    }

    private void UpdateRotationFlying()
    {
        if (rig.velocity.magnitude > 0.5)
        {
            Quaternion qu = Quaternion.LookRotation(Camera.main.transform.forward);

            transform.rotation = Quaternion.Lerp(transform.rotation, qu, fLerpSpeedFlying);
        }
    }

    private void Fly()
    {
        float fX = Input.GetAxis("Vertical");

        Vector3 move = fX * transform.forward;

        move = move * Time.deltaTime * fFlySpeed;

        if (Input.GetAxis("Vertical") > 0)
        {
            rig.AddForce(move, ForceMode.Acceleration);

            ani.SetBool("isFlyingFor", true);
        }
        else
        {
            ani.SetBool("isFlyingFor", false);

            rig.velocity = Vector3.Lerp(rig.velocity, Vector3.zero, fFlySpeed);
        }
    }

    public void FoodStep()
    {
        aud.clip = steps[Random.Range(0, steps.Length)];
        aud.Play();
    }

    public void WingSlap()
    {
        aud1.clip = slaps[Random.Range(0, slaps.Length)];
        aud1.Play();
    }
}