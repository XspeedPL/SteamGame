using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaPathfidingLibrary;
using System.IO;

namespace SteamGame
{
    public class MyMap
    {
        private static Random rand;
        public Map nodeMap;
        public sbyte[,] map;

        public enum Turn : sbyte
        {
            None = 0,
            EW = 1,
            NS = 2,
            SW = 3,
            NW = 4,
            NE = 5,
            SE = 6,
            NEW = 7,
            NSE = 8,
            SEW = 9,
            NSW = 10,
            NSEW = 11,
            EndN = 12,
            EndS = 13,
            EndE = 14,
            EndW = 15
        }

        internal MyMap(int seed)
        {
            rand = new Random(seed);
            Generate(40, 30);
        }

        internal MyMap()
        {
            rand = new Random();
            Generate(40, 30);
        }

        public void SaveMap(string name)
        {
            List<byte> buff = new List<byte>();
            buff.AddRange(BitConverter.GetBytes(Width));
            buff.AddRange(BitConverter.GetBytes(Height));
            for (int x = 0; x < Width; ++x)
                for (int y = 0; y < Height; ++y)
                    buff.Add((byte)map[x, y]);
            File.WriteAllBytes(name + ".stm", buff.ToArray());
        }

        public void LoadMap(string name)
        {
            if (File.Exists(name + ".stm"))
            {
                byte[] buff = File.ReadAllBytes(name + ".stm");
                int i = 0;
                int w = BitConverter.ToInt32(buff, i); i += 4;
                int h = BitConverter.ToInt32(buff, i); i += 4;
                map = new sbyte[w, h];
                nodeMap = new Map(w * 3, h * 3);
                for (int x = 0; x < w; ++x)
                    for (int y = 0; y < h; ++y)
                        SetMapPiece(x, y, (Turn)buff[i++]);
            }
        }

        public void SetMapPiece(int X, int Y, Turn side)
        {
            map[X, Y] = (sbyte)side;
            int x = X * 3, y = Y * 3;
            SetUnNav(x, y);
            switch (side)
            {
                case Turn.EW:
                    nodeMap[x, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 1].Navigable = true;
                    nodeMap[x + 2, y + 1].Navigable = true;
                    break;
                case Turn.NS:
                    nodeMap[x + 1, y].Navigable = true;
                    nodeMap[x + 1, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 2].Navigable = true;
                    break;
                case Turn.SW:
                    nodeMap[x, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 2].Navigable = true;
                    break;
                case Turn.NW:
                    nodeMap[x, y + 1].Navigable = true;
                    nodeMap[x + 1, y].Navigable = true;
                    break;
                case Turn.NE:
                    nodeMap[x + 2, y + 1].Navigable = true;
                    nodeMap[x + 1, y].Navigable = true;
                    break;
                case Turn.SE:
                    nodeMap[x + 2, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 2].Navigable = true;
                    break;
                case Turn.NEW:
                    nodeMap[x + 1, y + 1].Navigable = true;
                    nodeMap[x, y + 1].Navigable = true;
                    nodeMap[x + 2, y + 1].Navigable = true;
                    nodeMap[x + 1, y].Navigable = true;
                    break;
                case Turn.NSEW:
                    nodeMap[x, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 1].Navigable = true;
                    nodeMap[x + 2, y + 1].Navigable = true;
                    nodeMap[x + 1, y].Navigable = true;
                    nodeMap[x + 1, y + 2].Navigable = true;
                    break;
                case Turn.SEW:
                    nodeMap[x, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 1].Navigable = true;
                    nodeMap[x + 2, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 2].Navigable = true;
                    break;
                case Turn.NSE:
                    nodeMap[x + 1, y + 1].Navigable = true;
                    nodeMap[x + 2, y + 1].Navigable = true;
                    nodeMap[x + 1, y].Navigable = true;
                    nodeMap[x + 1, y + 2].Navigable = true;
                    break;
                case Turn.NSW:
                    nodeMap[x, y + 1].Navigable = true;
                    nodeMap[x + 1, y + 1].Navigable = true;
                    nodeMap[x + 1, y].Navigable = true;
                    nodeMap[x + 1, y + 2].Navigable = true;
                    break;
                case Turn.EndN:
                    nodeMap[x + 1, y].SetNav();
                    break;
                case Turn.EndS:
                    nodeMap[x + 1, y + 2].SetNav();
                    break;
                case Turn.EndE:
                    nodeMap[x, y + 1].SetNav();
                    break;
                case Turn.EndW:
                    nodeMap[x + 2, y + 1].SetNav();
                    break;
            }
        }

