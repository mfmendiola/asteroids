#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

		Utils.Timer m_kTimer = new Utils.Timer();

        public static AudioEngine audioEngine;
        public static WaveBank waveBank;
        public static SoundBank soundBank;

        private float rotationAngle = 0.05f;
        private float throttleForce = 150.0f;
        private float reverseThrottleForce;
   

		Star m_Star;

        Ship m_kShip;

        SpawnManager m_kSpawnManager;

        bool cooldown = false;

        

        public static Matrix cameraMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 2000.0f), Vector3.Zero, Vector3.UnitY);
        public static Matrix projectionMatrix = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 10000.0f);
        public static Vector3 ambientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
        public static Vector3 specularColor = new Vector3(1.0f, 1.0f, 1.0f);
        public static float specularPower = 80.0f;
        public static Vector3 lightDirection = new Vector3(0.5f, 0.2f, 0.0f);
        public static Vector3 lightDiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
        private System.Collections.ArrayList collisions = new System.Collections.ArrayList(); // to keep track of all objects that have collided in a given frame
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            reverseThrottleForce = -2f * throttleForce;
            
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            audioEngine = new AudioEngine("Content/Sounds.xgs");
            waveBank = new WaveBank(audioEngine, "Content/XNAsteroids Waves.xwb");
            soundBank = new SoundBank(audioEngine, "Content/XNAsteroids Cues.xsb");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

			//m_kTimer.AddTimer("Timer 1", 10.0f, new Utils.TimerDelegate(TimerOneShot), false);
			//m_kTimer.AddTimer("Timer 2", 1.0f, new Utils.TimerDelegate(TimerLoop), true);
			//m_kTimer.AddTimer("Timer 3", 10.0f, new Utils.TimerDelegate(TimerLoop), true);
			//m_kTimer.AddTimer("Timer 4", 5.0f, new Utils.TimerDelegate(TimerOneShot), false);
			//m_kTimer.AddTimer("Timer 5", 22.0f, new Utils.TimerDelegate(TimerLoopRemove), true);
			m_Star = new Star(ScreenManager.Game);
			ScreenManager.Game.Components.Add(m_Star);

            m_kShip = new Ship(ScreenManager.Game);
            ScreenManager.Game.Components.Add(m_kShip);

            m_kSpawnManager = new SpawnManager(ScreenManager.Game);
            ScreenManager.Game.Components.Add(m_kSpawnManager);

           
 
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(200, 200);

                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);

                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)

                // checks for missile-asteroid collision
                if (m_kSpawnManager.GetAsteroids() != null && m_kSpawnManager.GetAsteroids().Count > 0)
                {
                    foreach (Asteroid a in m_kSpawnManager.GetAsteroids())
                    {
                        if (m_kSpawnManager.GetMissiles() != null && m_kSpawnManager.GetMissiles().Count > 0)
                        {
                            foreach (Missile m in m_kSpawnManager.GetMissiles())
                            {
                                if (m.GetWorldBounds().Intersects(a.GetWorldBounds()) || a.GetWorldBounds().Intersects(m.GetWorldBounds()))
                                {
                                    collisions.Add(m);
                                    collisions.Add(a);
                                    break;
                                }
                            }
                        }

                    }
                    foreach (Object o in collisions)
                    {
                        if (o is Missile)
                            m_kSpawnManager.removeMissile((Missile)o);
                        else if (o is Asteroid)
                            m_kSpawnManager.removeAsteroid((Asteroid)o);
                    }
                }
                if (m_kSpawnManager.GetAsteroids() != null && m_kSpawnManager.GetAsteroids().Count > 0)
                {
                    foreach (Asteroid a in m_kSpawnManager.GetAsteroids())
                    {
                        if (m_kShip != null)
                        {
                            if (!m_kShip.GetRespawnInvincibility())
                            {
                                if (a.GetWorldBounds().Intersects(m_kShip.GetWorldBounds()) || m_kShip.GetWorldBounds().Intersects(a.GetWorldBounds()))
                                {

                                    m_kSpawnManager.removeAsteroid(a);
                                    removeShip();


                                    break;

                                }
                            }
                        }
                    }
                }
                
                if (m_kSpawnManager.GetAsteroids() != null && m_kSpawnManager.GetAsteroids().Count > 0)
                {
                    foreach (Asteroid a1 in m_kSpawnManager.GetAsteroids())
                    {
                        foreach(Asteroid a2 in m_kSpawnManager.GetAsteroids())
                        {
                            if (a1 != a2)
                            {
                                if (a1.GetWorldBounds().Intersects(a2.GetWorldBounds()) || a2.GetWorldBounds().Intersects(a1.GetWorldBounds()))
                                {
                                    if (!a1.IsCollision() && !a2.IsCollision())
                                    {
                                        a1.SetVelocityXY(-a1.GetVelocity().X, -a1.GetVelocity().Y);
                                        a1.SetForce(new Vector3(-a1.GetForce().X, -a1.GetForce().Y, 0.0f));
                                        a1.SetAcceleration(new Vector3(-a1.GetAcceleration().X, -a1.GetAcceleration().Y, 0.0f));
                                        a1.SetRotationDirection(new Vector3(-1.0f / a1.GetForce().X, 1.0f / a1.GetForce().Y, 0.0f));

                                        a2.SetVelocityXY(-a2.GetVelocity().X, -a2.GetVelocity().Y);
                                        a2.SetForce(new Vector3(-a2.GetForce().X, -a2.GetForce().Y, 0.0f));
                                        a2.SetRotationDirection(new Vector3(-1.0f / a2.GetForce().X, 1.0f / a2.GetForce().Y, 0.0f));
                                        a2.SetAcceleration(new Vector3(-a2.GetAcceleration().X, -a2.GetAcceleration().Y, 0.0f));

                                        a1.SetCollision();
                                        a2.SetCollision();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                m_kTimer.Update(gameTime);

            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
		public override void HandleInput(InputState input, GameTime gameTime)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
            else
            {/*
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;
                float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Left))
                    movement.X--;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Right))
                    movement.X++;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Up))
                    movement.Y--;

                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = input.CurrentGamePadStates[0].ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * (float)60 * deltaTime ;  // change so that movement is frame-rate independent
              * 
              * */
                if (m_kShip != null)
                {

                    if (input.ShipFire && !cooldown)
                    {
                        m_kSpawnManager.spawnMissile(m_kShip.GetWorldPosition(), m_kShip.GetWorldFacing(), m_kShip.GetRotation());
                        cooldown = true;
                        m_kTimer.AddTimer("missileCooldown", 1.0f, resetCooldown, false);
                    }
                    if (input.ShipMove && !input.ShipReverse)
                    {
                        Vector3 temp = m_kShip.GetWorldFacing();

                        temp.Normalize();
                        // m_kShip.SetVelocityXY(temp.X, temp.Y);
                        m_kShip.SetForce(new Vector3(throttleForce * temp.X, throttleForce * temp.Y, 0.0f));

                    }

                    if (!input.ShipMove && input.ShipReverse)
                    {
                        Vector3 temp = m_kShip.GetWorldFacing();

                        temp.Normalize();
                        //m_kShip.SetVelocityXY(-temp.X, -temp.Y);
                        Vector3 velocityCheck = m_kShip.GetVelocity();
                        if (velocityCheck != Vector3.Zero)
                            velocityCheck.Normalize();
                        if (temp.X * velocityCheck.X >= 0.0f && temp.Y * velocityCheck.Y >= 0.0f)
                        {
                            m_kShip.SetForce(new Vector3(reverseThrottleForce * temp.X, reverseThrottleForce * temp.Y, 0.0f));
                        }
                        else
                        {
                            m_kShip.SetVelocityXY(0.0f, 0.0f);
                            m_kShip.SetForce(new Vector3(0.0f, 0.0f, 0.0f));
                            m_kShip.SetAcceleration(Vector3.Zero);
                        }


                    }



                    if (input.ShipTurnLeft && !input.ShipTurnRight)
                    {
                        Vector3 temp = m_kShip.GetWorldFacing();
                        m_kShip.SetRotation(m_kShip.GetRotation() * Quaternion.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), rotationAngle));

                        m_kShip.SetAcceleration(Vector3.Zero);
                        if (m_kShip.GetVelocity().X > 0.0f && m_kShip.GetVelocity().Y > 0.0f)
                            m_kShip.SetForce(new Vector3(throttleForce * temp.X, throttleForce * temp.Y, 0.0f));
                    }
                    if (input.ShipTurnRight && !input.ShipTurnLeft)  // to prevent from turning both left and right at the same time
                    {
                        Vector3 temp = m_kShip.GetWorldFacing();
                        m_kShip.SetRotation(m_kShip.GetRotation() * Quaternion.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), -rotationAngle));

                        m_kShip.SetAcceleration(Vector3.Zero);
                        if(m_kShip.GetVelocity().X > 0.0f && m_kShip.GetVelocity().Y > 0.0f)
                            m_kShip.SetForce(new Vector3(throttleForce * temp.X, throttleForce * temp.Y, 0.0f));
                    }
                }

            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

            //spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
            //                       enemyPosition, Color.DarkRed);

			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 1"), new Vector2(20.0f, 500.0f), Color.Blue);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 2"), new Vector2(20.0f, 550.0f), Color.White);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 3"), new Vector2(20.0f, 600.0f), Color.White);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 4"), new Vector2(20.0f, 650.0f), Color.Blue);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 5"), new Vector2(20.0f, 700.0f), Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        public void removeShip(){

            ScreenManager.Game.Components.Remove(m_kShip);
            m_kShip = null;
            m_kTimer.AddTimer("shipRespawn", 5.0f, spawnShip, false);
                
        }

        public void spawnShip(){
            m_kShip = new Ship(ScreenManager.Game);
            ScreenManager.Game.Components.Add(m_kShip);
            GameplayScreen.soundBank.PlayCue("Ship_Spawn");
        }

        #endregion

		#region Timer Test Functions
		void TimerOneShot()
		{
			Console.WriteLine("TimerOneShot fired!");
		}

		void TimerLoop()
		{
			Console.WriteLine("TimerLoop fired!");
		}

		void TimerLoopRemove()
		{
			Console.WriteLine("TimerLoopRemove fired!");
			m_kTimer.RemoveTimer("Timer 3");
		}

		string MakeTimerDebugString(string sTimerName)
		{
			if (m_kTimer.GetTriggerCount(sTimerName) != -1)
				return sTimerName + " - Time: " + m_kTimer.GetRemainingTime(sTimerName).ToString("f3")
					+ " Count: " + m_kTimer.GetTriggerCount(sTimerName);
			else
				return sTimerName + " not found! ";
		}

        void resetCooldown()
        {
            cooldown = false;
        }
		#endregion
	}
}
