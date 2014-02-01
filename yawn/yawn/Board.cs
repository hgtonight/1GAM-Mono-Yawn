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

            switch(random)
            {
                case 0:
                    GenerateStandardLevel();
                    break;
                case -1:
                    GenerateBlockyLevel();
                    break;
                case -2:
                    GenerateWrappedLevel();
                    break;
                default:
                    GenerateRandomLevel(random);
                    break;
            }
        }

        private void GenerateWrappedLevel()
        {
            int j = 0;
            for (int i = 0; i < Width; i++)
            {
                j++;
                    if(j % 2 == 1 || i == Width - 1)
                    {
                        BlockPositions.Add(new Vector2(i, 0));
                        BlockPositions.Add(new Vector2(i, Height - 1));
                    }
            }
            
            j = 0;
            for (int i = 0; i < Height; i++)
            {
                j++;
                if (j % 2 == 1 || j == Height - 1)
                {
                    BlockPositions.Add(new Vector2(0, i));
                    BlockPositions.Add(new Vector2(Width - 1, i));
                }
            }
        }

        private void GenerateBlockyLevel()
        {
            // This is a level that has a block every other position
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        BlockPositions.Add(new Vector2(i, j));
                    }
                }
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
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i == 0 ||
                        i == Width - 1 ||
                        j == 0 ||
                        j == Height - 1)
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
                spriteBatch.Draw(Tile, new Vector2(BlockPositions[i].X * GridSize, BlockPositions[i].Y * GridSize), Block, new Color(68, 34, 9));
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
