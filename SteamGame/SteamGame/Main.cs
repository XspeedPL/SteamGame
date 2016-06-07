using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XnaPathfidingLibrary;

namespace SteamGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        internal static SpriteBatch spriteBatch;
        private KeyboardState keyState, prKeyState;
        private MouseState mouState, prMouState;
        public const float R90 = 1.570796327F;
        private Vector2 motion;
        private static GraphicsDevice graphDevice = null;
        internal static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public const int defSize = 24;
        public static MyMap map = new MyMap();
        public static List<GameObj> objects = new List<GameObj>();
        public static SpriteFont font = null;
        public static Main thisObj { get; private set; }
        public static Train Player;
        public static int MyTeam = 1;

        public static int ScreenWidth { get { return graphDevice.Viewport.Width; } }
        public static int ScreenHeight { get { return graphDevice.Viewport.Height; } }

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            thisObj = this;
        }

        public static Texture2D GetTex(string name)
        {
            if (textures.ContainsKey(name)) return textures[name];
            else return null;
        }

        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            if (bFullScreen == false)
            {
                if (iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width && iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
            }
            return false;
        }

        protected override void Initialize()
        {
            graphDevice = GraphicsDevice;
            InitGraphicsMode(800, 600, false);
            IsMouseVisible = true;
            base.Initialize();
            Train train = new Train(GetTex("Train"), new Vector2(10, 32), Vector2.Zero);
            train.Team = 1;
            objects.Add(train);
            Player = train;
        }

        public static Vector2 HalfSize(Texture2D tex) { return new Vector2(tex.Width / 2, tex.Height / 2); }

        public static float Rad(int degrees)
        {
            return MathHelper.ToRadians(degrees);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            textures.Add("RailStr", Content.Load<Texture2D>("Rails"));
            textures.Add("RailCor", Content.Load<Texture2D>("Rails2"));
            textures.Add("RailTCr", Content.Load<Texture2D>("Rails3"));
            textures.Add("RailCrs", Content.Load<Texture2D>("Rails4"));
            textures.Add("RailEnd", Content.Load<Texture2D>("Rails5"));
            textures.Add("Train", Content.Load<Texture2D>("Train"));
            textures.Add("Track", Content.Load<Texture2D>("Track"));
            textures.Add("Slot", Content.Load<Texture2D>("Slot"));
            textures.Add("Erase", Content.Load<Texture2D>("Erase"));
            textures.Add("Arrow", Content.Load<Texture2D>("Arrow"));
            textures.Add("Load", Content.Load<Texture2D>("Load"));
            textures.Add("Save", Content.Load<Texture2D>("Save"));
            textures.Add("New", Content.Load<Texture2D>("New"));
            font = Content.Load<SpriteFont>("Font");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                prKeyState = keyState;
                prMouState = mouState;
                keyState = Keyboard.GetState();
                mouState = Mouse.GetState();

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) Exit();
                motion = Vector2.Zero;

                if (keyState.IsKeyDown(Keys.Up)) ScrollUp();
                if (keyState.IsKeyDown(Keys.Left)) ScrollLeft();
                if (keyState.IsKeyDown(Keys.Down)) ScrollDown();
                if (keyState.IsKeyDown(Keys.Right)) ScrollRight();

                if (keyState.IsKeyDown(Keys.S) && prKeyState.IsKeyUp(Keys.S) && Menu.SelTrain != null) Menu.SelTrain.Destination = null;

                if (mouState.X >= 0 && mouState.Y >= 0 && mouState.X < Main.ScreenWidth && mouState.Y < Main.ScreenHeight)
                {
                    if (mouState.Y < Main.ScreenHeight - 48)
                    {
                        if (mouState.RightButton == ButtonState.Pressed && prMouState.RightButton == ButtonState.Released && Menu.SelTrain != null)
                        {
                            Point p = GetPos(8, mouState.X + Camera.Position.X, mouState.Y + Camera.Position.Y);
                            if (GetTileType(p.X / 3, p.Y / 3) > -1) Menu.SelTrain.SetDestination(p.X, p.Y);
                        }
                        if (mouState.LeftButton == ButtonState.Pressed)
                        {
                            if (Menu.Screen == 0)
                            {
                                Point p = GetPos(24, mouState.X + Camera.Position.X, mouState.Y + Camera.Position.Y);
                                if (GetTileType(p.X, p.Y) > -1 && GetTileType(p.X, p.Y) != Menu.SelType) map.SetMapPiece(p.X, p.Y, (MyMap.Turn)Menu.SelType);
                            }
                            else if (Menu.Screen == 1)
                            {
                                Menu.SelTrain = null;
                                foreach (Train t in objects)
                                    if (t.Team == MyTeam && Offset(t.DBox, -Main.defSize / 2, -Main.defSize / 2).Contains(mouState.X, mouState.Y))
                                    {
                                        Menu.SelTrain = t;
                                        break;
                                    }
                            }
                        }
                    }
                    else if (mouState.LeftButton == ButtonState.Pressed && prMouState.LeftButton == ButtonState.Released)
                    {
                        for (byte i = 0; i < 13; ++i)
                            if (new Rectangle(i * 50 + 82, Main.ScreenHeight - 32, 32, 32).Contains(mouState.X, mouState.Y))
                            {
                                if (Menu.Screen == 0)
                                {
                                    if (i > 0) Menu.SelType = (byte)(i - 1);
                                    else Menu.Screen = 1;
                                }
                                else if (Menu.Screen == 1)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Menu.Screen = 0;
                                            break;
                                        case 1:
                                            map.Generate(map.Width, map.Height);
                                            Menu.SetText("Map cleared!");
                                            break;
                                        case 2:
                                            map.LoadMap("Map");
                                            Menu.SetText("Map loaded!");
                                            break;
                                        case 3:
                                            map.SaveMap("Map");
                                            Menu.SetText("Map saved!");
                                            break;
                                        case 4:
                                            map.Randomize();
                                            Menu.SetText("Map randomized!");
                                            break;
                                        case 5:
                                            Menu.W = (short)map.Width;
                                            Menu.H = (short)map.Height;
                                            Menu.Screen = 2;
                                            break;
                                    }
                                }
                                else if (Menu.Screen == 2)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Menu.Screen = 1;
                                            break;
                                        case 4:
                                            --Menu.W;
                                            break;
                                        case 5:
                                            ++Menu.W;
                                            break;
                                        case 9:
                                            --Menu.H;
                                            break;
                                        case 10:
                                            ++Menu.H;
                                            break;
                                        case 11:
                                            Menu.Screen = 1;
                                            map.Generate(Menu.W, Menu.H);
                                            Menu.SetText("New map created!");
                                            break;
                                    }
                                }
                                break;
                            }
                    }
                }

                if (motion != Vector2.Zero)
                {
                    motion.Normalize();
                    Camera.Position += motion * Camera.Speed;
                }

                foreach (GameObj obj in objects) obj.Update();
                base.Update(gameTime);
            }
        }

        public sbyte GetTileType(int x, int y)
        {
            try { return map.map[x, y]; }
            catch { return -1; }
        }

        public static Point GetPos(int mult, float x, float y)
        {
            return new Point((int)Math.Floor(x / mult), (int)Math.Floor(y / mult));
        }

        public static Rectangle Offset(Rectangle src, int x, int y)
        {
            src.Offset(x, y);
            return src;
        }

        public static void Draw(Texture2D tex, Rectangle rect, Color color, float rotation = 0, SpriteEffects effects = SpriteEffects.None, float layer = 0)
        {
            Main.spriteBatch.Draw(tex, rect, null, color, rotation, new Vector2(tex.Width / 2F, tex.Height / 2F), effects, layer);
        }

        private void ScrollUp() { motion.Y = -1; }

        private void ScrollRight() { motion.X = 1; }

        private void ScrollDown() { motion.Y = 1; }

        private void ScrollLeft() { motion.X = -1; }

        private Point VectorToCell(Vector2 vector)
        {
            return new Point((int)(vector.X / defSize), (int)(vector.Y / defSize));
        }

        private Vector2 ViewPortVector()
        {
            return new Vector2(ScreenWidth + defSize, ScreenHeight + defSize);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);
            spriteBatch.Begin();
            map.Draw(Camera.Position);
            foreach (GameObj obj in objects) obj.Draw();
            Menu.DrawHotbar();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}