using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.IO;
using NeoAxis;

namespace Project
{
	public class SpaceshipGameGUI : NeoAxis.UIControl
	{
		
		bool GetShipControl(out double speedingUp, out double turning)
		{
			var script = Scene.First.GetComponentByPath(@"Spaceship\Input Processing\C# Script") as CSharpScript;
			if(script != null)
			{
				speedingUp = script.GetCompiledPropertyValue<double>("LastSpeedingUp");
				turning = script.GetCompiledPropertyValue<double>("LastTurning");
				return true;
			}

			speedingUp = 0;
			turning = 0;
			return false;
		}
		
        protected override void OnRenderUI(CanvasRenderer renderer)
        {
            base.OnRenderUI(renderer);

			//draw control vector
			if(EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation)
			{
				var imageControl = GetComponent<UIControl>("Control");
				if (imageControl != null)
				{
					var rectangle = imageControl.GetScreenRectangle();
					
					if(GetShipControl(out var speedingUp, out var turning))
					{
						var center = rectangle.GetCenter();
						var to = center + new Vector2(rectangle.Size.X / 2 * -turning, rectangle.Size.Y / 2 * -speedingUp);
						
						renderer.AddLine(center, to, new ColorValue(1, 1, 0));
					}
				}
			}
        }

/*		
		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();
			
		}

		bool IsAnyWindowOpened()
		{
			return ParentRoot.GetComponent<UIWindow>(true, true) != null;
		}
*/

   }
}