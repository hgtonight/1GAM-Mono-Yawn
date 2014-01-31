using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Graphics;

namespace yawn
{
    class Board
    {
        private List<Vector2> BlockPositions; // Where Board is
        private int Width, Height;
        private int GridSize;
        private Rectangle Block;

        public Board(int width, int height, int TileSize, int random = 0)
        {
            BlockPositions = new List<Vector2>();
            Width = width;
            Height = height;
            GridSize = TileSize;
            Block = new Rectangle(0 * GridSize, 1 * GridSize, GridSize, GridSize);

            if (random == 0)
            {
                GenerateStandardLevel();
            }
            else
            {
                GenerateRandomLevel(random);
            }
        }

        private void GenerateRandomLevel(int Num)
        {
            int x, y;
            Random r = new Random();
            for (int i = 0; i <= Num; i++)
            {
                x = r.Next(Width);
                y = r.Next(Height);
                BlockPositions.Add(new Vector2(x, y));
            }
            BlockPositions = BlockPositions.Distinct().ToList();
        }

        private void GenerateStandardLevel() {
            for (int i = 0; i <= Width; i++)
            {
                for (int j = 0; j <= Height; j++)
                {
                    if (i == 0 ||
                        i == Width ||
                        j == 0 ||
                        j == Height)
                    {
                        BlockPositions.Add(new Vector2(i, j));
                    }
                }
            }
        }

        public bool Update(GameTime gameTime, List<Direction> Dirs)
        {
           // Potentially allow the board to move
            return true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D Tile, SpriteFont Font)
        {
            for (int i = BlockPositions.Count - 1; i >= 0; i--)
            {
                // Actually print the section with the proper rotation
                spriteBatch.Draw(Tile, new Vector2(BlockPositions[i].X * GridSize, BlockPositions[i].Y * GridSize), Block, Color.White);
            }
        }

        public bool Collides(Vector2 Position)
        {
            if (BlockPositions.Exists(x => x.X == Position.X && x.Y == Position.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool OutOfBounds(Vector2 vector2)
        {
            if (vector2.X < 0 || vector2.X > Width || vector2.Y < 0 || vector2.Y > Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
