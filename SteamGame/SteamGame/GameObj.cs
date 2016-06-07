using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaPathfidingLibrary;

namespace SteamGame
{
    public class GameObj
    {
        public byte Team = 0;
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 PrVelocity, PrPosition;
        public Path Destination = null;

        public Rectangle BBox { get { return new Rectangle((int)(Position.X), (int)(Position.Y), Main.defSize, Main.defSize); } }

        public Rectangle DBox { get { Rectangle r = BBox; r.Offset((int)-Camera.Position.X, (int)-Camera.Position.Y); return r; } }

        public GameObj(Texture2D tex, Vector2 startPos, Vector2 startVel = new Vector2())
        {
            Texture = tex; Position = startPos; Velocity = startVel;
            PrVelocity = startVel; PrPosition = startPos;
        }

        public void S_OnSearchFinished(object sender, SearchFinishedEventArgs e)
        {
            AbstractSearch s = (AbstractSearch)sender;
            if (e.Found && s.Path.Count > 1) Destination = new Path(s.Path);
            Main.thisObj.Components.Remove(s);
        }

        public class Path
        {
            Vector2[] points;
            public Vector2[] Points { get { return points; } }
            float[] pointDistance;
            float totalDistance = 0;
            float position = 0;

            public Path(LinkedList<MapNode> list)
            {
                if (list.Count < 2) throw new Exception("Path requires at least 2 points!");
                points = new Vector2[list.Count];
                var num = list.GetEnumerator();
                for (int n = 0; num.MoveNext(); ++n)
                    points[n] = new Vector2(num.Current.Position.X * 8 + 4, num.Current.Position.Y * 8 + 4);
                pointDistance = new float[points.Length];
                pointDistance[0] = 0;
                for (int i = 1; i < points.Length; i++)
                {
                    float dist = Vector2.Distance(points[i - 1], points[i]);
                    totalDistance += dist;
                    pointDistance[i] = totalDistance;
                }
                return;
            }

            public bool AfterPoint(int index)
            {
                return pointDistance[index] < position;
            }

            public Vector2 GetPosition()
            {
                int i;
                float postmp = 0;
                for (i = 0; i < pointDistance.Length - 1; i++)
                {
                    if (position >= pointDistance[i + 1]) continue;
                    postmp = (position - pointDistance[i]) / (pointDistance[i + 1] - pointDistance[i]);
                    break;
                }
                if (i < pointDistance.Length - 1) return Vector2.Lerp(points[i], points[i + 1], postmp);
                else return points[i];
            }

            public bool Move(float speed)
            {
                position += speed;
                return position < totalDistance;
            }
        }

        public virtual void Draw()
        {
            Rectangle ret = DBox;
            float r = GetR() + Main.Rad(90);
            if (Destination != null && Menu.SelTrain == this)
                for (int i = 0; i < Destination.Points.Length; ++i)
                    if (!Destination.AfterPoint(i)) Main.Draw(Main.GetTex("Track"), new Rectangle((int)Destination.Points[i].X - (int)Camera.Position.X, (int)Destination.Points[i].Y - (int)Camera.Position.Y, 8, 8), Color.White, 0, SpriteEffects.None, 1);
            Main.Draw(Texture, ret, Color.White, r);
        }

        internal float GetR()
        {
            return Velocity == Vector2.Zero ? (float)Math.Atan2(PrVelocity.X, -PrVelocity.Y) : (float)Math.Atan2(Velocity.X, -Velocity.Y);
        }

        public virtual void Update()
        {
            PrVelocity = Velocity;
            PrPosition = Position;
        }
    }
}
