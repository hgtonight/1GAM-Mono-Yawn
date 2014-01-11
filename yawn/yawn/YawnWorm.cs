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
            Length = 1;
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D Tile)
        {
            Rectangle TileSection = MidSection;
            Direction PrevDir = CameFrom.Opposite();
            float Rotation = 0.0f;
            SpriteEffects FlipSection = SpriteEffects.None;

            // need to do this outside the loop so the first section is flipped properly
            if (CameFrom.Degrees() == 90)
            {
                FlipSection = SpriteEffects.FlipVertically;
            }

            spriteBatch.Begin();
            
            // Draw each section of the worm
            /*int Count = SectionPositions.Count;
            Direction PrevDir = CameFrom.Opposite();
            SpriteEffects FlipSection = SpriteEffects.None;
            if (CameFrom.Degrees() == 90)
            {
                FlipSection = SpriteEffects.FlipVertically;
            }*/
            for (int i = SectionPositions.Count - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    if (SectionPositions.Count == 2)
                    {
                        // Check the head position against the last section position
                        if (SectionPositions[0].X == HeadPosition.X || SectionPositions[0].Y == HeadPosition.Y)
                        {
                            TileSection = MidSection;
                        }
                        else
                        {
                            TileSection = TurnSection;
                        }
                    }
                    else if (i < SectionPositions.Count - 1)
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
                        }
                    }
                }
                else
                {
                    // if there is no previous position, print the end section
                    TileSection = EndSection;
                }
                
                // Find the rotation by looking at the previous direction
                Rotation = PrevDir.RotationAngle();

                // Actually print the section with the proper rotation
                spriteBatch.Draw(Tile, new Vector2(SectionPositions[i].X * GridSize + GridSize / 2, SectionPositions[i].Y * GridSize + GridSize / 2), TileSection, Color.Pink, Rotation, new Vector2(GridSize / 2, GridSize / 2), 1.0f, FlipSection, 0.0f);
                
                // Save the current direction for next sections rotation calculation
                if (SectionPositions.Count > 1 && i > 0)
                {
                    if (SectionPositions[i].X < SectionPositions[i - 1].X)
                    {
                        PrevDir = Direction.WEST;
                        FlipSection = SpriteEffects.FlipVertically;
                    }
                    else if (SectionPositions[i].X > SectionPositions[i - 1].X)
                    {
                        PrevDir = Direction.EAST;
                        FlipSection = SpriteEffects.None;
                    }
                    else if (SectionPositions[i].Y < SectionPositions[i - 1].Y)
                    {
                        PrevDir = Direction.NORTH;
                        FlipSection = SpriteEffects.None;
                    }
                    else
                    {
                        PrevDir = Direction.SOUTH;
                        FlipSection = SpriteEffects.None;
                    }
                }
                

                // Determine the rotation based on the previous direction
                /*float Rotation = PrevDir.RotationAngle();
                if (i == 0)
                {
                    spriteBatch.Draw(Tile, new Vector2(SectionPositions[i].X * GridSize + GridSize / 2, SectionPositions[i].Y * GridSize + GridSize / 2), EndSection, Color.Pink, Rotation, new Vector2(GridSize / 2, GridSize / 2), 1.0f, FlipSection, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(Tile, new Vector2(SectionPositions[i].X * GridSize + GridSize / 2, SectionPositions[i].Y * GridSize + GridSize / 2), MidSection, Color.Pink, Rotation, new Vector2(GridSize / 2, GridSize / 2), 1.0f, FlipSection, 0.0f);
                }

                if (SectionPositions.Count > 1 && i > 0)
                {
                    if (SectionPositions[i].X < SectionPositions[i - 1].X)
                    {
                        PrevDir = Direction.WEST;
                        FlipSection = SpriteEffects.FlipVertically;
                    }
                    else if (SectionPositions[i].X > SectionPositions[i - 1].X)
                    {
                        PrevDir = Direction.EAST;
                        FlipSection = SpriteEffects.None;
                    }
                    else if (SectionPositions[i].Y < SectionPositions[i - 1].Y)
                    {
                        PrevDir = Direction.NORTH;
                        FlipSection = SpriteEffects.None;
                    }
                    else
                    {
                        PrevDir = Direction.SOUTH;
                        FlipSection = SpriteEffects.None;
                    }
                }*/
            }

            // Draw the head
            // public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth);
            SpriteEffects Flip = SpriteEffects.None;
            if (CameFrom.Degrees() == 90)
            {
                Flip = SpriteEffects.FlipVertically;
            }
            spriteBatch.Draw(Tile, new Vector2(HeadPosition.X * GridSize + GridSize / 2, HeadPosition.Y * GridSize + GridSize / 2), HeadSection, Color.Red, CameFrom.Opposite().RotationAngle(), new Vector2(GridSize / 2, GridSize / 2), 1.0f, Flip, 0.0f);

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
