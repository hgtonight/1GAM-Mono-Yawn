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
        private int GridSize;
        private Rectangle HeadSection, MidSection, EndSection, TurnSection;

        public YawnWorm(int TileSize)
        {
            HeadPosition = new Vector2(5, 5);
            SectionPositions = new List<Vector2>();
            Length = 10;
            CameFrom = Direction.WEST;
            GridSize = TileSize;
            TurnSection = new Rectangle(0 * TileSize, 0, TileSize, TileSize);
            MidSection = new Rectangle(1 * TileSize, 0, TileSize, TileSize);
            EndSection = new Rectangle(2 * TileSize, 0, TileSize, TileSize);
            HeadSection = new Rectangle(3 * TileSize, 0, TileSize, TileSize);
        }

        public Direction Facing
        {
            get
            {
                return CameFrom.Opposite();
            }
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D Tile, SpriteFont Font)
        {
            SpriteEffects FlipSection;
            Rectangle TileSection = MidSection;
            Direction PrevDir = CameFrom.Opposite();
            float Rotation = 0.0f;
            
            spriteBatch.Begin();
            for (int i = SectionPositions.Count - 1; i >= 0; i--)
            {
                // Assume we don't need to flip the tile
                FlipSection = SpriteEffects.None;

                // Find the rotation by looking at the previous direction
                Rotation = PrevDir.RotationAngle();

                if (i == 0)
                {
                    // if there is no previous position, print the end section
                    TileSection = EndSection;
                }
                else {
                    
                    if (i < SectionPositions.Count - 1)
                    {

                        if ((SectionPositions[i - 1].X == SectionPositions[i + 1].X) || (SectionPositions[i - 1].Y == SectionPositions[i + 1].Y))
                        {
                            // if the previous position and the next position share an axis print a midsection
                            TileSection = MidSection;
                        }
                        else
                        {
                            // If the previous position and the next position are kitty corner, print a corner
                            TileSection = TurnSection;

                            // Determine if the tile needs to be flipped
                            Rotation += FindExtraRotation(SectionPositions[i - 1], SectionPositions[i], SectionPositions[i + 1]);
                        }
                    }
                    else
                    {
                        // Check the head position against the last section position
                        if (SectionPositions[i - 1].X == HeadPosition.X || SectionPositions[i - 1].Y == HeadPosition.Y)
                        {
                            TileSection = MidSection;
                        }
                        else
                        {
                            TileSection = TurnSection;
                            Rotation += FindExtraRotation(SectionPositions[i - 1], SectionPositions[i], HeadPosition);
                        }
                    }
                }

                // Actually print the section with the proper rotation
                spriteBatch.Draw(Tile, new Vector2(SectionPositions[i].X * GridSize + GridSize / 2, SectionPositions[i].Y * GridSize + GridSize / 2), TileSection, Color.Pink, Rotation, new Vector2(GridSize / 2, GridSize / 2), 1.0f, FlipSection, 0.0f);
                //spriteBatch.DrawString(Font, (i % 10).ToString(), new Vector2(SectionPositions[i].X * GridSize, SectionPositions[i].Y * GridSize), Color.Black);
                
                // Save the current direction for next sections rotation calculation
                if (SectionPositions.Count > 1 && i > 0)
                {
                    if (SectionPositions[i].X < SectionPositions[i - 1].X)
                    {
                        PrevDir = Direction.WEST;
                    }
                    else if (SectionPositions[i].X > SectionPositions[i - 1].X)
                    {
                        PrevDir = Direction.EAST;
                    }
                    else if (SectionPositions[i].Y < SectionPositions[i - 1].Y)
                    {
                        PrevDir = Direction.NORTH;
                    }
                    else
                    {
                        PrevDir = Direction.SOUTH;
                    }
                }
                
            }

            // Draw the head
            // public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth);
            SpriteEffects FlipHead = SpriteEffects.None;
            if (CameFrom.Degrees() == 90)
            {
                FlipHead = SpriteEffects.FlipVertically;
            }
            spriteBatch.Draw(Tile, new Vector2(HeadPosition.X * GridSize + GridSize / 2, HeadPosition.Y * GridSize + GridSize / 2), HeadSection, Color.Red, CameFrom.Opposite().RotationAngle(), new Vector2(GridSize / 2, GridSize / 2), 1.0f, FlipHead, 0.0f);

            spriteBatch.End();
        }

        private float FindExtraRotation(Vector2 PreviousPos, Vector2 CurrPos, Vector2 NextPos)
        {
            // Takes 2 positions that are connected via one point and determine if the turn sprite should be flipped
            if (PreviousPos.X == NextPos.X || PreviousPos.Y == NextPos.Y)
            {
                // This should never actually happen since this function is only called on turn sections
                return 0.0f;
            }
            else if (PreviousPos.X == CurrPos.X)
            {
                // Up or down?
                if (CurrPos.Y < PreviousPos.Y)
                {
                    // Left or right?
                    if (CurrPos.X < NextPos.X)
                    {
                        // Moving from bottom to the right
                        return 0.0f;
                    }
                    else
                    {
                        // Moving from bottom to the left
                        return -90.0f * (float)Math.PI / 180.0f;
                    }
                }
                else
                {
                    // Left or right?
                    if (CurrPos.X < NextPos.X)
                    {
                        // Moving from top to the right
                        return -90.0f * (float)Math.PI / 180.0f;
                    }
                    else
                    {
                        // Moving from top to the left
                        return 0.0f;
                    }
                }
            }
            else if(PreviousPos.Y == CurrPos.Y)
            {
                // Left or right?
                if (CurrPos.X < PreviousPos.X)
                {
                    // Up or down?
                    if (CurrPos.Y < NextPos.Y)
                    {
                        // Moving from right to bottom
                        return -90.0f * (float)Math.PI / 180.0f;
                    }
                    else
                    {
                        // Moving from right to top
                        return 0.0f;
                    }
                }
                else
                {
                    // Up or down?
                    if (CurrPos.Y < NextPos.Y)
                    {
                        // Moving from left to bottom
                        return 0.0f;
                    }
                    else
                    {
                        // Moving from left to top
                        return -90.0f * (float)Math.PI / 180.0f;
                    }
                }
            }

            // This shouldn't happen
            return 0.0f;
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
