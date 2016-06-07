using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnaPathfidingLibrary;

namespace SteamGame
{
    public class Train : GameObj
    {
        public float MaxSpeed = 1;
        public float Acceleration = 0.1F;

        public Train(Texture2D tex, Vector2 startPos, Vector2 startVel = new Vector2(), float maxSpeed = 10, float accel = 2)
            : base(tex, startPos, startVel)
        {
            MaxSpeed = maxSpeed; Acceleration = accel;
        }

        public override void Update()
        {
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);
            Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxSpeed, MaxSpeed);
            base.Update();
            if (Destination != null)
            {
                bool flag = Destination.Move(5);
                Position = Destination.GetPosition();
                Velocity = Position - PrPosition;
                if (!flag) Destination = null;
            }
        }

        public override void Draw()
        {
            if (this == Menu.SelTrain)
            {
                Rectangle ret = DBox;
                ret.Inflate(2, 2);
                ret.Offset(2, 2);
                float r = GetR() + Main.Rad(90);
                Main.Draw(Texture, ret, Color.Black, r);
            }
            base.Draw();
        }

        public void SetDestination(int x, int y)
        {
            AbstractSearch s = new AStarSearch(Main.thisObj);
            s.Map = Main.map.nodeMap;
            Point p = Main.GetPos(8, Position.X, Position.Y);
            s.StartNode = GetNode(p.X, p.Y);
            s.EndNode = GetNode(x, y);
            s.TimeLimit = 0.07;
            s.Configuration = SearchConfiguration.Timed;
            s.Orientation = SearchOrientation.Clockwise;
            s.Type = SearchType.EightWay;
            s.OnSearchFinished += new EventHandler<SearchFinishedEventArgs>(S_OnSearchFinished);
            Main.thisObj.Components.Add(s);
            s.Start();
        }

        private MapNode GetNode(int x, int y)
        {
            if (Main.map.nodeMap[x, y].Navigable) return Main.map.nodeMap[x, y];
            for (int i = 1; i <= 3; ++i)
                for (int X = x + i; X > x - i; --X)
                    for (int Y = y + i; Y > y - i; --Y)
                        if (Main.map.nodeMap.Nodes.GetLength(1) > Y && Y >= 0 && Main.map.nodeMap.Nodes.GetLength(0) > X && X >= 0)
                            if (Main.map.nodeMap[X, Y].Navigable) return Main.map.nodeMap[X, Y];
            return Main.map.nodeMap[x, y];
        }
    }
}