using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Graphics;

namespace yawn
{
    class Food
    {
        private Vector2 Position;
        private int Value;
        private bool Eaten;
        private Rectangle Cherry, Banana;
        private int GridSize;

        public Food(int gridsize)
        {
            GridSize = gridsize;
            Banana = new Rectangle(2 * gridsize, 1 * gridsize, gridsize, gridsize);
            Cherry = new Rectangle(3 * gridsize, 1 * gridsize, gridsize, gridsize);
            Eaten = true;
            Position = new Vector2(-1, -1);
            Value = 1;
        }

        public int Potency()
        {
            return Value;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D Tile, SpriteFont Font)
        {
            if (Eaten != true)
            {
                spriteBatch.Draw(Tile, new Vector2(Position.X * GridSize, Position.Y * GridSize), Cherry, Color.White);
            }
        }

        public bool Collides(Vector2 position)
        {
            if (Position.Equals(position))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        public void Devoured()
        {
            Value++;
            Eaten = true;
            Position = new Vector2(-1, -1);
        }

        public bool IsEaten()
        {
            return Eaten;
        }

        public void Respawn(Vector2 NewPosition)
        {
            Eaten = false;
            Position = NewPosition;
        }
    }
}
