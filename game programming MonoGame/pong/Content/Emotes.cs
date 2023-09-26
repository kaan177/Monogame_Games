﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace pong.Content
{
    internal class Emotes
    {
        Texture2D hihihihahTexture;
        SoundEffect hihihahSound;
        bool draw = false;
        Vector2 drawLocation;
        KeyboardState keyboard, lastKeyBoardState;
        float drawTime;
        float drawDuration = 1.5f;


        public Emotes(ContentManager Content)
        {
            hihihahSound = Content.Load<SoundEffect>("hihihahSound");
            hihihihahTexture = Content.Load<Texture2D>("hihihihahTexture");
        }            
        public void Draw(SpriteBatch spriteBatch)
        {
            if(draw)
                if (drawLocation.X == 0)
                    spriteBatch.Draw(hihihihahTexture, drawLocation, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.FlipHorizontally, 0f);
                else
                    spriteBatch.Draw(hihihihahTexture, drawLocation, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
        public void HandleInput(GameTime gametime)
        {
            lastKeyBoardState = keyboard;
            keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.E) && lastKeyBoardState.IsKeyUp(Keys.E)) 
            { 
                draw = true;
                drawTime = (float)gametime.TotalGameTime.TotalSeconds + drawDuration;
                drawLocation = Vector2.Zero;
                hihihahSound.Play();
            }
            else if(keyboard.IsKeyDown(Keys.Home) && lastKeyBoardState.IsKeyUp(Keys.Home))
            {
                draw = true;
                drawTime = (float)gametime.TotalGameTime.TotalSeconds + drawDuration;
                drawLocation = new Vector2(Pong.screenSize.X - hihihihahTexture.Width / 2, 0);
                hihihahSound.Play();
            }
            else if (gametime.TotalGameTime.TotalSeconds > drawTime)
            {
                draw = false;
            }
        }

    }
}