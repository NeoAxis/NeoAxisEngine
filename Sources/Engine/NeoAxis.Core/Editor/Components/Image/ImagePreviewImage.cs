#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class ImagePreviewImage : PreviewImageGenerator
	{
		public ImagePreviewImage()
		{
		}

		public override bool ClampImage
		{
			get { return false; }
		}

		protected override void OnUpdate()
		{
			var scene = CreateScene( false );

			var texture = ObjectOfPreview as ImageComponent;
			if( texture?.Result?.TextureType == ImageComponent.TypeEnum.Cube )
			{
				var skybox = scene.CreateComponent<Skybox>();
				skybox.Cubemap = texture;
				skybox.AllowProcessEnvironmentCubemap = false;
			}

			scene.Enabled = true;

			if( texture?.Result?.TextureType == ImageComponent.TypeEnum._2D )
			{
				double viewScale = 0.95;

				double scale = Math.Min(
					(double)Viewport.SizeInPixels.X / (double)texture.Result.ResultSize.X,
					(double)Viewport.SizeInPixels.Y / (double)texture.Result.ResultSize.Y );
				Vector2 size = texture.Result.ResultSize.ToVector2() * scale * viewScale;
				Vector2 center = Viewport.SizeInPixels.ToVector2() / 2;
				Rectangle rectInPixels = new Rectangle( center - size / 2, center + size / 2 );

				Rectangle rect = rectInPixels / Viewport.SizeInPixels.ToVector2();

				var renderer = Viewport.CanvasRenderer;

				var pointFiltering = false;
				if( rectInPixels.Size.X >= texture.Result.ResultSize.X && rectInPixels.Size.Y >= texture.Result.ResultSize.Y )
					pointFiltering = true;

				if( pointFiltering )
					renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );
				renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), texture );
				if( pointFiltering )
					renderer.PopTextureFilteringMode();
			}
		}

	}
}

#endif