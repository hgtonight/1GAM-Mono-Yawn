using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Graphics;

namespace yawn
{
    class YawnWorm
    {
        private List<Vector2> SectionPositions; // Where the worm has been
        private Vector2 HeadPosition; // Where the worm is right now
        private int Length; // how many sections there should be
        private Direction CameFrom; // which direction the worm came from

        public YawnWorm()
        {
            HeadPosition = new Vector2(30, 20);
            SectionPositions = new List<Vector2>();
            Length = 1;
            CameFrom = Direction.WEST;
        }

        public Vector2 Position {
            get {
                return HeadPosition;
            }
        }

        public void Eat(int Food)
        {
            Length += Food;
        }

        public bool Update(GameTime gameTime, List<Direction> Dirs)
        {
            Vector2 TempPosition = new Vector2(0, 0);
            if (Dirs.Count() > 0)
            {
                foreach(Direction Dir in Dirs) {
                    if (Dir.Ordinal() != CameFrom.Ordinal())
                    {
                        TempPosition = Dir.Move(HeadPosition);
                        CameFrom = Dir.Opposite();
                        break;
                    }
                    else
                    {
                        TempPosition = CameFrom.Opposite().Move(HeadPosition);
                    }
                }
            }
            else
            {
                // no input, so go in the opposite direction we came from
                TempPosition = CameFrom.Opposite().Move(HeadPosition);
            }

            // check for collision with self
            if (SectionPositions.Exists(x => x.X == TempPosition.X && x.Y == TempPosition.Y))
            {
                // do something
                return false;
            }
            else {
                // add a section where the head is and move the head
                SectionPositions.Add(HeadPosition);
                HeadPosition = TempPosition;

                // delete sections if we exceeded our length
                while (SectionPositions.Count() > Length)
                {
                    SectionPositions.RemoveAt(0);
                }
                return true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D Tile)
        {
            spriteBatch.Begin();
            
            // Draw each section of the worm
            foreach (Vector2 Section in SectionPositions)
            {
                spriteBatch.Draw(Tile, new Vector2(Section.X * 10.0f, Section.Y * 10.0f), Color.Pink);
            }

            // Draw the head
            spriteBatch.Draw(Tile, new Vector2(HeadPosition.X * 10.0f, HeadPosition.Y * 10.0f), Color.Red);

            spriteBatch.End();
        }

        public bool SectionHere(Vector3 Food)
        {
            if (SectionPositions.Exists(x => x.X == Food.X && x.Y == Food.Y))
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
