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
	public class Star : Microsoft.Xna.Framework.DrawableGameComponent
	{
		Texture2D myTexture;
		Vector2 vPosition = Vector2.Zero;
		Vector2 vVelocity = Vector2.Zero;
		const float fSpeed = 100.0f;
		SpriteBatch spriteBatch;

        Vector2 normalLeft;
        Vector2 normalRight;
        Vector2 normalTop;
        Vector2 normalBottom;

        const float leftBound = -512.0f;
        const float rightBound = 512.0f;
        const float topBound = -384.0f;
        const float bottomBound = 384.0f;



		Random rand = new Random();

		public Star(Game game)
			: base(game)
		{
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
			DrawOrder = 500;

			// Set velocity to random direction
			vVelocity.X = rand.Next(-1000, 1000);
			vVelocity.Y = rand.Next(-1000, 1000);

			// Set to the const speed
			vVelocity.Normalize();
			vVelocity *= fSpeed;

            // Set normal vectors
            normalLeft = new Vector2(1, 0);    // normal of left surface
            normalRight = new Vector2(-1, 0);    // normal of right surface
            normalTop = new Vector2(0, -1);    // normal of top surface
            normalBottom = new Vector2(0, 1);    // normal of bottom surface
		}


		protected override void LoadContent()
		{
			base.LoadContent();
			myTexture = Game.Content.Load<Texture2D>("supermariostar");
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			// BEGIN: Don't change this code!
			float fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			vPosition += vVelocity * fDeltaTime;
			// END: Don't change this code!

			// TODO: Add your screen boundary detection
			// and reflection here!

            if (vPosition.X > rightBound)
            {
                vVelocity = vVelocity - 2 * normalRight * (Vector2.Dot(vVelocity, normalRight));
            }
            if (vPosition.X < leftBound)
            {
                vVelocity = vVelocity - 2 * normalLeft * (Vector2.Dot(vVelocity, normalLeft));
            }
            if (vPosition.Y > bottomBound)
            {
                vVelocity = vVelocity - 2 * normalBottom * (Vector2.Dot(vVelocity, normalBottom));
            }
            if (vPosition.Y < topBound)
            {
                vVelocity = vVelocity - 2 * normalTop * (Vector2.Dot(vVelocity, normalTop));
            }

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			spriteBatch.Begin();
			Vector2 vDrawPosition = vPosition;
			vDrawPosition.X += 512 - 64;
			vDrawPosition.Y += 384 - 64;
			spriteBatch.Draw(myTexture, vDrawPosition, Color.White);
			spriteBatch.End();
		}
	}
}
