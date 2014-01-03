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
        private List<Tuple<int, int>> SectionPositions; // Where the worm has been
        private Tuple<int, int> HeadPosition; // Where the worm is right now
        private int Length; // how many sections there should be
        private Direction CameFrom; // which direction the worm came from

        public YawnWorm()
        {
            HeadPosition = new Tuple<int, int>(30, 20);
            Length = 1;
            CameFrom = Direction.WEST;
        }

        public bool Update(GameTime gameTime, List<Direction> Dirs)
        {
            Tuple<int, int> TempPosition = new Tuple<int,int>(0, 0);
            // no direction input or no valid direction input
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
            if (SectionPositions.Exists(x => x.Item1 == TempPosition.Item1 && x.Item2 == TempPosition.Item2))
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
            foreach (Tuple<int, int> Section in SectionPositions)
            {
                spriteBatch.Draw(Tile, new Vector2(Section.Item1 * 10.0f, Section.Item2 * 10.0f), Color.Pink);
            }

            // Draw the head
            spriteBatch.Draw(Tile, new Vector2(HeadPosition.Item1 * 10.0f, HeadPosition.Item2 * 10.0f), Color.Red);

            spriteBatch.End();
        }
    }
}
