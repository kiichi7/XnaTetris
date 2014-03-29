
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XnaTetris.Graphics;
using XnaTetris.Helpers;
#endregion

namespace XnaTetris
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public partial class NextBlock : Microsoft.Xna.Framework.DrawableGameComponent
	{
		#region Constants
		/// <summary>
		/// Width and height of the preview grid
		/// </summary>
		const int PreviewGridWidth = 5,
			PreviewGridHeight = 5;
		#endregion

		#region Variables
		/// <summary>
		/// Remember game for accessing textures and sprites
		/// </summary>
		TetrisGame game = null;

		/// <summary>
		/// Next block type we are currently displaying
		/// </summary>
		TetrisGrid.BlockTypes nextBlockType = TetrisGrid.BlockTypes.Empty;

		/// <summary>
		/// Set new random block and return the last block type.
		/// </summary>
		/// <returns></returns>
		public TetrisGrid.BlockTypes SetNewRandomBlock()
		{
			// Generate first next block if not done yet
			if (nextBlockType == TetrisGrid.BlockTypes.Empty)
				nextBlockType = (TetrisGrid.BlockTypes)
					(1+RandomHelper.GetRandomInt(TetrisGrid.NumOfBlockTypes - 1));

			// Copy over current next and use it for current block
			TetrisGrid.BlockTypes currentBlockType = nextBlockType;

			// Generate next random block type
			nextBlockType = (TetrisGrid.BlockTypes)
				(1 + RandomHelper.GetRandomInt(TetrisGrid.NumOfBlockTypes - 1));

			return currentBlockType;
		} // SetNewRandomBlock()

		/// <summary>
		/// Rectangle for this element. Set in constructor, used in Draw.
		/// </summary>
		Rectangle nextBlockRect;
		#endregion

		#region Constructor
		public NextBlock(TetrisGame setGame, Rectangle setNextBlockRect)
			: base(setGame)
		{
			game = setGame;
			nextBlockRect = setNextBlockRect;
		} // NextBlock(game)
		#endregion

		#region Draw
		public override void Draw(GameTime gameTime)
		{
			TextureFont.WriteText(nextBlockRect.X + 5, nextBlockRect.Y + 10, "Next:");

			Rectangle gridRect = new Rectangle(
				nextBlockRect.X + 5, nextBlockRect.Y + 43,
				nextBlockRect.Width - 15, nextBlockRect.Height - 46);
			int blockWidth = gridRect.Width / PreviewGridWidth;
			int blockHeight = gridRect.Height / PreviewGridHeight;

			for (int x = 0; x < PreviewGridWidth; x++)
				for (int y = 0; y < PreviewGridHeight; y++)
				{
					int[,] blockData = TetrisGrid.BlockTypeShapesNormal[(int)nextBlockType];
					bool isFilled = x > 0 && y > 0 &&
						x - 1 < blockData.GetLength(0) &&
						y - 1 < blockData.GetLength(1) &&
						blockData[x - 1, y - 1] != 0;
					game.BlockSprite.Render(new Rectangle(
						gridRect.X + x * blockWidth,
						gridRect.Y + y * blockHeight,
						blockWidth - 1, blockHeight - 1),
						TetrisGrid.BlockColor[isFilled ? (int)nextBlockType : 0]);
				} // for for
		} // Draw(gameTime)
		#endregion
	} // class NextBlock
} // namespace XnaTetris


