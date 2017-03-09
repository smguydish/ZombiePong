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

namespace ZombiePong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background, spritesheet;

        Sprite paddle1, paddle2, ball;

        List<Sprite> zombies = new List<Sprite>();
        int Score1, Score2;

        float ballSpeed = 180;
        Random rand = new Random(System.Environment.TickCount);


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            spritesheet = Content.Load<Texture2D>("spritesheet");

            paddle1 = new Sprite(new Vector2(20, 20), spritesheet, new Rectangle(0, 516, 25, 150), Vector2.Zero);
            paddle2 = new Sprite(new Vector2(970, 20), spritesheet, new Rectangle(32, 516, 25, 150), new Vector2(0, 400));
            ball = new Sprite(new Vector2(700, 350), spritesheet, new Rectangle(76, 510, 40, 40), new Vector2(350, -150));

            SpawnZombie(new Vector2(10, 600), new Vector2(-45, 0));
            SpawnZombie(new Vector2(420, 300), new Vector2(50, 0));
            SpawnZombie(new Vector2(100, 10), new Vector2(60, 0));

            Score1 = 0;
            Score2 = 0;

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void SpawnZombie(Vector2 location, Vector2 velocity)
        {
            Sprite zombie = new Sprite(location, spritesheet, new Rectangle(0, 25, 155, 145), velocity);

            for (int i = 1; i < 10; i++)
            {
                zombie.AddFrame(new Rectangle(i * 165, 25, 160, 150));
            }

            zombies.Add(zombie);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            ball.Update(gameTime);

            Window.Title = "Player: " + Score1 + " Computer: " + Score2;

            for (int i = 0; i < zombies.Count; i++)
            {
                zombies[i].Update(gameTime);
                // Zombie logic goes here..

                if (zombies[i].Location.X >= 875)
                {
                    zombies[i].FlipHorizontal = false;
                    zombies[i].Velocity *= new Vector2(-1, 1);
                }

                if (zombies[i].Location.X <= 0)
                {
                    zombies[i].FlipHorizontal = true;
                    zombies[i].Velocity *= new Vector2(-1, 1);
                }

                if (zombies[i].Velocity.X > 0)
                {
                    zombies[i].FlipHorizontal = true;
                }
                
                else
                    zombies[i].FlipHorizontal = false;

                if (ball.IsBoxColliding(zombies[i].BoundingBoxRect))
                {
                    ball.Velocity *= new Vector2(-1, 1);
                    ball.FlipHorizontal = true;
                }
            }

            if (rand.Next(0, 400) < 100)
            {
                paddle2.Velocity = new Vector2(0, ball.Center.Y - paddle2.Center.Y);
            }
            

            if (paddle2.IsBoxColliding(ball.BoundingBoxRect))
            {
                ball.Velocity *= new Vector2(-1, 1);
                ball.FlipHorizontal = true;
                ballSpeed += 50;
            }

            else if (paddle1.IsBoxColliding(ball.BoundingBoxRect))
            {
                ball.Velocity *= new Vector2(-1, 1);
                ball.FlipHorizontal = false;
                ballSpeed += 50;
            }
            
            else if (ball.Location.Y < 0)
            {
                ball.Velocity *= new Vector2(1, -1);
            }

            else if (ball.Location.Y > 725)
            {
                ball.Velocity *= new Vector2(1, -1);
            }
            MouseState ms = Mouse.GetState();
            paddle1.Location = new Vector2(paddle1.Location.X, (float)ms.Y - 75);
            base.Update(gameTime);

            Vector2 vel = ball.Velocity;
            vel.Normalize();
            vel = vel * ballSpeed;
            ball.Velocity = vel;
            if (ball.Location.X <= 0)
            {
                Score2 = Score2 + 1;
                ball = new Sprite(new Vector2(500, 350), spritesheet, new Rectangle(76, 510, 40, 40), new Vector2(250, -100));
                ballSpeed = 350;
            }

            if (ball.Location.X >= 1000)
            {
                Score1 = Score1 + 1;
                ball = new Sprite(new Vector2(500, 350), spritesheet, new Rectangle(76, 510, 40, 40), new Vector2(250, -100));
                ballSpeed = 350;
            }

            if (paddle2.IsBoxColliding(ball.BoundingBoxRect))
            {
                ball.Velocity *= new Vector2(-1, 1);
                ball.Location = new Vector2(paddle2.Location.X - paddle2.BoundingBoxRect.Width, ball.Location.Y);
            }
            if (paddle1.IsBoxColliding(ball.BoundingBoxRect))
            {
                ball.Velocity *= new Vector2(-1, 1);
                ball.Location = new Vector2(paddle1.Location.X + paddle1.BoundingBoxRect.Width, ball.Location.Y);
            }

            paddle1.Location = new Vector2(paddle1.Location.X, MathHelper.Clamp(ms.Y, 0, this.Window.ClientBounds.Height - paddle1.BoundingBoxRect.Height));
            paddle2.Location = new Vector2(paddle2.Location.X, MathHelper.Clamp(paddle2.Location.Y, 0, this.Window.ClientBounds.Height - paddle2.BoundingBoxRect.Height));
            paddle2.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            paddle1.Draw(spriteBatch);
            paddle2.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            for (int i = 0; i < zombies.Count; i++)
            {
                zombies[i].Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
