// Only used in debug mode
#if DEBUG
#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using XnaTetris.Helpers;
using Microsoft.Xna.Framework;
#endregion

namespace XnaTetris.Game
{
	/// <summary>
	/// Test game
	/// </summary>
	public class TestGame : TetrisGame//BaseGame
	{
		#region Variables
		/// <summary>
		/// Render delegate for rendering methods, also used for many other
		/// methods.
		/// </summary>
		public delegate void RenderDelegate();

		/// <summary>
		/// Init code
		/// </summary>
		protected RenderDelegate renderCode;
		#endregion
		
		#region Constructor
		/// <summary>
		/// Create test game
		/// </summary>
		/// <param name="setWindowsTitle">Set windows title</param>
		/// <param name="windowWidth">Window width</param>
		/// <param name="windowHeight">Window height</param>
		/// <param name="setInitCode">Set init code</param>
		/// <param name="setUpdateCode">Set update code</param>
		/// <param name="setRenderCode">Set render code</param>
		protected TestGame(string setWindowsTitle,
			RenderDelegate setRenderCode)
		{
			this.Window.Title = setWindowsTitle;

#if !XBOX360
#if DEBUG
			// Force window on top
			WindowsHelper.ForceForegroundWindow(this.Window.Handle.ToInt32());
#endif
#endif
			renderCode = setRenderCode;
		} // TestGame(setWindowsTitle, setRenderCode)
		#endregion

		#region Update
		/// <summary>
		/// Update
		/// </summary>
		protected override void Update(GameTime time)
		{
			base.Update(time);
		} // Update()
		#endregion

		#region Draw
		/// <summary>
		/// Draw
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
			// Drawing code
			if (renderCode != null)
				renderCode();

			base.Draw(gameTime);
		} // Draw(gameTime)
		#endregion

		#region Start test
		public static TestGame game;

		/// <summary>
		/// Start
		/// </summary>
		/// <param name="testName">Test name</param>
		/// <param name="renderCode">Render code</param>
		public static void Start(string testName,
			RenderDelegate renderCode)
		{
			using (game = new TestGame(testName, renderCode))
			{
				game.Run();
			} // using (game)
		} // Start(testName, initCode, updateCode)

		/// <summary>
		/// Start
		/// </summary>
		/// <param name="renderCode">Render code</param>
		public static void Start(RenderDelegate renderCode)
		{
			using (game = new TestGame("Unit Test", renderCode))
			{
				game.Run();
			} // using (game)
		} // Start(testName, initCode, updateCode)
		#endregion

		#region Unit Testing
#if DEBUG
		#region TestEmptyGame
		/// <summary>
		/// Test empty game
		/// </summary>
		public static void TestEmptyGame()
		{
			TestGame.Start(null);
		} // TestEmptyGame()
		#endregion
#endif
		#endregion
	} // class TestGame
} // namespace XnaTetris.Game
#endif