// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Security;
using System.Runtime.InteropServices;
using BulletSharp;
using BulletSharp.Math;

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

		public override void DrawLine( ref BulletSharp.Math.Vector3 from, ref BulletSharp.Math.Vector3 to, ref BulletSharp.Math.Vector3 color )
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

		public override void Draw3dText( ref BulletSharp.Math.Vector3 location, string textString )
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
