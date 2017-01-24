using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using LeyStoneEngine;
using LeyStoneEngine.Utility;
using LeyStoneEngine.Interface;
using LeyStoneEngine.Input;
using LeyStoneEngine.Guis;

using LeyStoneGame.Guis;

namespace LeyStoneGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : BaseMain
    {
        GraphicsDeviceManager graphics;
        SpriteBatch batch;

        //public World world;

        bool paused;

        public Main() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;

            Content.RootDirectory = "Content";

            assets = new Assets();
            keyboard = new GameKeyboard();
            mouse = new GameMouse();

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(GraphicsDevice.Viewport);
            camera.defaultMoveMode = CameraMoveMode.Smooth;

            currentGui = new GuiMainMenu(new GuiGameHud(), new World(new Vector2(128)));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batch = new SpriteBatch(GraphicsDevice);

            assets.Load(GraphicsDevice, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboard.Update();

            foreach (Timer t in timers)
                t.Count();

            try
            {
                currentGui.Update(this);
            }
            catch(Exception e)
            {
                Logger.Log("currentGui MUST BE SET.", true);
                new Exception(e.Message);
            }
            

            if (!currentGui.stopsWorldInput)
            {
                camera.Update();
                mouse.Update();

                if (keyboard.KeyPressed(Keys.P))
                    paused = !paused;
                    //World.timeScale.scale = World.timeScale.scale == 0 ? 1 : 0;

                if (!paused || keyboard.KeyPressed(Keys.O))
                {
                    world.Update(this);
                }
                camera.PostUpdate(this);
            }

            keyboard.PostUpdate();
            mouse.PostUpdate();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, DrawHelper.primitiveSampler, DepthStencilState.Default, null, null, camera.GetViewMatrix());    //Forced to use immediate because primitives are drawing out of order
            //((BasicEffect)assets.GetEffect("basicEffect")).World = camera.GetViewMatrix();

            if (!currentGui.stopsWorldDraw)
            {
                world.Draw(graphics.GraphicsDevice, batch);
            }
            currentGui.Draw(batch);

            //((BasicEffect)assets.GetEffectContainer("basicEffect").effect).World = camera.GetViewMatrix();

            camera.Draw(this, batch);

            batch.End();

            base.Draw(gameTime);
        }
    }
}
