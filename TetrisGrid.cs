// Project: XnaTetris, File: TetrisGrid.cs
// Namespace: XnaTetris, Class: TetrisGrid
// Path: C:\code\XnaBook\XnaTetris, Author: Abi
// Code lines: 16, Size of file: 298 Bytes
// Creation date: 21.11.2006 03:56
// Last modified: 26.11.2006 13:21
// Generated with Commenter by abi.exDream.com

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnaTetris.Helpers;
using XnaTetris.Sounds;
#endregion

namespace XnaTetris
{
	/// <summary>
	/// TetrisGrid helper class to manage the grid and all block types for
	/// the tetris game.
	/// </summary>
	public class TetrisGrid : Microsoft.Xna.Framework.DrawableGameComponent
	{
		#region Constants
		public const int GridWidth = 12;
		public const int GridHeight = 20;

		/// <summary>
		/// Block types we can have for each new block that falls down.
		/// </summary>
		public enum BlockTypes
		{
			Empty,
			Block,
			Triangle,
			Line,
			RightT,
			LeftT,
			RightShape,
			LeftShape,
		} // enum BlockTypes

		/// <summary>
		/// Number of block types we can use for each grid block.
		/// </summary>
		public static readonly int NumOfBlockTypes =
#if XBOX360
			8;
#else
			EnumHelper.GetSize(typeof(BlockTypes));
#endif

		/// <summary>
		/// Block colors for each block type.
		/// </summary>
		public static readonly Color[] BlockColor = new Color[]
			{
				new Color( 60, 60, 60, 128 ), // Empty, color unused
				new Color( 50, 50, 255, 255 ), // Line, blue
				new Color( 160, 160, 160, 255 ), // Block, gray
				new Color( 255, 50, 50, 255 ), // RightT, red
				new Color( 255, 255, 50, 255 ), // LeftT, yellow
				new Color( 50, 255, 255, 255 ), // RightShape, teal
				new Color( 255, 50, 255, 255 ), // LeftShape, purple
				new Color( 50, 255, 50, 255 ), // Triangle, green
			}; // Color[] BlockColor

