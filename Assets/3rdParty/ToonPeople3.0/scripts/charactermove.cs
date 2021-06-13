using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charactermove : MonoBehaviour {


    public Transform trans;
    public Rigidbody rigid;
    public float walkspeed;
    public float runspeed;
    public float jumpforce;
    Transform transP;
    Collider coll;
    Animator anim;
    bool active;
    bool grounded;
    bool jumping;
    float stairs;
    float run;
    float walk;
    float turn;
    float strafe;
    float speed;
    Vector3 dir;
    RaycastHit hit0;
    RaycastHit hit1;
    RaycastHit hit2;
    RaycastHit hit3;
    float tilt;
    float Wstrafe;
    Vector3 lastPosition;
    float forwardSpeed;
    bool blocked;
    bool door;
    float grfactor;
    public float tiltangle;

    void Start()
    {
        transP = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        active = true;
        walkspeed *= 0.1f;
        runspeed *= 0.1f;
        jumpforce *= 0.1f;
    }


    void Update()
    {
        setdir();        

        if (active)
        {
            
        groundcheck();

            //MOVEMENT

            if (Input.GetAxis("Vertical") != 0f) walk = Input.GetAxis("Vertical") * 2f;
            else walk = Mathf.Lerp(walk, 0f, Time.deltaTime * 10f);
            if (walk < 0.1f && walk > -0.1f) walk = 0f; 
            
            if (stairs <-47f) walk = 0f;
                turn = Input.GetAxis("Horizontal");
                //RUN               
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    run = Mathf.Lerp(run, 3, Time.deltaTime*2f );
                    anim.SetBool("runing", true);
                }
                else
                {
                    run = Mathf.Lerp(run, 0, Time.deltaTime * 4f);
                    if (run < 0.1f) run = 0f;
                    anim.SetBool("runing", false);
                }

            if (grounded)
            {
                //STRAFE
                strafe = 0f;
                if (Input.GetKey("e")) strafe = walkspeed*2.5f + walkspeed * run ;
                if (Input.GetKey("q")) strafe = -walkspeed*2.5f - walkspeed*run ;
                //APPLY MOVEMENT
                trans.Rotate(new Vector3(0f, turn*2f + turn* (run*0.5f), 0f));
                speed = (walk * walkspeed * ((run *runspeed *  0.5f) + 1)) + (walk *walkspeed * 0.5f * Mathf.Abs(stairs) / -70f) - (walk * Mathf.Abs(strafe) * 0.125f);                
                if (!jumping && !blocked ) rigid.velocity = (dir * speed) + (trans.right*strafe*0.5f);                
                // ANIMATOR
                anim.SetFloat("walk", walk);
                anim.SetFloat("turn", turn);
                anim.SetFloat("run", run*10f);
                anim.SetFloat("strafe", strafe);
                anim.SetFloat("speed", speed);

                //REALSPEED
                forwardSpeed = Mathf.Lerp(forwardSpeed,(trans.position - lastPosition).magnitude / Time.deltaTime,Time.deltaTime*8);
                lastPosition = trans.position;

                //BLOCKED && HITS  
                if (Physics.Raycast(trans.position + new Vector3(0f, 1f, 0f) + trans.forward * speed * 0.1f, 
                                    trans.forward * speed * 0.1f, 
                                    0.1f + (run / 20f)) ||
                    Physics.Raycast(
                                    trans.position + new Vector3(0f, 1.5f, 0f) + trans.forward * speed * 0.1f, 
                                    trans.forward * speed * 0.1f, 
                                    0.1f + (run / 20f)))
                {
                    blocked = true;
                    anim.SetBool("blocked", true);
                    if (forwardSpeed > 4f)
                    {                        
                        StartCoroutine("wait", 0.4f);
                        if (walk > 0f) anim.Play("hitforward");
                        if (walk < 0f) anim.Play("hitbackwards");
                        run = 0f;anim.SetFloat("run", 0f);
                        forwardSpeed = 0f;
                        rigid.AddForce(trans.forward * -8f * Mathf.Sign(walk), ForceMode.VelocityChange);
                    }
                    if (walk == 0)
                    {
                        blocked = false;
                        anim.SetBool("blocked", false);
                    }
                }
                else blocked = false;
                anim.SetBool("blocked", blocked);


                //TILT & WALKSTRAFE       
                if (walk > 0.1f && run > 1f)
                {
                    tilt = Mathf.Lerp(tilt,turn * -12f * tiltangle, Time.deltaTime * 3f);
                }
                else tilt = Mathf.Lerp(tilt, 0, Time.deltaTime * 5f);  
                
                if (walk != 0f && strafe != 0f) Wstrafe = Mathf.Lerp(Wstrafe, Mathf.Sign(walk) *45f *  Mathf.Sign(strafe), Time.deltaTime * 3f);
                else Wstrafe = Mathf.Lerp(Wstrafe, 0, Time.deltaTime * 3f);
                if (Wstrafe < 0.1f && Wstrafe > -0.1f) Wstrafe = 0f;
                transP.localEulerAngles = new Vector3(0f, Wstrafe, tilt);
                                           
                //JUMP  
                if (Input.GetKeyDown("space") && !jumping)
                {                    
                    jumping = true;
                    StartCoroutine("jumpdelayed",0.1f);
                    anim.SetBool("jump", jumping);
                    StartCoroutine("nojump");
                }

                //SITDOWN        
                
                if (Input.GetKeyDown(KeyCode.C) && grounded  && active && run == 0f && !anim.GetBool("sitdown"))
                {
                    if (Physics.Raycast(trans.position + new Vector3(0.0f, 0.3f, 0f) - trans.forward * 0.1f, -trans.forward, 0.17f)
                        && !Physics.Raycast(trans.position + new Vector3(0.0f, 0.42f, 0f) - trans.forward * 0.1f, -trans.forward, 0.4f))
                    {
                        active = false;
                        anim.SetBool("sitdown", true);                        
                        StartCoroutine("sitdelayed", 0.5f);
                        anim.Play("sitdown");
                    }
                    else if (anim.GetFloat("walk") == 0f)
                    {
                        anim.Play("lookback");
                        StartCoroutine("wait", 1.25f);
                    }
                }
            }
            if (!grounded)
            {
                trans.Rotate(0f, Input.GetAxis("Horizontal"), 0f);
                rigid.velocity += trans.forward * Input.GetAxis("Vertical")*0.025f;
                tilt = Mathf.Lerp(tilt, 0, 0.25f);
                transP.localEulerAngles = new Vector3(0f, 0f, tilt);                
            }            
        }                
            if (anim.GetBool("sitdown") && Input.anyKeyDown )
            {
                anim.SetBool("sitdown", false);
                StartCoroutine("wait", 1f);
            }
    }



    void groundcheck()
    {
        
        if (walk == 0f) grfactor = 0.3f;
        else grfactor = 0.35f;
        grounded = false;        
        Physics.Raycast(trans.position + new Vector3(0f, 0.25f, 0f) + trans.forward * 0.18f + trans.right * 0.18f,
            Vector3.down, out hit0 , 0.5f);
        if ((hit0.point - (trans.position + new Vector3(0f, 0.25f, 0f) + trans.forward * 0.18f + trans.right * 0.18f)).magnitude < grfactor) grounded = true ;

        Physics.Raycast(trans.position + new Vector3(0f, 0.25f, 0f) + trans.forward * 0.18f - trans.right * 0.18f,
            Vector3.down, out hit0, 0.5f);
        if ((hit0.point - (trans.position + new Vector3(0f, 0.25f, 0f) + trans.forward * 0.18f - trans.right * 0.18f)).magnitude < grfactor) grounded = true;

        Physics.Raycast(trans.position + new Vector3(0f, 0.25f, 0f) - trans.forward * 0.18f + trans.right * 0.18f,
            Vector3.down, out hit0, 0.5f);
        if ((hit0.point - (trans.position + new Vector3(0f, 0.25f, 0f) - trans.forward * 0.18f + trans.right * 0.18f)).magnitude < grfactor) grounded = true;

        Physics.Raycast(trans.position + new Vector3(0f, 0.25f, 0f) - trans.forward * 0.18f - trans.right * 0.18f,
            Vector3.down, out hit0, 0.5f);
        if ((hit0.point - (trans.position + new Vector3(0f, 0.25f, 0f) - trans.forward * 0.18f - trans.right * 0.18f)).magnitude < grfactor) grounded = true;

        Physics.Raycast(trans.position + new Vector3(0f, 0.25f, 0f), 
            Vector3.down, out hit0, 0.5f);
        if ((hit0.point - (trans.position + new Vector3(0f, 0.25f, 0f))).magnitude < grfactor) grounded = true;
        

        if (grounded)
        { 
            coll.material.dynamicFriction = 1f;
            coll.material.staticFriction = 1f;
            //STAIRS            
            stairs = Vector3.SignedAngle(trans.forward, dir, trans.right);
            anim.SetFloat("stairs", stairs);
        }
        else
        {
            coll.material.dynamicFriction = 0f;
            coll.material.staticFriction = 1f;
        }
        
        anim.SetBool("grounded", grounded);
        
    }
    void setdir()
    {
        if (!grounded) dir = trans.forward;
        else
        {
            Physics.Raycast(trans.position + new Vector3(0f, 0.5f, 0f) + (trans.forward * 0.2f), Vector3.down, out hit1, 1f);
            Physics.Raycast(trans.position + new Vector3(0f, 0.5f, 0f), Vector3.down, out hit2, 1f);
            Physics.Raycast(trans.position + new Vector3(0f, 0.5f, 0f) + (trans.forward * -0.15f), Vector3.down, out hit3, 1f);
            dir = Vector3.Lerp(dir, Vector3.Cross(trans.right, hit1.normal + hit2.normal + hit3.normal), 0.5f).normalized;
        }
        //Debug.DrawRay(trans.position + new Vector3(0f, 0.5f, 0f) + (trans.forward * 0.2f), Vector3.down, Color.green);
        Debug.DrawRay(trans.position + new Vector3(0f, 0.5f, 0f) , dir, Color.green  );
        
    }

    IEnumerator jumpdelayed(float wait)
    {
        yield return new WaitForSeconds(wait);
        rigid.velocity += (trans.up * jumpforce*4f) + (trans.forward * walk *walkspeed);
    }
    IEnumerator sitdelayed(float wait)
    {
        yield return new WaitForSeconds(wait);
        anim.SetBool("sitdown", true);
    }
    IEnumerator nojump()
    {
        yield return new WaitForSeconds(0.5f);
        jumping = false;
        anim.SetBool("jump", jumping);
    }
    IEnumerator wait(float wait)
    {
        active = false;
        yield return new WaitForSeconds(wait);
        active = true;
    }    
}
