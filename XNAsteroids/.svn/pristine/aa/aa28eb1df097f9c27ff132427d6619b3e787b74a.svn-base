using System;
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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 


  
    public class SpawnManager : Microsoft.Xna.Framework.GameComponent
    {
        protected System.Collections.ArrayList asteroids = new System.Collections.ArrayList();
        protected System.Collections.ArrayList missiles = new System.Collections.ArrayList();
        private Utils.Timer timer;


        private Asteroid tempAsteroid;

        private Random randomNumGenerator;

        private Missile tempMissile;

        private int missileCount = 0;
        private int asteroidCount = 0;
       

        public SpawnManager(Game game)
            : base(game)
        {
            timer = new Utils.Timer();
            timer.AddTimer("asteroidTimer", 5.0f, spawnAsteroid, true);
            
            randomNumGenerator = new Random();
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code htere

            
          /*  foreach(Asteroid a in asteroids){
                a.worldTransformMatrix = Matrix.CreateTranslation(a.GetWorldPosition() - fDeltaTime*60.0f*new Vector3(0.0f, 1.0f, 0.0f));
      
            }

           */
            timer.Update(gameTime);
            base.Update(gameTime);
        }

        public System.Collections.ArrayList getAsteroids()
        {
            return asteroids;
        }

        public void spawnAsteroid()
        {
            if (asteroids.Count < 10)
            {

                tempAsteroid = new Asteroid(base.Game, new Vector3((float)randomNumGenerator.Next(-100, 100) * 5.120f, (float)randomNumGenerator.Next(-100, 100) * 3.120f, 0.0f), asteroidCount);

              
                base.Game.Components.Add(tempAsteroid);
                asteroids.Add(tempAsteroid);
                asteroidCount++;
            }

           

        }

        public void spawnMissile(Vector3 startPosition, Vector3 startVector, Quaternion startRotation)
        {
            tempMissile = new Missile(base.Game, missileCount);

            tempMissile.SetWorldPosition(startPosition);
            if (startVector != Vector3.Zero) 
            startVector.Normalize();
           // tempMissile.SetVelocityXY(startVector.X, startVector.Y);
            tempMissile.SetForce(new Vector3(1000.0f*startVector.X, 1000.0f*startVector.Y, 0.0f));
            tempMissile.SetRotation(startRotation*tempMissile.GetRotation());
            base.Game.Components.Add(tempMissile);
            missiles.Add(tempMissile);
         
            timer.AddTimer(tempMissile.GetId().ToString(), 3.0f, removeMissile, false);
            missileCount++;
            GameplayScreen.soundBank.PlayCue("Ship_Missile");

        }

        public void removeMissile()
        {

                
                base.Game.Components.Remove((Missile)missiles[0]);
                missiles.Remove(missiles[0]);
            



        }

        public void removeMissile(Missile m)
        {
            
                base.Game.Components.Remove(m);
                missiles.Remove(m);
                timer.RemoveTimer(m.GetId().ToString());


        }

        public void removeAsteroid(Asteroid a)
        {

            base.Game.Components.Remove(a);
            asteroids.Remove(a);


        }

        public System.Collections.ArrayList GetMissiles()
        {
            return missiles;
        }

        public System.Collections.ArrayList GetAsteroids()
        {
            return asteroids;
        }

      
    }
}
