﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics
{
    // Radius of the ball
    private float radius;
    
    // Ball bounciness
    private float bounciness;
    
    // Ball hits array
    RaycastHit[] ballHits = new RaycastHit[1];

    private float groundY;

    private PointManager _pointManager;
    private SoundManager _soundManager;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public BallPhysics(float radius, float bounciness, float grndY, SoundManager sndManager, PointManager pointManager)
    {
        this.radius = radius;
        this.bounciness = bounciness;
        this.groundY = grndY;
        _pointManager = pointManager;
        _soundManager = sndManager;
    }
    
    // --------------------------------------------------------------------------------
    public BallInfo UpdateBallInfo(BallInfo bi, float dt)
    {
        // Apply gravity
        bi.velocity += new Vector3(0.0f, Physics.gravity.y * dt, 0.0f);

        // If there is upwards velocity, then the ball is definitely not grounded..
        if (bi.velocity.y > 0.0f) bi.grounded = false;

        // If the ball is grounded, apply friction to its movement
        if (bi.grounded)
        {
            var vx = bi.velocity.x * 0.96f;
            var vy = bi.velocity.z * 0.96f;

            bi.velocity = new Vector3(vx, bi.velocity.y, vy);
	        
            bi.eulerAngles = new Vector3(bi.velocity.x, 0.0f, bi.velocity.z).magnitude * Vector3.right * dt * 200.0f;
        }
        else
        {
            bi.eulerAngles = new Vector3(bi.velocity.x, 0.0f, bi.velocity.z).magnitude * Vector3.right * dt * 100.0f;
        }

        // Modify the position of the ball applying velocity
        bi.Position += bi.velocity * dt;
        
        // If the ball is not on the floor, then it's definitely not grounded.
        if (bi.Position.y - radius > groundY)
            bi.grounded = false;

        
        // If the ball touches the floor, then it's grounded
        if (bi.Position.y - radius <= groundY)
        {
            if (!bi.grounded)
            {
                float bounceSpeed = bi.velocity.y * -bounciness;
                bounceSpeed = (bounceSpeed > 0.001f ? bounceSpeed : 0.0f);
                bi.velocity = new Vector3(bi.velocity.x * 0.85f, bounceSpeed, bi.velocity.z * 0.85f);
                
                _pointManager.SetCourtBallBounce();
                _pointManager.HandleBallBounce(new Vector2(bi.Position.x, bi.Position.z));
                _soundManager.PlayBallBounce(bi.velocity);
                Debug.Log("BOUNCE WITH FLOOR");
            }
            else
            {
                bi.velocity = new Vector3(bi.velocity.x, 0f, bi.velocity.z);
            }
          
            // Clamp position above the floor
            bi.Position = new Vector3(bi.Position.x, groundY + radius, bi.Position.z);

            bi.grounded = true;
        }

        return bi;
    }
}