        public void Randomize()
        {
            Maze m = new Maze(Width, Height);
            m.Generate();
            for (int x = 0; x < Width; ++x)
                for (int y = 0; y < Height; ++y)
                {
                    Cell c = m.Cells[x, y];
                    if (!c.Walls[0] && !c.Walls[1] && c.Walls[2] && c.Walls[3])
                        SetMapPiece(x, y, Turn.NW);
                    else if (!c.Walls[0] && c.Walls[1] && !c.Walls[2] && c.Walls[3])
                        SetMapPiece(x, y, Turn.NS);
                    else if (!c.Walls[0] && c.Walls[1] && c.Walls[2] && !c.Walls[3])
                        SetMapPiece(x, y, Turn.NE);
                    else if (c.Walls[0] && !c.Walls[1] && !c.Walls[2] && c.Walls[3])
                        SetMapPiece(x, y, Turn.SW);
                    else if (c.Walls[0] && !c.Walls[1] && c.Walls[2] && !c.Walls[3])
                        SetMapPiece(x, y, Turn.EW);
                    else if (c.Walls[0] && c.Walls[1] && !c.Walls[2] && !c.Walls[3])
                        SetMapPiece(x, y, Turn.SE);
                    else if (!c.Walls[0] && !c.Walls[1] && !c.Walls[2] && c.Walls[3])
                        SetMapPiece(x, y, Turn.NSW);
                    else if (!c.Walls[0] && !c.Walls[1] && c.Walls[2] && !c.Walls[3])
                        SetMapPiece(x, y, Turn.NEW);
                    else if (!c.Walls[0] && c.Walls[1] && !c.Walls[2] && !c.Walls[3])
                        SetMapPiece(x, y, Turn.NSE);
                    else if (c.Walls[0] && !c.Walls[1] && !c.Walls[2] && !c.Walls[3])
                        SetMapPiece(x, y, Turn.SEW);
                    else if (!c.Walls[0] && !c.Walls[1] && !c.Walls[2] && !c.Walls[3])
                        SetMapPiece(x, y, Turn.NSEW);
                    else if (c.Walls[0] && !c.Walls[0] && c.Walls[0] && c.Walls[0])
                        SetMapPiece(x, y, Turn.EndN);
                    else if (c.Walls[0] && c.Walls[0] && c.Walls[0] && !c.Walls[0])
                        SetMapPiece(x, y, Turn.EndS);
                    else if (c.Walls[0] && c.Walls[0] && !c.Walls[0] && c.Walls[0])
                        SetMapPiece(x, y, Turn.EndE);
                    else if (!c.Walls[0] && c.Walls[0] && c.Walls[0] && c.Walls[0])
                        SetMapPiece(x, y, Turn.EndW);
                }
        }

        public void Generate(int width, int height)
        {
            map = new sbyte[width, height];
            nodeMap = new Map(width * 3, height * 3);
            for (int x = 1; x < Width - 1; ++x)
            {
                SetMapPiece(x, 0, Turn.EW);
                SetMapPiece(x, Height - 1, Turn.EW);
            }
            for (int y = 1; y < Height - 1; ++y)
            {
                SetMapPiece(0, y, Turn.NS);
                SetMapPiece(Width - 1, y, Turn.NS);
            }
            SetMapPiece(0, 0, Turn.SE);
            SetMapPiece(0, Height - 1, Turn.NE);
            SetMapPiece(Width - 1, 0, Turn.SW);
            SetMapPiece(Width - 1, Height - 1, Turn.NW);
        }

        public int Width { get { return map.GetLength(0); } }
        public int Height { get { return map.GetLength(1); } }

        private void SetUnNav(int X, int Y)
        {
            for (int x = X; x < X + 3; ++x)
                for (int y = Y; y < Y + 3; ++y)
                    nodeMap[x, y].ResetNav();
        }

        internal void Draw(Vector2 cam)
        {
            for (int x = 0; x < Width; ++x)
                for (int y = 0; y < Height; ++y)
                    DrawTile(new Rectangle(GetX(x, cam), GetY(y, cam), Main.defSize, Main.defSize), map[x, y]);
        }

        public static void DrawTile(Rectangle rect, sbyte tile)
        {
            Texture2D tex;
            float r = 0;
            SpriteEffects eff = SpriteEffects.None;
            switch ((Turn)tile)
            {
                case Turn.NS:
                    tex = Main.GetTex("RailStr");
                    break;
                case Turn.EW:
                    tex = Main.GetTex("RailStr");
                    r = Main.Rad(90);
                    break;
                case Turn.SW:
                    tex = Main.GetTex("RailCor");
                    r = Main.Rad(90);
                    break;
                case Turn.SE:
                    tex = Main.GetTex("RailCor");
                    break;
                case Turn.NW:
                    tex = Main.GetTex("RailCor");
                    r = Main.Rad(180);
                    break;
                case Turn.NE:
                    tex = Main.GetTex("RailCor");
                    eff = SpriteEffects.FlipVertically;
                    break;
                case Turn.SEW:
                    tex = Main.GetTex("RailTCr");
                    break;
                case Turn.NSE:
                    tex = Main.GetTex("RailTCr");
                    r = Main.Rad(270);
                    break;
                case Turn.NEW:
                    tex = Main.GetTex("RailTCr");
                    eff = SpriteEffects.FlipVertically;
                    break;
                case Turn.NSW:
                    tex = Main.GetTex("RailTCr");
                    r = Main.Rad(90);
                    break;
                case Turn.NSEW:
                    tex = Main.GetTex("RailCrs");
                    break;
                case Turn.EndN:
                    tex = Main.GetTex("RailEnd");
                    break;
                case Turn.EndS:
                    tex = Main.GetTex("RailEnd");
                    eff = SpriteEffects.FlipVertically;
                    break;
                case Turn.EndE:
                    tex = Main.GetTex("RailEnd");
                    r = Main.Rad(90);
                    break;
                case Turn.EndW:
                    tex = Main.GetTex("RailEnd");
                    r = Main.Rad(90);
                    eff = SpriteEffects.FlipVertically;
                    break;
                default:
                    return;
            }
            Main.Draw(tex, rect, Color.White, r, eff, 1);
        }

        private int GetX(int x, Vector2 cam)
        {
            return (int)((x + 0.5F) * Main.defSize -(int)cam.X);
        }

        private int GetY(int y, Vector2 cam)
        {
            return (int)((y + 0.5F) * Main.defSize -(int)cam.Y);
        }
    }
}