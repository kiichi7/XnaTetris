// Project: XnaTetris, File: Directories.cs
// Namespace: XnaTetris.Helpers, Class: Directories
// Path: C:\code\XnaTetris\Helpers, Author: Abi
// Code lines: 169, Size of file: 3,68 KB
// Creation date: 07.09.2006 05:56
// Last modified: 01.10.2006 18:53
// Generated with Commenter by abi.exDream.com

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace XnaTetris.Helpers
{
	/// <summary>
	/// Helper class which stores all used directories.
	/// </summary>
	class Directories
	{
		#region Game base directory
		/// <summary>
		/// We can use this to relocate the whole game directory to another
		/// location. Used for testing (everything is stored on a network drive).
		/// </summary>
		public static readonly string GameBaseDirectory =
			// Update to support Xbox360:
			StorageContainer.TitleLocation;
			//"";
		#endregion

		#region Directories
		/// <summary>
		/// Content directory for all our textures, models and shaders.
		/// </summary>
		/// <returns>String</returns>
		public static string ContentDirectory
		{
			get
			{
				return Path.Combine(GameBaseDirectory, "Content");
			} // get
		} // ContentDirectory

		/// <summary>
		/// Sounds directory, for some reason XAct projects don't produce
		/// any content files (bug?). We just load them ourself!
		/// Update: Uses the content directory now too instead of "Sounds".
		/// </summary>
		/// <returns>String</returns>
		public static string SoundsDirectory
		{
			get
			{
				return Path.Combine(GameBaseDirectory, "Content");
			} // get
		} // SoundsDirectory

		/// <summary>
		/// Textures directory, just used for testing. The game just uses
		/// the content directory.
		/// </summary>
		/// <returns>String</returns>
		public static string TexturesDirectory
		{
			get
			{
				return Path.Combine(GameBaseDirectory, "Textures");
			} // get
		} // TexturesDirectory

		/// <summary>
		/// Shaders directory
		/// </summary>
		/// <returns>String</returns>
		public static string ShadersDirectory
		{
			get
			{
				return Path.Combine(GameBaseDirectory, "Shaders");
			} // get
		} // ShadersDirectory

		/// <summary>
		/// Default Screenshots directory.
		/// </summary>
		/// <returns>String</returns>
		public static string ScreenshotsDirectory
		{
			get
			{
				return Path.Combine(GameBaseDirectory, "Screenshots");
			} // get
		} // ScreenshotsDirectory

		/// <summary>
		/// Levels directory
		/// </summary>
		/// <returns>String</returns>
		public static string TracksDirectory
		{
			get
			{
				return Path.Combine(GameBaseDirectory, "Tracks");
			} // get
		} // TracksDirectory
		#endregion
	} // class Directories
} // namespace XnaTetris.Helpers
