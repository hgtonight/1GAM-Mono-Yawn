#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
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
        const int BoardWidth = 50;
        const int BoardHeight = 30;
        const int InitialSpeed = 10; // Speed represents number of ticks between worm updates
        const int GridSize = 16; // size in pixels each grid section is
        static int Ticks = 0;

        Color BgColor, MenuColor;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Worm Worm;
        Board Board;
        Food Food;
        int Points;
        bool Paused, GameOver;
        float GameSpeed;
        Texture2D SpriteSheet, DaklutzLogo, ColorTexture;
        SpriteFont Font;
        SoundEffect Eat, Impact;
        Vector2 LogoPosition, TextPosition;
        
        public Yawn()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            ResetGame(0);
            GameOver = true;
        }

        private void ResetGame(int Type)
        {
            Board = new Board(BoardWidth, BoardHeight, GridSize, Type);
            Worm = new Worm(GridSize);
            Food = new Food(GridSize);
            GameSpeed = InitialSpeed;
            Points = 0;
            Paused = true;
            GameOver = false;
            BgColor = new Color(68, 34, 9);
            MenuColor = Color.DarkGreen;
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

            Eat = Content.Load<SoundEffect>("eat");
            Impact = Content.Load<SoundEffect>("impact1");
            Font = Content.Load<SpriteFont>("PressStart2P");
            SpriteSheet = Content.Load<Texture2D>("sprites");
            DaklutzLogo = Content.Load<Texture2D>("daklutz");
            ColorTexture = new Texture2D(GraphicsDevice, 1, 1);
            ColorTexture.SetData(new Color[] { Color.White });

            LogoPosition = new Vector2((BoardWidth * GridSize / 2) - (DaklutzLogo.Width / 2), (BoardHeight * GridSize / 2) - (DaklutzLogo.Height / 2));
            TextPosition = new Vector2(BoardWidth * GridSize / 2, LogoPosition.Y + DaklutzLogo.Height);
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
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    Dirs.Add(Direction.NORTH);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    Dirs.Add(Direction.SOUTH);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    Dirs.Add(Direction.WEST);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
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

                // Check for wrapping around the edge of the board
                if (Board.OutOfBounds(Worm.Position))
                {
                    Worm.WrapPosition(BoardWidth, BoardHeight);
                }


                // check for collision with the walls
                if (Board.Collides(Worm.Position))
                {
                    GameOver = true;
                    Impact.Play();
                }

                // Respawn food if needed
                if (Food.IsEaten())
                {
                    Vector2 NewPosition = new Vector2(-1, -1);
                    Random random = new Random();
                    do
                    {
                        NewPosition.X = random.Next(0, BoardWidth);
                        NewPosition.Y = random.Next(0, BoardHeight);
                    } while (Worm.Collides(NewPosition) || Board.Collides(NewPosition));

                    Food.Respawn(NewPosition);
                }

                // Check for collision with food
                if (Food.Collides(Worm.Position))
                {
                    Worm.Eat(Food.Potency());
                    Points++;

                    // speed up the game
                    if (GameSpeed > 1 && Food.Potency() % 4 == 0)
                    {
                        GameSpeed--;
                    }
                    Food.Devoured();
                    Eat.Play();
                }
            }
            else
            {
                // Game over
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up) ) {
                    ResetGame(0);
                }
                else if(Keyboard.GetState().IsKeyDown(Keys.Down) ) {
                    ResetGame(-1);
                }
                else if(Keyboard.GetState().IsKeyDown(Keys.Left) ) {
                    ResetGame(-2);
                }
                else if(Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    ResetGame(50);
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
            Vector2 PointOffset = new Vector2(Font.MeasureString(Points.ToString()).Length() / 2, 0);
            GraphicsDevice.Clear(BgColor);

            // Render the HUD
            spriteBatch.Begin();

            spriteBatch.Draw(DaklutzLogo, LogoPosition, Color.White * 0.25f);
            
            // Render the game entities if we are playing
            if (Paused == false && GameOver == false)
            {
                spriteBatch.DrawString(Font, Points.ToString(), TextPosition - PointOffset, Color.Black * 0.25f);
                Board.Draw(gameTime, spriteBatch, SpriteSheet, Font);
                Worm.Draw(gameTime, spriteBatch, SpriteSheet, Font);
                Food.Draw(gameTime, spriteBatch, SpriteSheet, Font);
            }
            else if (GameOver == true)
            {
                Vector2 GameOverOffset = new Vector2(Font.MeasureString("GAME OVER").Length() / 2, 0);
                spriteBatch.Draw(ColorTexture, new Rectangle(0, 0, BoardWidth * GridSize, BoardHeight * GridSize), Color.DarkBlue * 0.25f);
                spriteBatch.Draw(ColorTexture, new Rectangle(0, (int)TextPosition.Y - 25, BoardWidth * GridSize, 100), Color.DarkBlue * 0.25f);
                spriteBatch.DrawString(Font, "GAME OVER", TextPosition - GameOverOffset, Color.White);
                GameOverOffset = new Vector2(Font.MeasureString("FINAL SCORE").Length() / 2, -25);
                spriteBatch.DrawString(Font, "FINAL SCORE", TextPosition - GameOverOffset, Color.White);
                GameOverOffset = new Vector2(Font.MeasureString(Points.ToString()).Length() / 2, -50);
                spriteBatch.DrawString(Font, Points.ToString(), TextPosition - GameOverOffset, Color.White);

            }
            else
            {
                // Default to paused screen
                Vector2 PausedOffset = new Vector2(Font.MeasureString("PAUSED").Length() / 2, 0);
                spriteBatch.Draw(ColorTexture, new Rectangle(0, 0, BoardWidth * GridSize, BoardHeight * GridSize), Color.DarkBlue * 0.25f);
                spriteBatch.Draw(ColorTexture, new Rectangle(0, (int)TextPosition.Y - 25, BoardWidth * GridSize, 75), Color.DarkBlue * 0.25f);
                spriteBatch.DrawString(Font, "PAUSED", TextPosition - PausedOffset, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
