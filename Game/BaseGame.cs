// Author: Ben
// Project: XnaTetris
// Path: C:\code\Xna\XnaTetris\Game
// Creation date: 27.01.2008 16:38
// Last modified: 27.01.2008 17:02

#region Using directives
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using XnaTetris.Graphics;
using XnaTetris.Helpers;
using XnaTetris.Sounds;
#endregion

namespace XnaTetris.Game
{
	/// <summary>
	/// Base game class for all the basic game support.
	/// Connects all our helper classes together and makes our live easier!
	/// </summary>
	public class BaseGame : Microsoft.Xna.Framework.Game
	{
		#region Variables
		/// <summary>
		/// Graphics
		/// </summary>
		protected GraphicsDeviceManager graphics;

		/// <summary>
		/// Resolution of our game.
		/// </summary>
		protected static int width, height;

		/// <summary>
		/// Font for rendering text
		/// </summary>
		TextureFont font = null;
		#endregion

		#region Properties
		/// <summary>
		/// Width
		/// </summary>
		/// <returns>Int</returns>
		public static int Width
		{
			get
			{
				return width;
			} // get
		} // Width

		/// <summary>
		/// Height
		/// </summary>
		/// <returns>Int</returns>
		public static int Height
		{
			get
			{
				return height;
			} // get
		} // Height
		#endregion

		#region Constructor
		/// <summary>
		/// Create base game
		/// </summary>
		public BaseGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		} // BaseGame()

		/// <summary>
		/// Initialize
		/// </summary>
		protected override void Initialize()
		{
			// Remember resolution
			width = graphics.GraphicsDevice.Viewport.Width;
			height = graphics.GraphicsDevice.Viewport.Height;

			base.Initialize();
		} // Initialize()

		/// <summary>
		/// Load all graphics content (just our background texture).
		/// Use this method to make sure a device reset event is handled correctly.
		/// </summary>
		protected override void LoadContent()
		{
			// Create font
			font = new TextureFont(graphics.GraphicsDevice, Content);
		} // LoadContent()

		/// <summary>
		/// Unload graphic content if the device gets lost.
		/// </summary>
		protected override void UnloadContent()
		{
			Content.Unload();
			SpriteHelper.Dispose();
		} // UnloadContent()
		#endregion

		#region Update
		/// <summary>
		/// Update
		/// </summary>
		/// <param name="gameTime">Game time</param>
		protected override void Update(GameTime gameTime)
		{
			Sound.Update();

			Input.Update();

			if (Input.KeyboardEscapeJustPressed ||
				Input.GamePadBackJustPressed)
				this.Exit();

			base.Update(gameTime);
		} // Update(gameTime)
		#endregion

		#region Draw
		/// <summary>
		/// Draw
		/// </summary>
		/// <param name="gameTime">Game time</param>
		protected override void Draw(GameTime gameTime)
		{
			// Draw all sprites and fonts
			SpriteHelper.DrawSprites(width, height);
			font.WriteAll();

			base.Draw(gameTime);
		} // Draw(gameTime)
		#endregion
	} // class BaseGame
} // namespace XnaTetris.Game
