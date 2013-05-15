﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameStateManagement
{
    public class Asteroid : Actor
    {
       
        private Vector3 rotationDirection;
        private Random randomNumGenerator;
        private bool isCollision;
        private Utils.Timer collisionDelay = new Utils.Timer();


        public Asteroid(Game game, Vector3 position, int number)
            : base(game)
        {
            worldPosition = position;
            isCollision = false;
            id = number;
            meshName = "Asteroid";
            SetWorldTransformMatrix(Matrix.CreateRotationX(MathHelper.PiOver2));    // rotates 45 degrees or pi/2 radians about the x-axis
          
           


        }

        public override void Initialize()
        {
            base.Initialize();
            bPhysicsDriven = true;
            randomNumGenerator = new Random();

            terminalVelocity = 120.0f;
            const float speed = 100.0f;
            
            velocity.X = (float)randomNumGenerator.Next(-50, 50);
            velocity.Y = (float)randomNumGenerator.Next(-50, 50);
            /*velocity.Normalize();
            velocity *= speed;
            */  // Lab 5

            force = new Vector3((float)randomNumGenerator.NextDouble(), (float)randomNumGenerator.NextDouble(), 0.0f);
            force.Normalize();

            rotationDirection = new Vector3(-1.0f / force.X, 1.0f / force.Y, 0.0f);
            force *= 50.0f;
            rotationDirection.Normalize();

            
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            
            rotation *= Quaternion.CreateFromAxisAngle(rotationDirection, 0.01f);
            Matrix tempTranslate = Matrix.CreateTranslation(worldPosition);
            Matrix tempRotate = Matrix.CreateFromQuaternion(rotation);
            SetWorldTransformMatrix(tempRotate * tempTranslate);
            //worldTransformMatrix = tempTranslate;
            timer.Update(gameTime);
            collisionDelay.Update(gameTime);
            base.Update(gameTime);
            
        }

        public void SetRotationDirection(Vector3 direction){
            rotationDirection = direction;
            rotationDirection.Normalize();
        }

        public bool IsCollision()
        {
            return isCollision;
        }

        public void SetCollision()
        {
            collisionDelay.AddTimer(id.ToString(), 5.0f, ResetCollision, false);
            isCollision = true;
        }

        public void ResetCollision()
        {
            isCollision = false;
        }

       
    }
    
}
