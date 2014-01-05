#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace yawn
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Yawn : Game
    {
        const int BoardWidth = 60;
        const int BoardHeight = 40;
        const int InitialSpeed = 15; // Speed represents number of ticks between worm updates
        static int Ticks = 0;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        YawnWorm Worm;
        int Points;
        bool Paused, GameOver;
        float GameSpeed;
        Texture2D Tile, DaklutzLogo, BlankTexture;
        SpriteFont Font;
        Vector3 Food;

        public Yawn()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Worm = new YawnWorm();
            GameSpeed = InitialSpeed;
            Points = 0;
            Paused = true;
            GameOver = false;
            Food = new Vector3(0, 0, 1);
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

            Font = Content.Load<SpriteFont>("Calibri");
            BlankTexture = Content.Load<Texture2D>("blank");
            DaklutzLogo = Content.Load<Texture2D>("daklutz");
            Tile = Content.Load<Texture2D>("tile");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Ticks++;
            // Check for pause
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Paused = true;
            }
            
            if (Paused) {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up) ||
                    Keyboard.GetState().IsKeyDown(Keys.Down) ||
                    Keyboard.GetState().IsKeyDown(Keys.Left) ||
                    Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    Paused = false;
                }
            }
            else if (GameOver == false)
            {
                // Update that worm
                List<Direction> Dirs = new List<Direction>();
                if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
                    Dirs.Add(Direction.NORTH);
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
                    Dirs.Add(Direction.SOUTH);
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
                    Dirs.Add(Direction.WEST);
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
                    Dirs.Add(Direction.EAST);
                }

                // Only update as fast as the game is going
                if (Ticks % GameSpeed == 0)
                {
                    Ticks = 0;
                    if (Worm.Update(gameTime, Dirs) == false)
                    {
                        GameOver = true;
                    }

                }

                // check for collision with the walls
                if (Worm.Position.X <= 0 || Worm.Position.X >= BoardWidth || Worm.Position.Y <= 0 || Worm.Position.Y >= BoardHeight)
                {
                    GameOver = true;
                }

                // Check for collision with food
                if (Food.X != 0)
                {
                    if (Worm.Position.X == Food.X && Worm.Position.Y == Food.Y)
                    {
                        Worm.Eat((int)Food.Z);
                        Points += (int)Food.Z;
                        Food.Z++;
                        Food.X = 0;
                        // speed up the game
                        if (GameSpeed > 1 && Food.Z % 4 == 0)
                        {
                            GameSpeed--;
                        }
                    }
                }
                else
                {
                    // respawn food
                    Random random = new Random();
                    do
                    {
                        Food.X = random.Next(1, BoardWidth + 1);
                        Food.Y = random.Next(1, BoardHeight + 1);
                    } while (Worm.SectionHere(Food));
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Render the HUD
            spriteBatch.Begin();
            //spriteBatch.DrawString(Font, gameTime.TotalGameTime.ToString(), new Vector2(1, 1), Color.Black);

            spriteBatch.DrawString(Font, "GameOver: " + GameOver.ToString(), new Vector2(1, BoardHeight * 10 + 1), Color.Black);
            spriteBatch.DrawString(Font, "Paused: " + Paused.ToString(), new Vector2(1, BoardHeight * 10 + 14), Color.Black);
            spriteBatch.DrawString(Font, "X: " + Worm.Position.X.ToString(), new Vector2(1, BoardHeight * 10 + 27), Color.Black);
            spriteBatch.DrawString(Font, "Y: " + Worm.Position.Y.ToString(), new Vector2(1, BoardHeight * 10 + 40), Color.Black);
            spriteBatch.DrawString(Font, "Food.X: " + Food.X.ToString(), new Vector2(100, BoardHeight * 10 + 27), Color.Black);
            spriteBatch.DrawString(Font, "Food.Y: " + Food.Y.ToString(), new Vector2(100, BoardHeight * 10 + 40), Color.Black);
            spriteBatch.DrawString(Font, "Food Worth: " + Food.Z.ToString(), new Vector2(100, BoardHeight * 10 + 53), Color.Black);
            spriteBatch.DrawString(Font, "Points: " + Points.ToString(), new Vector2(1, BoardHeight * 10 + 53), Color.Black);

            // Render the food
            spriteBatch.Draw(BlankTexture, new Vector2(Food.X * 10.0f, Food.Y * 10.0f), Color.Green);

            // Render the board
            for (int i = 0; i <= BoardWidth; i++)
            {
                for (int j = 0; j <= BoardHeight; j++)
                {
                    if (i == 0 ||
                        i == BoardWidth ||
                        j == 0 ||
                        j == BoardHeight)
                    {
                        spriteBatch.Draw(BlankTexture, new Vector2(i * 10.0f, j * 10.0f), Color.Black);
                    }
                }
            }
            
            spriteBatch.End();

            // Render the worm
            if (Paused == false)
            {
                Worm.Draw(gameTime, spriteBatch, BlankTexture);
            }

            // Render the food

            base.Draw(gameTime);
        }
    }
}
