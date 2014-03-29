// Author: Ben
// Project: XnaTetris
// Path: C:\code\Xna\XnaTetris
// Creation date: 27.01.2008 16:38
// Last modified: 27.01.2008 17:08

#region Using directives
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using XnaTetris.Game;
using XnaTetris.Graphics;
using XnaTetris.Helpers;
using XnaTetris.Sounds;
#endregion

namespace XnaTetris
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class TetrisGame : BaseGame
	{
		#region Variables
		/// <summary>
		/// Textures for the background, the box and the blocks.
		/// </summary>
		Texture2D backgroundTexture, blockTexture,
			backgroundSmallBoxTexture, backgroundBigBoxTexture;

		/// <summary>
		/// Block and background as sprite helpers.
		/// </summary>
		SpriteHelper block, background, backgroundSmallBox, backgroundBigBox;

		/// <summary>
		/// TetrisGrid helper, handles all the graphical aspects!
		/// </summary>
		TetrisGrid tetrisGrid;

		/// <summary>
		/// Level, which sets the difficulty. Starts at level 1 until
		/// we reach level 10 (which is 10 times as fast), then there
		/// is no limit, but the game does not get any harder.
		/// </summary>
		int level = 0;

		/// <summary>
		/// Highscore
		/// </summary>
		int highscore;
		#endregion

		#region Properties
		/// <summary>
		/// Block sprite
		/// </summary>
		/// <returns>Sprite helper</returns>
		public SpriteHelper BlockSprite
		{
			get
			{
				return block;
			} // get
		} // BlockSprite
		#endregion

		#region Constructor
		/// <summary>
		/// Create tetris game
		/// </summary>
		public TetrisGame()
		{
			// Create components
			tetrisGrid = new TetrisGrid(this,
				new Rectangle(512 - 200, 40, 400, 768 - (40 + 44)),
				new Rectangle(512 - 480 + 10, 40, 290 - 50, 280));
			this.Components.Add(tetrisGrid);

			// Don't limit the framerate to the vertical retrace
			graphics.SynchronizeWithVerticalRetrace = false;
			this.IsFixedTimeStep = false;
		} // TetrisGame()
		#endregion

		#region Load content
		/// <summary>
		/// Load all graphics content (just our background texture).
		/// Use this method to make sure a device reset event is handled correctly.
		/// </summary>
		protected override void LoadContent()
		{
			// Load all our content
			backgroundTexture = Content.Load<Texture2D>("SpaceBackground");
			blockTexture = Content.Load<Texture2D>("Block");
			backgroundSmallBoxTexture = Content.Load<Texture2D>("BackgroundSmallBox");
			backgroundBigBoxTexture = Content.Load<Texture2D>("BackgroundBigBox");

			// Create all sprites
			block = new SpriteHelper(blockTexture, null);
			background = new SpriteHelper(backgroundTexture, null);
			backgroundSmallBox = new SpriteHelper(backgroundSmallBoxTexture, null);
			backgroundBigBox = new SpriteHelper(backgroundBigBoxTexture, null);

			base.LoadContent();
		} // LoadContent()
		#endregion

		#region Update
		/// <summary>
		/// Remember current game time and when we pressed left, right
		/// or down the last time to allow movement after waiting for a key
		/// timeout.
		/// </summary>
		int elapsedGameMs = 0;
		int pressedLeftMs = 0;
		int pressedRightMs = 0;
		int pressedDownMs = 0;
		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			int oldGameMs = elapsedGameMs;
			int frameMs = (int)gameTime.ElapsedGameTime.TotalMilliseconds;
			elapsedGameMs += frameMs;
			// Game tick time, 1000 for difficutly=0, 100 for difficutly=9
			int gameTickTime = 1000 / (level + 1);
			if (elapsedGameMs / gameTickTime != oldGameMs / gameTickTime ||
				tetrisGrid.movingDownWasBlocked == true)
				tetrisGrid.Update();

			if (tetrisGrid.lines / 10 > level)
			{
				level++;
				Sound.Play(Sound.Sounds.Victory);
			} // if

			// Handle input, we have 2 modes: Immediate response if one of the
			// keys is pressed to move our block and after a timeout we move even
			// more. This is a slight change, but has a nice impact on the game
			// feeling as controlling the current block feels more natural.
			if (Input.KeyboardLeftPressed ||
				Input.GamePadLeftPressed)
				pressedLeftMs += frameMs;
			else
				pressedLeftMs = 0;
			if (Input.KeyboardRightPressed ||
				Input.GamePadRightPressed)
				pressedRightMs += frameMs;
			else
				pressedRightMs = 0;
			if (Input.KeyboardDownPressed ||
				Input.GamePadDownPressed ||
				Input.Keyboard.IsKeyDown(Keys.Space) ||
				Input.GamePadAPressed)
				pressedDownMs += frameMs;
			else
				pressedDownMs = 0;

			if (Input.KeyboardLeftJustPressed ||
				Input.GamePadLeftJustPressed ||
				pressedLeftMs > 200)
			{
				if (pressedLeftMs > 75)
					pressedLeftMs -= 75;
				tetrisGrid.MoveBlock(TetrisGrid.MoveTypes.Left);
			} // if
			else if (Input.KeyboardRightJustPressed ||
				Input.GamePadRightJustPressed ||
				pressedRightMs > 200)
			{
				if (pressedRightMs > 75)
					pressedRightMs -= 75;
				tetrisGrid.MoveBlock(TetrisGrid.MoveTypes.Right);
			} // else if
			else if (Input.KeyboardDownJustPressed ||
				Input.GamePadDownJustPressed ||
				Input.KeyboardSpaceJustPressed ||
				Input.GamePadAJustPressed ||
				pressedDownMs > 150)
			{
				if (pressedDownMs > 50)
					pressedDownMs -= 50;
				tetrisGrid.MoveBlock(TetrisGrid.MoveTypes.Down);
			} // else if
			else if (Input.KeyboardUpJustPressed ||
				Input.GamePadUpJustPressed)
			{
				tetrisGrid.RotateBlock();
			} // else

			if (tetrisGrid.score > highscore)
				highscore = tetrisGrid.score;

			if (tetrisGrid.gameOver &&
				(Input.KeyboardSpaceJustPressed ||
				Input.GamePadAJustPressed))
			{
				tetrisGrid.gameOver = false;
				tetrisGrid.score = 0;
				tetrisGrid.lines = 0;
				level = 0;
				tetrisGrid.Restart();
			} // if

			// Update title
			//obs: Window.Title = "Xna Tetris - Level " + (level + 1) +
			//	" - Score " + tetrisGrid.score +
			//	" - Highscore " + highscore;

			base.Update(gameTime);
		} // Update(gameTime)
		#endregion

		#region Draw
		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Don't render background boxes or tetris grid for unit tests
			if (isUnitTest)
			{
				base.Draw(gameTime);
				return;
			} // if

			// Render background
			background.Render();

			// Draw background boxes for all the components
			backgroundBigBox.Render(new Rectangle(
				(512 - 200) - 15, 40 - 12, 400 + 23, (768 - (40 + 41)) + 16));
			backgroundSmallBox.Render(new Rectangle(
				(512 - 480) - 15 + 10, 40 - 10, 290 - 30, 300));
			backgroundSmallBox.Render(new Rectangle(
				(512 + 240) - 15 - 10, 40 - 10, 290 - 30, 190));

			// Show tetris grid in center of screen
			tetrisGrid.Draw(gameTime);

			// Show current level, score, etc.
			TextureFont.WriteText(512 + 230, 50, "Level: ");
			TextureFont.WriteText(512 + 400, 50, (level + 1).ToString());
			TextureFont.WriteText(512 + 230, 90, "Score: ");
			TextureFont.WriteText(512 + 400, 90, tetrisGrid.score.ToString());
			TextureFont.WriteText(512 + 230, 130, "Lines: ");
			TextureFont.WriteText(512 + 400, 130, tetrisGrid.lines.ToString());
			TextureFont.WriteText(512 + 230, 170, "Highscore: ");
			TextureFont.WriteText(512 + 400, 170, highscore.ToString());

			base.Draw(gameTime);
		} // Draw(gameTime)
		#endregion

		#region Start game
		static bool isUnitTest = true;
		/// <summary>
		/// Start game
		/// </summary>
		public static void StartGame()
		{
			isUnitTest = false;
			using (TetrisGame game = new TetrisGame())
			{
				game.Run();
			} // using (game)
		} // StartGame()
		#endregion

		#region Unit tests
