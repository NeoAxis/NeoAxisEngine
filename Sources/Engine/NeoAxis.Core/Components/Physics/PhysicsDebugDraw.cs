// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Security;
using System.Runtime.InteropServices;
using Internal.BulletSharp;
using Internal.BulletSharp.Math;

namespace NeoAxis
{
	class PhysicsDebugDraw : DebugDraw
	{
		public DebugDrawModes debugDrawMode = DebugDrawModes.DrawWireframe;
		public Simple3DRenderer renderer;

		public int verticesRenderedCounter;
		public int verticesRenderedCounterLimit = -1;

		//

		public PhysicsDebugDraw()
		{
		}

		public override void DrawLine( ref Internal.BulletSharp.Math.BVector3 from, ref Internal.BulletSharp.Math.BVector3 to, ref Internal.BulletSharp.Math.BVector3 color )
		{
			if( verticesRenderedCounterLimit != -1 && verticesRenderedCounter > verticesRenderedCounterLimit )
				return;

			renderer.SetColor( new ColorValue( BulletPhysicsUtility.Convert( color ) ) );
			renderer.AddLineThin( BulletPhysicsUtility.Convert( from ), BulletPhysicsUtility.Convert( to ) );

			unchecked
			{
				verticesRenderedCounter += 2;
			}
		}

		public override void Draw3dText( ref Internal.BulletSharp.Math.BVector3 location, string textString )
		{
		}

		public override void ReportErrorWarning( string warningString )
		{
		}

		public override DebugDrawModes DebugMode
		{
			get { return debugDrawMode; }
			set { debugDrawMode = value; }
		}
	}
}
