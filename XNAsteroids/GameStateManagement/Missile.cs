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
    public class Missile : Actor
    {


        
     public Missile(Game game, int number)
            : base(game)
        {
            meshName = "Missile";
            rotation = Quaternion.CreateFromAxisAngle(new Vector3(0.0f,1.0f,0.0f), MathHelper.Pi);    // rotates 90 degrees or pi/2 radians about the x-axis
            //velocity = new Vector3(0.0f,0.0f,1.0f);
            bPhysicsDriven = true;
            id = number;
            terminalVelocity = 400.0f;
        }

     public override void Initialize()
     {
         base.Initialize();


     }

     public override void Update(GameTime gameTime)
     {
         base.Update(gameTime);
         Matrix tempTranslate = Matrix.CreateTranslation(worldPosition);
         Matrix tempRotate = Matrix.CreateFromQuaternion(rotation);
         SetWorldTransformMatrix(tempRotate * tempTranslate);
         
     }

     
    }
}
