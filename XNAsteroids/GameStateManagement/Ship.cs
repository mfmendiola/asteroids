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
    class Ship : Actor
    {


     private bool respawnInvincibility = false;  // ship is invincible for a given amount of time after spawning, prevents spawning into asteroid
     private Utils.Timer respawnTimer = new Utils.Timer();
     public Ship(Game game)
            : base(game)
        {
            meshName = "Ship";
            rotation = Quaternion.CreateFromAxisAngle(new Vector3(1.0f,0.0f,0.0f), MathHelper.PiOver2);    // rotates 90 degrees or pi/2 radians about the x-axis
            velocity = Vector3.Zero;
            worldPosition = Vector3.Zero;

            SetWorldTransformMatrix(Matrix.CreateFromQuaternion(rotation));// *Matrix.CreateTranslation(worldPosition);
            //velocity = new Vector3(0.0f,0.0f,1.0f);
           // rotation = Quaternion.Identity;
            bPhysicsDriven = true;
            terminalVelocity = 5000.0f;
        }

     public override void Initialize()
     {
         base.Initialize();
         SetRespawnInvincibility();
         GameplayScreen.soundBank.PlayCue("Ship_Spawn");


     }

     public override void Update(GameTime gameTime)
     {
         base.Update(gameTime);
         
         Matrix tempTranslate = Matrix.CreateTranslation(worldPosition);
         Matrix tempRotate = Matrix.CreateFromQuaternion(rotation);
         SetWorldTransformMatrix(tempRotate * tempTranslate);
         //velocity = Vector3.Zero;
         force = Vector3.Zero;
         respawnTimer.Update(gameTime);

         
     }

        
        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            meshData.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mesh in meshData.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if(respawnInvincibility)
                        effect.Alpha = 0.1f;
                    else
                        effect.Alpha = 1.0f;
                }
                mesh.Draw();
            }
         

            base.Draw(gameTime);
     
        }
     public void SetRespawnInvincibility()
     {
         respawnInvincibility = true;
         respawnTimer.AddTimer("respawn", 5.0f, ResetRespawnInvincibility, false);
     }

     public void ResetRespawnInvincibility()
     {
         respawnInvincibility = false;
     }

     public bool GetRespawnInvincibility()
     {
         return respawnInvincibility;
     }

    }
}
