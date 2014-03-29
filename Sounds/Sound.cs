// Project: XnaTetris, File: Sound.cs
// Namespace: XnaTetris.Sounds, Class: Sound
// Path: C:\code\XnaTetris\Sounds, Author: Abi
// Code lines: 729, Size of file: 21,38 KB
// Creation date: 23.10.2006 17:21
// Last modified: 23.10.2006 23:28
// Generated with Commenter by abi.exDream.com

#region Using directives
#if DEBUG
//using NUnit.Framework;
#endif
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using XnaTetris.Game;
using XnaTetris.Graphics;
using XnaTetris.Helpers;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Microsoft.Xna.Framework;
using System.IO;
#endregion

namespace XnaTetris.Sounds
{
	/// <summary>
	/// Sound
	/// </summary>
	class Sound
	{
		#region Variables
		/// <summary>
		/// Sound stuff for XAct
		/// </summary>
		static AudioEngine audioEngine;
		/// <summary>
		/// Wave bank
		/// </summary>
		static WaveBank waveBank;
		/// <summary>
		/// Sound bank
		/// </summary>
		static SoundBank soundBank;
		#endregion

		#region Enums
		/// <summary>
		/// Sounds we use in this game.
		/// </summary>
		/// <returns>Enum</returns>
		public enum Sounds
		{
			BlockMove,
			BlockRotate,
			BlockFalldown,
			LineKill,
			Fight,
			Victory,
			Lose,
		} // enum Sounds
		#endregion

		#region Constructor
		/// <summary>
		/// Create sound
		/// </summary>
		static Sound()
		{
			try
			{
				string dir = Directories.SoundsDirectory;
				audioEngine = new AudioEngine(
					Path.Combine(dir, "TetrisSound.xgs"));
				waveBank = new WaveBank(audioEngine,
					Path.Combine(dir, "Wave Bank.xwb"));

				// Dummy wavebank call to get rid of the warning that waveBank is
				// never used (well it is used, but only inside of XNA).
				if (waveBank != null)
					soundBank = new SoundBank(audioEngine,
						Path.Combine(dir, "Sound Bank.xsb"));
			} // try
			catch (Exception ex)
			{
				// Audio creation crashes in early xna versions, log it and ignore it!
				Log.Write("Failed to create sound class: " + ex.ToString());
			} // catch
		} // Sound()
		#endregion

		#region Play
		/// <summary>
		/// Play
		/// </summary>
		/// <param name="soundName">Sound name</param>
		public static void Play(string soundName)
		{
			if (soundBank == null)
				return;

			try
			{
				soundBank.PlayCue(soundName);
			} // try
			catch (Exception ex)
			{
				Log.Write("Playing sound " + soundName + " failed: " + ex.ToString());
			} // catch
		} // Play(soundName)

		/// <summary>
		/// Play
		/// </summary>
		/// <param name="sound">Sound</param>
		public static void Play(Sounds sound)
		{
			Play(sound.ToString());
		} // Play(sound)
		#endregion

		#region Update
		/// <summary>
		/// Update, just calls audioEngine.Update!
		/// </summary>
		public static void Update()
		{
			if (audioEngine != null)
				audioEngine.Update();
		} // Update()
		#endregion

		#region Unit Testing
#if DEBUG
		/// <summary>
		/// Test play click sound
		/// </summary>
		//[Test]
		public static void TestPlayClickSound()
		{
			//int crazyCounter = 0;

			TestGame.Start(
				delegate
				{
					if (Input.MouseLeftButtonJustPressed ||
						Input.GamePadAJustPressed)
						Sound.Play(Sounds.BlockMove);
					else if (Input.MouseRightButtonJustPressed ||
						Input.GamePadBJustPressed)
						Sound.Play(Sounds.BlockRotate);
					else if (Input.KeyboardKeyJustPressed(Keys.D1))
						Sound.Play(Sounds.BlockFalldown);
					else if (Input.KeyboardKeyJustPressed(Keys.D2))
						Sound.Play(Sounds.LineKill);
					else if (Input.KeyboardKeyJustPressed(Keys.D3))
						Sound.Play(Sounds.Fight);
					else if (Input.KeyboardKeyJustPressed(Keys.D4))
						Sound.Play(Sounds.Victory);
					else if (Input.KeyboardKeyJustPressed(Keys.D5))
						Sound.Play(Sounds.Lose);

					TextureFont.WriteText(2, 30,
						"Press 0-5 or A/B or left/right mouse buttons to play back "+
						"sounds!");
				});
		} // TestPlayClickSound()
#endif
		#endregion
	} // class Sound
} // XnaTetris.Sounds
