using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SteamGame
{
    public static class Menu
    {
        public static byte SelType = 0;
        public static Train SelTrain = null;
        public static byte Screen = 0;
        private static byte TextTime = 0;
        private static string Text = "";
        public static short W = 40, H = 30;

        public static void SetText(string text)
        {
            Text = text;
            TextTime = 255;
        }

        public static void DrawHotbar()
        {
            Texture2D slot = Main.GetTex("Slot");
            Main.Draw(slot, new Rectangle(Main.ScreenWidth / 2, Main.ScreenHeight - 24, Main.ScreenWidth, 48), Color.White);
            for (byte i = 0; i < 13; ++i)
            {
                Rectangle rect = new Rectangle(i * 50 + 98, Main.ScreenHeight - 24, 32, 32);
                switch (Screen)
                {
                    case 0:
                        Main.Draw(slot, rect, SelType + 1 == i ? Color.Silver : Color.White, 0, SpriteEffects.None, 0);
                        rect.Inflate(-4, -4); rect.Offset(-4, -4);
                        if (i > 1) MyMap.DrawTile(rect, (sbyte)(i - 1));
                        else if (i == 1) Main.Draw(Main.GetTex("Erase"), rect, Color.White);
                        else Main.Draw(Main.GetTex("Arrow"), rect, Color.White);
                        break;
                    case 1:
                        Main.Draw(slot, rect, Color.White, 0, SpriteEffects.None, 0);
                        rect.Inflate(-4, -4); rect.Offset(-4, -4);
                        switch (i)
                        {
                            case 0:
                                Main.Draw(Main.GetTex("Arrow"), rect, Color.White);
                                break;
                            case 1:
                                Main.Draw(Main.GetTex("Erase"), rect, Color.Gray);
                                break;
                            case 2:
                                Main.Draw(Main.GetTex("Load"), rect, Color.White);
                                break;
                            case 3:
                                Main.Draw(Main.GetTex("Save"), rect, Color.White);
                                break;
                            case 4:
                                Main.Draw(Main.GetTex("Erase"), rect, Color.LightGray);
                                Main.Draw(Main.GetTex("Arrow"), rect, Color.White, Main.Rad(180));
                                break;
                            case 5:
                                Main.Draw(Main.GetTex("New"), rect, Color.White);
                                break;
                        }
                        break;
                    case 2:
                        if (i == 0 || i == 4 || i == 5 || i > 8 && i < 12) Main.Draw(slot, rect, Color.White, 0, SpriteEffects.None, 0);
                        rect.Inflate(-4, -4); rect.Offset(-4, -4);
                        switch (i)
                        {
                            case 0:
                                Main.Draw(Main.GetTex("Arrow"), rect, Color.White);
                                break;
                            case 1:
                                DrawText("Width: " + W, Color.Black, Color.White, 1F, 0F, new Vector2(i * 50 + 86, Main.ScreenHeight - 32));
                                break;
                            case 4:
                                break;
                            case 5:
                                break;
                            case 6:
                                DrawText("Height: " + H, Color.Black, Color.White, 1F, 0F, new Vector2(i * 50 + 86, Main.ScreenHeight - 32));
                                break;
                            case 9:
                                break;
                            case 10:
                                break;
                            case 11:
                                Main.Draw(Main.GetTex("New"), rect, Color.White);
                                break;
                        }
                        break;
                }
            }
            if (TextTime > 0)
            {
                DrawText(Text, Color.Black, Color.White, 1.2F, 0, new Vector2(24, 24));
                --TextTime;
            }
        }

        public static void DrawText(string text, Color backColor, Color frontColor, float scale, float rotation, Vector2 position)
        {
            Main.spriteBatch.DrawString(Main.font, text, position + new Vector2(1 * scale, 1 * scale), backColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 1f);
            Main.spriteBatch.DrawString(Main.font, text, position + new Vector2(-1 * scale, -1 * scale), backColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 1f);
            Main.spriteBatch.DrawString(Main.font, text, position + new Vector2(-1 * scale, 1 * scale), backColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 1f);
            Main.spriteBatch.DrawString(Main.font, text, position + new Vector2(1 * scale, -1 * scale), backColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 1f);
            Main.spriteBatch.DrawString(Main.font, text, position, frontColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }
    }
}