		/// <summary>
		/// Unrotated shapes
		/// </summary>
		public static readonly int[][,] BlockTypeShapesNormal = new int[][,]
			{
				// Empty
				new int[,] { { 0 } },
				// Line
				new int[,] { { 0, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 } },
				// Block
				new int[,] { { 1, 1 }, { 1, 1 } },
				// RightT
				new int[,] { { 1, 1 }, { 1, 0 }, { 1, 0 } },
				// LeftT
				new int[,] { { 1, 1 }, { 0, 1 }, { 0, 1 } },
				// RightShape
				new int[,] { { 0, 1, 1 }, { 1, 1, 0 } },
				// LeftShape
				new int[,] { { 1, 1, 0 }, { 0, 1, 1 } },
				// LeftShape
				new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 0, 0 } },
			}; // BlockTypeShapesNormal
		#endregion

		#region Variables
		/// <summary>
		/// Remember game for accessing textures and sprites
		/// </summary>
		TetrisGame game = null;

		/// <summary>
		/// Precalculated Rotated shapes
		/// </summary>
		static int[,][,] BlockTypeShapes;

		/// <summary>
		/// The actual grid, contains all blocks,
		/// including the currently falling block.
		/// </summary>
		BlockTypes[,] grid = new BlockTypes[GridWidth, GridHeight];
		/// <summary>
		/// Use this simple array to see where the floating parts are.
		/// Simply check from bottom up and putting stuff in the next line.
		/// </summary>
		bool[,] floatingGrid = new bool[GridWidth, GridHeight];

		/// <summary>
		/// When game is over, this is set to true, start over then!
		/// </summary>
		public bool gameOver = false;

		/// <summary>
		/// Remember current block type and rotation
		/// </summary>
		int currentBlockType = 0;
		int currentBlockRot = 0;
		Point currentBlockPos;

		/// <summary>
		/// Next block game component, does not only store the next block type,
		/// but also displays it.
		/// </summary>
		internal NextBlock nextBlock;

		/// <summary>
		/// Grid rectangle for drawing.
		/// </summary>
		Rectangle gridRect;
		#endregion

		#region Constructor
		public TetrisGrid(TetrisGame setGame, Rectangle setGridRect,
			Rectangle setNextBlockRect)
			: base(setGame)
		{
			game = setGame;
			gridRect = setGridRect;

			nextBlock = new NextBlock(game, setNextBlockRect);
			game.Components.Add(nextBlock);
		} // TetrisGrid(game)
		#endregion

		#region Initialize
		public override void Initialize()
		{
			// Precalculate Rotated shapes, for all types
			BlockTypeShapes = new int[NumOfBlockTypes, 4][,];
			for (int type = 0; type < NumOfBlockTypes; type++)
			{
				int[,] shape = BlockTypeShapesNormal[type];
				int width = shape.GetLength(0);
				int height = shape.GetLength(1);
				// Init all precalculated shapes
				BlockTypeShapes[type, 0] = new int[height, width];
				BlockTypeShapes[type, 1] = new int[width, height];
				BlockTypeShapes[type, 2] = new int[height, width];
				BlockTypeShapes[type, 3] = new int[width, height];
				for (int x = 0; x < width; x++)
					for (int y = 0; y < height; y++)
					{
						BlockTypeShapes[type, 0][y, x] = shape[(width - 1) - x, y];
						BlockTypeShapes[type, 1][x, y] = shape[x, y];
						BlockTypeShapes[type, 2][y, x] = shape[x, (height - 1) - y];
						BlockTypeShapes[type, 3][x, y] =
							shape[(width - 1) - x, (height - 1) - y];
					} // for for
			} // for

			Restart();
			AddRandomBlock();

			base.Initialize();
		} // Initialize()
		#endregion

		#region Restart
		public void Restart()
		{
			for ( int x=0; x<GridWidth; x++ )
				for ( int y=0; y<GridHeight; y++ )
				{
					grid[x,y] = BlockTypes.Empty;
					floatingGrid[x,y] = false;
				} // for for

			//done automatically: AddRandomBlock();
			Sound.Play(Sound.Sounds.Fight);
		} // Restart()
		#endregion

		#region Add random block
		/// <summary>
		/// Adds a random block in the top middle
		/// </summary>
		public void AddRandomBlock()
		{
			// Randomize block type and rotation
			currentBlockType = (int)nextBlock.SetNewRandomBlock();
			currentBlockRot = RandomHelper.GetRandomInt(4);

			// Get precalculated shape
			int[,] shape = BlockTypeShapes[currentBlockType,currentBlockRot];
			int xPos = GridWidth/2-shape.GetLength(0)/2;
			// Center block at top most position of our grid
			currentBlockPos = new Point(xPos, 0);

			// Add new block
			for ( int x=0; x<shape.GetLength(0); x++ )
				for ( int y=0; y<shape.GetLength(1); y++ )
					if ( shape[x,y] > 0 )
					{
						// Check if there is already something
						if (grid[x + xPos, y] != BlockTypes.Empty)
						{
							// Then game is over dude!
							gameOver = true;
							Sound.Play(Sound.Sounds.Lose);
						} // if
						else
						{
							grid[x + xPos, y] = (BlockTypes)currentBlockType;
							floatingGrid[x + xPos, y] = true;
						} // else
					} // for for if

			// Add 1 point per block!
			score++;
		} // AddRandomBlock()
		#endregion

		#region Move block
		public enum MoveTypes
		{
			Left,
			Right,
			Down,
		} // enum MoveTypes

		/// <summary>
		/// Remember if moving down was blocked, this increases
		/// the game speed because we can force the next block!
		/// </summary>
		public bool movingDownWasBlocked = false;
		/// <summary>
		/// Move current floating block to left, right or down.
		/// If anything is blocking, moving is not possible and
		/// nothing gets changed!
		/// </summary>
		/// <returns>Returns true if moving was successful, otherwise false</returns>
		public bool MoveBlock(MoveTypes moveType)
		{
			// Clear old pos
			for ( int x=0; x<GridWidth; x++ )
				for ( int y=0; y<GridHeight; y++ )
					if ( floatingGrid[x,y] )
						grid[x,y] = BlockTypes.Empty;
			// Move stuff to new position
			bool anythingBlocking = false;
			Point[] newPos = new Point[4];
			int newPosNum = 0;
			if ( moveType == MoveTypes.Left )
			{
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						if ( floatingGrid[x,y] )
						{
							if ( x-1 < 0 ||
								grid[x-1,y] != BlockTypes.Empty )
								anythingBlocking = true;
							else if ( newPosNum < 4 )
							{
								newPos[newPosNum] = new Point( x-1, y );
								newPosNum++;
							} // else if
						} // for for if
			} // if (left)
			else if ( moveType == MoveTypes.Right )
			{
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						if ( floatingGrid[x,y] )
						{
							if ( x+1 >= GridWidth ||
								grid[x+1,y] != BlockTypes.Empty )
								anythingBlocking = true;
							else if ( newPosNum < 4 )
							{
								newPos[newPosNum] = new Point( x+1, y );
								newPosNum++;
							} // else if
						} // for for if
			} // if (right)
			else if ( moveType == MoveTypes.Down )
			{
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						if ( floatingGrid[x,y] )
						{
							if ( y+1 >= GridHeight ||
								grid[x,y+1] != BlockTypes.Empty )
								anythingBlocking = true;
							else if ( newPosNum < 4 )
							{
								newPos[newPosNum] = new Point( x, y+1 );
								newPosNum++;
							} // else if
						} // for for if
				if ( anythingBlocking == true )
					movingDownWasBlocked = true;
			} // if (down)

			// If anything is blocking restore old state
			if ( anythingBlocking ||
				// Or we didn't get all 4 new positions?
				newPosNum != 4 )
			{
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						if ( floatingGrid[x,y] )
							grid[x,y] = (BlockTypes)currentBlockType;
				return false;
			} // if
			else
			{
				if ( moveType == MoveTypes.Left )
					currentBlockPos = new Point( currentBlockPos.X-1, currentBlockPos.Y );
				else if ( moveType == MoveTypes.Right )
					currentBlockPos = new Point( currentBlockPos.X+1, currentBlockPos.Y );
				else if ( moveType == MoveTypes.Down )
					currentBlockPos = new Point( currentBlockPos.X, currentBlockPos.Y+1 );

				// Else we can move to the new position, lets do it!
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						floatingGrid[x,y] = false;
				for ( int i=0; i<4; i++ )
				{
					grid[newPos[i].X,newPos[i].Y] = (BlockTypes)currentBlockType;
					floatingGrid[newPos[i].X,newPos[i].Y] = true;
				} // for
				Sound.Play(Sound.Sounds.BlockMove);

				return true;
			} // else
		} // MoveBlock(moveType)
		#endregion

		#region Rotate block
		public void RotateBlock()
		{
			// Clear old pos
			for ( int x=0; x<GridWidth; x++ )
				for ( int y=0; y<GridHeight; y++ )
					if ( floatingGrid[x,y] )
						grid[x,y] = BlockTypes.Empty;
			// Rotate and check if new position is valid
			bool anythingBlocking = false;
			Point[] newPos = new Point[4];
			int newPosNum = 0;

			int newRotation = (currentBlockRot+1)%4;
			int[,] shape = BlockTypeShapes[currentBlockType,newRotation];
			for ( int x=0; x<shape.GetLength(0); x++ )
				for ( int y=0; y<shape.GetLength(1); y++ )
					if ( shape[x,y] > 0 )
					{
						if ( currentBlockPos.X+x >= GridWidth ||
							currentBlockPos.Y+y >= GridHeight ||
							currentBlockPos.X+x < 0 ||
							currentBlockPos.Y+y < 0 ||
							grid[currentBlockPos.X+x,currentBlockPos.Y+y] != BlockTypes.Empty )
							anythingBlocking = true;
						else if ( newPosNum < 4 )
						{
							newPos[newPosNum] = new Point(
								currentBlockPos.X+x, currentBlockPos.Y+y );
							newPosNum++;
						} // else if
					} // for for if

			// If anything is blocking restore old state
			if ( anythingBlocking ||
				// Or we didn't get all 4 new positions?
				newPosNum != 4 )
			{
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						if ( floatingGrid[x,y] )
							grid[x,y] = (BlockTypes)currentBlockType;
			} // if
			else
			{
				// Else we can rotate, lets do it!
				currentBlockRot = newRotation;
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						floatingGrid[x,y] = false;
				for ( int i=0; i<4; i++ )
				{
					grid[newPos[i].X,newPos[i].Y] = (BlockTypes)currentBlockType;
					floatingGrid[newPos[i].X,newPos[i].Y] = true;
				} // for
				Sound.Play(Sound.Sounds.BlockRotate);
			} // else
		} // RotateBlock()
		#endregion

		#region Update
		// Current score we got
		public int score = 0;
		// Number of lines we crushed
		public int lines = 0;
		/// <summary>
		/// Update whole field, move current floating stuff down
		/// and check if any full lines are given.
		/// Note: Do not use the override Update(gameTime) method here,
		/// Update is ONLY called when 1000ms have passed (level 1), or
		/// 100ms for level 10.
		/// </summary>
		public void Update()
		{
			if (gameOver)
				return;

			// Try to move floating stuff down
			if (MoveBlock(MoveTypes.Down) == false ||
				movingDownWasBlocked)
			{
				// Failed? Then fix floating stuff, not longer moveable!
				for ( int x=0; x<GridWidth; x++ )
					for ( int y=0; y<GridHeight; y++ )
						floatingGrid[x,y] = false;
				Sound.Play(Sound.Sounds.BlockFalldown);
			} // if
			movingDownWasBlocked = false;

			// Check if we got any moveable stuff,
			// if not add new random block at top!
			bool canMove = false;
			for ( int x=0; x<GridWidth; x++ )
				for ( int y=0; y<GridHeight; y++ )
					if ( floatingGrid[x,y] )
						canMove = true;
			if (canMove == false)
			{
				int linesKilled = 0;
				// Check if we got a full line
				for ( int y=0; y<GridHeight; y++ )
				{
					bool fullLine = true;
					for ( int x=0; x<GridWidth; x++ )
						if ( grid[x,y] == BlockTypes.Empty )
						{
							fullLine = false;
							break;
						} // for if
					// We got a full line?
					if (fullLine)
					{
						// Move everything down
						for ( int yDown=y-1; yDown>0; yDown-- )
							for ( int x=0; x<GridWidth; x++ )
								grid[x,yDown+1] = grid[x,yDown];
						// Clear top line
						for ( int x=0; x<GridWidth; x++ )
							grid[0,x] = BlockTypes.Empty;
						// Add 10 points and count line
						score += 10;
						lines++;
						linesKilled++;
						Sound.Play(Sound.Sounds.LineKill);
					} // if
				} // for
				// If we killed 2 or more lines, add extra score
				if (linesKilled >= 2)
					score += 5;
				if (linesKilled >= 3)
					score += 10;
				if (linesKilled >= 4)
					score += 25;

				// Add new block at top
				AddRandomBlock();
			} // if
		} // Update()
		#endregion

		#region Draw
		public override void Draw(GameTime gameTime)
		{
			// Show next block
			nextBlock.Draw(gameTime);

			// Calc sizes for block, etc.
			int blockWidth = gridRect.Width / GridWidth;
			int blockHeight = gridRect.Height / GridHeight;
			if ( blockWidth < 2 )
				blockWidth = 2;
			if ( blockHeight < 2 )
				blockHeight = 2;

			for ( int x=0; x<GridWidth; x++ )
				for ( int y=0; y<GridHeight; y++ )
				{
					game.BlockSprite.Render(new Rectangle(
						gridRect.X + x * blockWidth,
						gridRect.Y + y * blockHeight,
						blockWidth-1, blockHeight-1 ),
						BlockColor[(int)grid[x,y]]);
				} // for for
		} // Draw(gameTime)
		#endregion
	} // class TetrisGrid
} // namespace XnaTetris.TetrisGrid
