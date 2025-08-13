using CAC.checks.events.impl;
using CAC.data;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.checks.impl.movement.invalid
{
    public class InvalidPosition() : Check("InvalidPosition", CheckLevel.B, "Basically, checks for ground spoofing", 5, 5)
    {
        public Transform orientation;

        //Other
        private Rigidbody rb;

        //Rotation and look
        private float xRotation;
        private float sensitivity = 50f;
        private float sensMultiplier = 1f;

        //Movement
        public float moveSpeed = 4500;
        public float maxSpeed = 20;
        public bool grounded;
        public LayerMask whatIsGround;

        public float counterMovement = 0.175f;
        private float threshold = 0.01f;
        public float maxSlopeAngle = 35f;

        //Crouch & Slide
        private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
        private Vector3 playerScale;
        public float slideForce = 400;
        public float slideCounterMovement = 0.2f;

        //Jumping
        private bool readyToJump = true;
        private float jumpCooldown = 0.25f;
        public float jumpForce = 550f;

        //Input
        float x, y;
        bool jumping, sprinting, crouching;

        //Sliding
        private Vector3 normalVector = Vector3.up;
        private Vector3 wallNormalVector;

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;

            if(rb == null)
            {
                rb = new Rigidbody();
            }

            Movement();

            double diffX = positionTracker.x - rb.position.x;
            double diffY = positionTracker.y - rb.position.y;
            double diffZ = positionTracker.z - rb.position.z;

            double allowance = 0;

            if(diffX > allowance || diffY > allowance || diffZ > 0)
            {
                if(this.Buffer.increase() > 5)
                {
                    this.fail();
                }
            } else
            {
                this.Buffer.decrease();
            }

            base.handleMovementUpdate(e);
        }

        private void Jump()
        {
            if (grounded && readyToJump)
            {
                readyToJump = false;

                //Add jump forces
                rb.AddForce(Vector2.up * jumpForce * 1.5f);
                rb.AddForce(normalVector * jumpForce * 0.5f);

                //If jumping while falling, reset y velocity.
                Vector3 vel = rb.velocity;
                if (rb.velocity.y < 0.5f)
                    rb.velocity = new Vector3(vel.x, 0, vel.z);
                else if (rb.velocity.y > 0)
                    rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            }
        }

        private void Movement()
        {
            //Extra gravity
            rb.AddForce(Vector3.down * Time.deltaTime * 10);

            //Find actual velocity relative to where player is looking
            Vector2 mag = FindVelRelativeToLook();
            float xMag = mag.x, yMag = mag.y;

            //Counteract sliding and sloppy movement
            CounterMovement(x, y, mag);

            //If holding jump && ready to jump, then jump
            if (readyToJump && jumping) Jump();

            //Set max speed
            float maxSpeed = this.maxSpeed;

            //If sliding down a ramp, add force down so player stays grounded and also builds speed
            if (crouching && grounded && readyToJump)
            {
                rb.AddForce(Vector3.down * Time.deltaTime * 3000);
                return;
            }

            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (x > 0 && xMag > maxSpeed) x = 0;
            if (x < 0 && xMag < -maxSpeed) x = 0;
            if (y > 0 && yMag > maxSpeed) y = 0;
            if (y < 0 && yMag < -maxSpeed) y = 0;

            //Some multipliers
            float multiplier = 1f, multiplierV = 1f;

            // Movement in air
            if (!grounded)
            {
                multiplier = 0.5f;
                multiplierV = 0.5f;
            }

            // Movement while sliding
            if (grounded && crouching) multiplierV = 0f;

            //Apply forces to move player
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        }

        private void CounterMovement(float x, float y, Vector2 mag)
        {
            if (!grounded || jumping) return;

            //Slow down sliding
            if (crouching)
            {
                rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
                return;
            }

            //Counter movement
            if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
            {
                rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
            }
            if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
            {
                rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
            }

            //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
            if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
            {
                float fallspeed = rb.velocity.y;
                Vector3 n = rb.velocity.normalized * maxSpeed;
                rb.velocity = new Vector3(n.x, fallspeed, n.z);
            }
        }

        public Vector2 FindVelRelativeToLook()
        {
            float lookAngle = orientation.transform.eulerAngles.y;
            float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

            float u = Mathf.DeltaAngle(lookAngle, moveAngle);
            float v = 90 - u;

            float magnitue = rb.velocity.magnitude;
            float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
            float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

            return new Vector2(xMag, yMag);
        }

        private bool IsFloor(Vector3 v)
        {
            float angle = Vector3.Angle(Vector3.up, v);
            return angle < maxSlopeAngle;
        }
    }
}
