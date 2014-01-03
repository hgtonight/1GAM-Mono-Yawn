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
        const float InitialSpeed = 1.0f;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        YawnWorm Worm;
        Direction InputDirection;
        int Points;
        bool Paused, GameOver;
        float GameSpeed;
        Texture2D Tile, DaklutzLogo, BlankTexture;
        SpriteFont Font;

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

                if (Worm.Update(gameTime, Dirs) == false)
                {
                    GameOver = true;
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

            // Render the worm
            Worm.Draw(gameTime, spriteBatch, Tile);

            base.Draw(gameTime);
        }
    }
}
