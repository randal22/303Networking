using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerController : MonoBehaviour
{
    
    // values set in engine to 600
    //multipliers applied in calculations
    public float speed;                 
    public float jumpStrength;         

       
    private float movementInput;
    private Rigidbody playerBody;



    // Prevent jumping mid air - stops double jumps
    private float heightAbove;
    


    void Start()
    {
        
        playerBody = GetComponent<Rigidbody>();

              
        heightAbove = GetComponent<Collider>().bounds.extents.y;





    }

    private void FixedUpdate()
    {
        ClientOut.clientDataUpdate();

        foreach (KeyValuePair<int, PlayerManager> player in GameManager.playersDict)
        {
            
            
            if (player.Key != Client.instance.myId)
            {
                Debug.Log("predicting player" + player.Value.id);

                MakePrediction(player.Value);
               
            }

        }








    }

    //interpolation for other playersDict 
    private void MakePrediction(PlayerManager player)
    {

        if (player.recentPos.Count < 3)
        {
            Debug.Log("not enough data to make prediction");
            return;
        }
        else
        {
            //oldest
            Debug.Log(player.id);
            Vector3 oldPos = player.recentPos.Peek();
            player.recentPos.Enqueue(player.recentPos.Dequeue());
            //Debug.Log("oldest postiion stored");
            int s3 = player.secs.Peek();
            player.secs.Enqueue(player.secs.Dequeue());
            int ms3 = player.msecs.Peek();
            player.msecs.Enqueue(player.msecs.Dequeue());

            //basically temp 
            Vector3 tempPos = player.recentPos.Peek();
            //Debug.Log("2nd oldest postiion stored");
            player.recentPos.Enqueue(player.recentPos.Dequeue());
            int s2 = player.secs.Peek();
            player.secs.Enqueue(player.secs.Dequeue());
            int ms2 = player.msecs.Peek();
            player.msecs.Enqueue(player.msecs.Dequeue());

            // Latest
            Vector3 recentPos = player.recentPos.Peek();
            //Debug.Log("newest postiion stored");
            player.recentPos.Enqueue(player.recentPos.Dequeue());
            int s1 = player.secs.Peek();
            player.secs.Enqueue(player.secs.Dequeue());
            int ms1 = player.msecs.Peek();
            player.msecs.Enqueue(player.msecs.Dequeue());

            float t3 = s3 + (ms3 / 1000);
            float t1 = s1 + (ms1 / 1000);

            if (t3 > t1)
            {
                t1 += 60;
            }

            float lerpFactor = (t1- t3)/0.1f;
            //the bigger the gap the heavier the lerp
            float interpx = (1-lerpFactor)* oldPos.x+lerpFactor* recentPos.x; 
            //Debug.Log(interpx);
            float interpy = (1 - lerpFactor) * oldPos.y + lerpFactor * recentPos.y;
            //Debug.Log(interpy);
            player.transform.position.Set(interpx,interpy,0);
            //Debug.Log("predicted pos applied");
        }

       







    }
    





    bool OnFloor()
    {
        return Physics.Raycast(transform.position, -Vector3.up, heightAbove + 0.1f);
    }



    
    void Update()
    {
        
        
        if (OnFloor())
        {
            // If space is pressed
            if (Input.GetAxis("Jump") != 0)
            {


                // Using the ray add a force to the player
                Vector2 jumpVec = Vector2.up;
                playerBody.AddForce(jumpVec * jumpStrength * Time.deltaTime, ForceMode.Impulse);



            }
                        
        }
        //handle movement
        //horizontal in unity means 'a'/'d' key down
        movementInput = Input.GetAxis("Horizontal");
        playerBody.AddForce(Vector3.right * Time.deltaTime * speed * movementInput, ForceMode.Force);
        
    }
}





