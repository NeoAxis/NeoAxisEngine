using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.IO;
using NeoAxis;
using NeoAxis.Input;

namespace Project
{
	public class SimpleGameGUI : NeoAxis.UIControl
	{
		object touchDown;
		Vector2? touchPosition;

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			InitializeSceneEvents();
		}

		void InitializeSceneEvents()
		{
			// Subscribe to Render event of the scene.
			Component_Scene.First.RenderEvent += SceneRenderEvent;
		}

		void SceneRenderEvent(Component_Scene scene, Viewport viewport)
		{
			// Find object by the cursor. 
			var obj = GetObjectByCursor(viewport);

			// Draw selection border.
			if (obj != null)
			{
				viewport.Simple3DRenderer.SetColor(new ColorValue(1, 1, 0));
				viewport.Simple3DRenderer.AddBounds(obj.SpaceBounds.CalculatedBoundingBox);
			}
		}

		List<Component_ObjectInSpace> GetObjectsThanCanBeSelected()
		{
			var result = new List<Component_ObjectInSpace>();
			foreach (var obj in Component_Scene.First.GetComponents<Component_MeshInSpace>())
			{
				// Skip ground.
				if (obj.Name != "Ground")
					result.Add(obj);
			}
			return result;
		}

		Component_ObjectInSpace GetObjectByCursor(Viewport viewport)
		{
			var mouse = MousePosition;
			if( touchPosition != null )
				mouse = touchPosition.Value;

			// Get scene object.
			var scene = Component_Scene.First;

			// Get world ray by cursor position.
			var ray = viewport.CameraSettings.GetRayByScreenCoordinates(mouse);

			// Get objects by the ray.
			var item = new Component_Scene.GetObjectsInSpaceItem(Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray);
			scene.GetObjectsInSpace(item);

			// To test by physical objects:
			//scene.PhysicsRayTest()
			//scene.PhysicsContactTest()
			//scene.PhysicsConvexSweepTest()

			var objectsThanCanBeSelected = GetObjectsThanCanBeSelected();

			// Process objects.
			foreach (var resultItem in item.Result)
			{
				if (objectsThanCanBeSelected.Contains(resultItem.Object))
				{
					// Found.
					return resultItem.Object;
				}
			}

			return null;
		}

		protected override void OnUpdate(float delta)
		{
			base.OnUpdate(delta);

			// Update Button Next Level.
			if (EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation)
			{
				var buttonNextLevel = GetComponent<UIButton>("Button Next Level");
				if (buttonNextLevel != null)
					buttonNextLevel.ReadOnly = GetObjectsThanCanBeSelected().Count != 0;
			}
		}

		void ClickToDestroy()
		{
			// Get viewport.
			var viewport = ParentContainer.Viewport;

			// Get object by the cursor.
			var obj = GetObjectByCursor( viewport );
			if( obj != null )
			{
				// Destroy the object.
				obj.RemoveFromParent( false );

				// Show screen messages.
				var objectsLeft = GetObjectsThanCanBeSelected().Count;
				ScreenMessages.Add( $"Objects left: {objectsLeft}" );
				if( objectsLeft == 0 )
					ScreenMessages.Add( "You won!" );
			}
		}

		protected override bool OnMouseDown(EMouseButtons button)
		{
			if (EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && button == EMouseButtons.Left)
				ClickToDestroy();

			return base.OnMouseDown(button);
		}

		protected override bool OnTouch( TouchData e )
		{
			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( touchDown == null )
				{
					touchDown = e.PointerIdentifier;
					touchPosition = e.Position;
					ClickToDestroy();
				}
				break;

			case TouchData.ActionEnum.Up:
				if( touchDown != null && ReferenceEquals( e.PointerIdentifier, touchDown ) )
				{
					touchDown = null;
					touchPosition = null;
				}
				break;

			case TouchData.ActionEnum.Move:
				if( touchDown != null && ReferenceEquals( e.PointerIdentifier, touchDown ) )
					touchPosition = e.Position;
				break;

				//case TouchData.ActionEnum.Cancel:
				//	break;
				//case TouchData.ActionEnum.Outside:
				//	break;
			}

			return base.OnTouch( e );
		}

		public void ButtonNextLevel_Click(UIButton sender)
		{
			// Get next level file name.
			string nextLevel;
			{
				var currentLevel = VirtualPathUtility.NormalizePath( Component_Scene.First.HierarchyController.CreatedByResource.Owner.Name );
				var fileName = Path.GetFileName( VirtualPathUtility.NormalizePath( currentLevel ) );
				string numberStr = new String(fileName.Where(Char.IsDigit).ToArray());
				int number = int.Parse(numberStr) + 1;
				nextLevel = Path.Combine(Path.GetDirectoryName(currentLevel), $"SimpleGameLevel{number}.scene");

				//nextLevel = @"Samples\Simple Game\SimpleGameLevel2.scene";				
			}

			if (VirtualFile.Exists(nextLevel))
			{
				// Load next level without changing current GUI screen.
				if (PlayScreen.Instance.Load(nextLevel, false))
				{
					InitializeSceneEvents();
				}
			}
			else
				ScreenMessages.Add("No more levels.");
		}

        public void ButtonExit_Click(NeoAxis.UIButton sender)
        {
			SimulationApp.ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui" );
        }
    }
}