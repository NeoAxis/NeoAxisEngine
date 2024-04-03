using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.IO;
using NeoAxis;

namespace Project
{
	//!!!!base on BasicSceneScreen?

	public class SimpleGameGUI : NeoAxis.UIControl
	{
		object touchDown;
		Vector2? touchPosition;

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
				InitializeSceneEvents();
		}

		void InitializeSceneEvents()
		{
			//subscribe to Render event of the scene
			Scene.First.RenderEvent += SceneRenderEvent;
		}

		void SceneRenderEvent(Scene scene, Viewport viewport)
		{
			if( !IsAnyWindowOpened() )
			{
				//find object by the cursor
				var obj = GetObjectByCursor( viewport );

				//draw selection border
				if( obj != null )
				{
					viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 1, 0 ) );
					viewport.Simple3DRenderer.AddBounds( obj.SpaceBounds.BoundingBox );
				}
			}
		}

		List<ObjectInSpace> GetObjectsThanCanBeSelected()
		{
			var result = new List<ObjectInSpace>();
			foreach (var obj in Scene.First.GetComponents<MeshInSpace>())
			{
				//skip ground
				if (obj.Name != "Ground")
					result.Add(obj);
			}
			return result;
		}

		ObjectInSpace GetObjectByCursor(Viewport viewport)
		{
			var mouse = MousePosition;
			if( touchPosition != null )
				mouse = touchPosition.Value;

			//get scene object
			var scene = Scene.First;

			//get world ray by cursor position
			var ray = viewport.CameraSettings.GetRayByScreenCoordinates(mouse);

			//get objects by the ray
			var item = new Scene.GetObjectsInSpaceItem(Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, ray);
			scene.GetObjectsInSpace(item);

			//to test by physical objects:
			//scene.PhysicsRayTest()
			//scene.PhysicsContactTest()
			//scene.PhysicsConvexSweepTest()

			var objectsThanCanBeSelected = GetObjectsThanCanBeSelected();

			//process objects
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

			//update Button Next Level
			if (EngineApp.IsSimulation)
			{
				var buttonNextLevel = GetComponent<UIButton>("Button Next Level");
				if( buttonNextLevel != null )
					buttonNextLevel.ReadOnly = GetObjectsThanCanBeSelected().Count != 0 || IsAnyWindowOpened();
			}
		}

		void ClickToDestroy()
		{
			//get viewport
			var viewport = ParentContainer.Viewport;

			//get scene object
			var scene = Scene.First;

			//get object by the cursor
			var obj = GetObjectByCursor( viewport );
			if( obj != null )
			{
				//destroy the object
				obj.RemoveFromParent( false );

				//play sound
				ParentContainer.PlaySound(@"Base\UI\Styles\Sounds\ButtonClick.ogg");
				
				//show screen messages
				var objectsLeft = GetObjectsThanCanBeSelected().Count;
				ScreenMessages.Add( $"Objects left: {objectsLeft}" );

				//check to win
				if( objectsLeft == 0 )
				{
					ScreenMessages.Add( "You won!" );
					scene.SoundPlay2D( @"Samples\Simple Game\Sounds\Win.ogg" );
				}
			}
		}

		protected override bool OnMouseDown(EMouseButtons button)
		{
			if (EngineApp.IsSimulation && button == EMouseButtons.Left && !IsAnyWindowOpened() )
				ClickToDestroy();

			return base.OnMouseDown(button);
		}
		
		bool IsAnyWindowOpened()
		{
			return ParentRoot.GetComponent<UIWindow>(true, true) != null;
		}

		protected override bool OnTouch( TouchData e )
		{
			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( touchDown == null && !IsAnyWindowOpened() )
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
				var currentLevel = VirtualPathUtility.NormalizePath( Scene.First.HierarchyController.CreatedByResource.Owner.Name );
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

		/*public void ButtonExit_Click(NeoAxis.UIButton sender)
        {
			SimulationApp.ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui" );
        }*/
    }
}