#if DEBUG
		#region TestBackgroundBoxes
		public static void TestBackgroundBoxes()
		{
			TestGame.Start("TestBackgroundBoxes",
				delegate
				{
					// Render background
					TestGame.game.background.Render();

					// Draw background boxes for all the components
					TestGame.game.backgroundBigBox.Render(new Rectangle(
						(512 - 200) - 15, 40 - 12, 400 + 23, (768 - 40) + 16));
					TestGame.game.backgroundSmallBox.Render(new Rectangle(
						(512 - 480) - 15, 40 - 10, 290 - 30, 300));
					TestGame.game.backgroundSmallBox.Render(new Rectangle(
						(512 + 240) - 15, 40 - 10, 290 - 30, 190));
				});
		} // TestBackgroundBoxes()
		#endregion

		#region TestEmptyGrid
		public static void TestEmptyGrid()
		{
			TestGame.Start("TestEmptyGrid",
				delegate
				{
					// Render background
					TestGame.game.background.Render();

					// Draw background box
					TestGame.game.backgroundBigBox.Render(new Rectangle(
						(512 - 200) - 15, 40 - 12, 400 + 23, (768 - 40) + 16));

					// Show TetrisGrid component inside that box
					TestGame.game.tetrisGrid.Draw(new GameTime());
				});
		} // TestEmptyGrid()
		#endregion
		
		#region TestNextBlock
		public static void TestNextBlock()
		{
			TestGame.Start("TestNextBlock",
				delegate
				{
					// Render background
					TestGame.game.background.Render();

					// Draw background box
					TestGame.game.backgroundSmallBox.Render(new Rectangle(
						(512 - 480) - 15, 40 - 10, 290 - 30, 300));
					
					// Show NextBlock component inside that box
					TestGame.game.tetrisGrid.nextBlock.Draw(new GameTime());
				});
		} // TestNextBlock()
		#endregion

		#region TestScoreboard
		public static void TestScoreboard()
		{
			int level = 3, score = 350, highscore = 1542, lines = 13; 
			TestGame.Start("TestScoreboard",
				delegate
				{
					// Draw background box
					TestGame.game.backgroundSmallBox.Render(new Rectangle(
						(512 + 240) - 15, 40 - 10, 290 - 30, 190));

					// Show tetris grid in center of screen
					TestGame.game.tetrisGrid.Draw(new GameTime());
					// Show current level, score, etc.
					TextureFont.WriteText(512 + 240, 50, "Level: ");
					TextureFont.WriteText(512 + 420, 50, (level + 1).ToString());
					TextureFont.WriteText(512 + 240, 90, "Score: ");
					TextureFont.WriteText(512 + 420, 90, score.ToString());
					TextureFont.WriteText(512 + 240, 130, "Lines: ");
					TextureFont.WriteText(512 + 420, 130, lines.ToString());
					TextureFont.WriteText(512 + 240, 170, "Highscore: ");
					TextureFont.WriteText(512 + 420, 170, highscore.ToString());
				});
		} // TestScoreboard()
		#endregion

		#region TestFallingBlockAndLineKill
		public static void TestFallingBlockAndLineKill()
		{
			// Just use the game itself to test the falling blocks.
			// This was previously done differently, but due the changes made
			// over the development time it was easier to get rid of this
			// unit test and implement everything directly into the game.
			StartGame();
		} // TestFallingBlockAndLineKill()
		#endregion
#endif
		#endregion
	} // class TetrisGame
} // namespace XnaTetris