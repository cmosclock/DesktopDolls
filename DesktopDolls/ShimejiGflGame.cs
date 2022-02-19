/******************************************************************************
 * Spine Runtimes Software License
 * Version 2.1
 * 
 * Copyright (c) 2013, Esoteric Software
 * All rights reserved.
 * 
 * You are granted a perpetual, non-exclusive, non-sublicensable and
 * non-transferable license to install, execute and perform the Spine Runtimes
 * Software (the "Software") solely for internal use. Without the written
 * permission of Esoteric Software (typically granted by licensing Spine), you
 * may not (a) modify, translate, adapt or otherwise create derivative works,
 * improvements of the Software or develop new applications using the Software
 * or (b) remove, delete, alter or obscure any trademarks or any copyright,
 * trademark, patent or other intellectual property or proprietary rights
 * notices on or in the Software, including any copy thereof. Redistributions
 * in binary or source form must include this license and terms.
 * 
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Forms;
using DesktopDolls.Spine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vanara.PInvoke;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace DesktopDolls {
	public class ShimejiGflGame : Game {
		GraphicsDeviceManager graphics;
		SkeletonMeshRenderer skeletonRenderer;
		Skeleton skeleton;
		AnimationState state;
		private SpriteBatch _spriteBatch;

		private SpriteFont _font;
		private bool _holding;
		private readonly SwapChainRenderTarget _clickPanelRenderT;
		private readonly Texture2D _clickRect;
		private readonly Form _clickPanelForm;

		public ShimejiGflGame () {
			IsMouseVisible = true;
			graphics = new GraphicsDeviceManager(this);
			Window.IsBorderless = true;
			var content = new EmbeddedResourceContentManager(Services);
			content.RootDirectory = "Content";
			Content = content;
			
			// fps
			graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;
			TargetElapsedTime = TimeSpan.FromSeconds(1d / 300d);
			// disable inactive sleep
			InactiveSleepTime = TimeSpan.Zero;

			var mainGameForm = (Form)Control.FromHandle(Window.Handle);
			mainGameForm.FormBorderStyle = FormBorderStyle.None;
			mainGameForm.TopMost = true;
			mainGameForm.WindowState = FormWindowState.Maximized;
			mainGameForm.ShowInTaskbar = false;
			MakeFullScreenOverlay(Window.Handle);
			graphics.HardwareModeSwitch = false;
			graphics.IsFullScreen = true;
			graphics.PreferredBackBufferWidth = (int)SystemParameters.FullPrimaryScreenWidth;
			graphics.PreferredBackBufferHeight = (int)SystemParameters.FullPrimaryScreenHeight;
			graphics.ApplyChanges();
			mainGameForm.Hide();
			
			_clickPanelForm = new Form();
			_clickPanelForm.FormBorderStyle = FormBorderStyle.None;
			_clickPanelForm.TransparencyKey = System.Drawing.Color.Red;
			_clickPanelForm.TopMost = true;
			_clickPanelForm.WindowState = FormWindowState.Maximized;
			_clickPanelForm.AllowTransparency = true;
			_clickPanelForm.ShowInTaskbar = false;
			_clickPanelForm.Opacity = 0.01;
			MakeFullScreenOverlay(_clickPanelForm.Handle, true);
			_clickPanelRenderT = new SwapChainRenderTarget(GraphicsDevice,
				_clickPanelForm.Handle,
				_clickPanelForm.Width,
				_clickPanelForm.Height,
				false,
				SurfaceFormat.Color,
				DepthFormat.Depth16,
				1,
				RenderTargetUsage.DiscardContents,
				PresentInterval.Default);
			
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_clickRect = new Texture2D(GraphicsDevice, 1, 1);
			_clickRect.SetData(new[] { Color.Blue });
		}

		public void MakeFullScreenOverlay(IntPtr hWnd, bool clickable = false)
		{
			var flag = 0
			           | User32.GetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE)
			           // hide in alt tab
			           | (int)User32.WindowStylesEx.WS_EX_TOOLWINDOW 
			           | 0;
			if (!clickable)
			{
				// make entire window click through
				flag |= (int)User32.WindowStylesEx.WS_EX_TRANSPARENT | (int)User32.WindowStylesEx.WS_EX_LAYERED;
			}

			User32.SetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE, flag);
			User32.SetWindowPos(hWnd, HWND.HWND_TOPMOST, 0, 0, 0, 0, 0
			                                                         | User32.SetWindowPosFlags.SWP_NOSIZE
			                                                         | User32.SetWindowPosFlags.SWP_NOMOVE
			                                                         | 0);
			DwmApi.MARGINS margins = new DwmApi.MARGINS(-1);
			DwmApi.DwmExtendFrameIntoClientArea(Window.Handle, margins);
		}

		protected override void Initialize () {
			// TODO: Add your initialization logic here
			//
			
			_clickPanelForm.Show();
			// should swap the 2 windows instead so that input api would work...
			_clickPanelForm.KeyDown += (object sender, KeyEventArgs args) =>
			{
				if (args.KeyCode == System.Windows.Forms.Keys.Escape)
				{
					Exit();
				}
			};
			base.Initialize();
		}

		protected override void LoadContent () {
			skeletonRenderer = new SkeletonMeshRenderer(GraphicsDevice);
			skeletonRenderer.PremultipliedAlpha = true;

			var name = "rm4a1";

			var atlasPath = $"Content/bin/DesktopGL/Content/spine/{name}.atlas";
			using var atlasStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(DesktopDolls)}.{atlasPath.Replace("/", ".")}");
			using var atlasReader = new StreamReader(atlasStream);
			var atlas = new Atlas(atlasReader, "Content/bin/DesktopGL/Content/spine/", new XnaTextureLoader(GraphicsDevice));

			var scale = 1;
			var binary = new SkeletonBinary(atlas);
			binary.Scale = scale;
			var skeletonPath = $"Content/bin/DesktopGL/Content/spine/{name}.skel";
			using var skeletonStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(DesktopDolls)}.{skeletonPath.Replace("/", ".")}");
			var skeletonData = binary.ReadSkeletonData(skeletonStream);
			skeleton = new Skeleton(skeletonData);

			// Define mixing between animations.
			var stateData = new AnimationStateData(skeleton.Data);
			state = new AnimationState(stateData);

			state.SetAnimation(0, "wait", true);
			stateData.SetMix("wait", "pick", 0.1f);
			stateData.SetMix("pick", "wait", 0.2f);

			skeleton.X = GraphicsDevice.Viewport.Bounds.Width / 2;
			skeleton.Y = GraphicsDevice.Viewport.Bounds.Height / 2;
			skeleton.UpdateWorldTransform();
			
			// fonts
			_font = Content.Load<SpriteFont>("bin.Windows.Content.DebugText");
		}

		protected override void UnloadContent () {
			// TODO: Unload any non ContentManager content here
			base.UnloadContent();
		}

		protected override void Update (GameTime gameTime) {
			// Allows the game to exit
			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw (GameTime gameTime) {
			// helper window (intercept click)
			GraphicsDevice.SetRenderTarget(_clickPanelRenderT);
			GraphicsDevice.Clear(Color.Red);

			_spriteBatch.Begin();
			_spriteBatch.Draw(_clickRect, new Vector2(skeleton.X - skeleton.Data.Width / 2, skeleton.Y - skeleton.Data.Height), null,
				Color.Blue, 0f, Vector2.Zero, new Vector2(skeleton.Data.Width, skeleton.Data.Height),
				SpriteEffects.None, 1f);

			_spriteBatch.End();
			_clickPanelRenderT.Present();
			
			// game window
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Transparent);
			
			#if DEBUG
			_spriteBatch.Begin();
			_spriteBatch.DrawString(_font, $"mouse x - {Mouse.GetState().X} - y - {Mouse.GetState().Y}", new Vector2(10, 10),
				Color.Orange, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
			_spriteBatch.DrawString(_font, $"mouse x - {skeleton.X} - y - {skeleton.Y}", new Vector2(10, 50),
				Color.Orange, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
			_spriteBatch.DrawString(_font, $"fps - {1 / gameTime.ElapsedGameTime.TotalSeconds}", new Vector2(10, 130),
				Color.Orange, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
			_spriteBatch.End();
			#endif
			
			state.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
			state.Apply(skeleton);
			skeleton.UpdateWorldTransform();
			skeletonRenderer.Begin();
			skeletonRenderer.Draw(skeleton);
			skeletonRenderer.End();
			
			var mouse = Mouse.GetState();
			// origin is center bottom
			if (mouse.X > skeleton.X - skeleton.Data.Width / 2 && mouse.X < skeleton.X + skeleton.Data.Width / 2
			    && mouse.Y > skeleton.Y - skeleton.Data.Height && mouse.Y < skeleton.Y + skeleton.Data.Height) {
#if DEBUG
				_spriteBatch.Begin();
				_spriteBatch.DrawString(_font, $"HIT", new Vector2(10, 90),
					Color.Orange, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
				_spriteBatch.End();
#endif
				if (mouse.LeftButton == ButtonState.Pressed && !_holding)
				{
					_holding = true;
					state.SetAnimation(0, "pick", true);
				}
			}
			if (_holding)
			{
				skeleton.X = mouse.X;
				skeleton.Y = mouse.Y + skeleton.Data.Height / 2;
			}
			if (_holding && mouse.LeftButton != ButtonState.Pressed)
			{
				_holding = false;
				state.SetAnimation(0, "wait", true);
			}
			base.Draw(gameTime);
		}
	}
}
