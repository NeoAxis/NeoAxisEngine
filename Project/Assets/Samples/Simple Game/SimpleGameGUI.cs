using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.IO;
using NeoAxis;

namespace Project
{
	public class SimpleGameGUI : NeoAxis.UIControl
	{
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
			// Get scene object.
			var scene = Component_Scene.First;

			// Get world ray by cursor position.
			var ray = viewport.CameraSettings.GetRayByScreenCoordinates(MousePosition);

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


		protected override bool OnMouseDown(EMouseButtons button)
		{
			if (EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && button == EMouseButtons.Left)
			{
				// Get viewport.
				var viewport = ParentContainer.Viewport;

				// Get object by the cursor.
				var obj = GetObjectByCursor(viewport);
				if (obj != null)
				{
					// Destroy the object.
					obj.RemoveFromParent(false);

					// Show screen messages.
					var objectsLeft = GetObjectsThanCanBeSelected().Count;
					ScreenMessages.Add($"Objects left: {objectsLeft}");
					if (objectsLeft == 0)
						ScreenMessages.Add("You won!");
				}
			}

			return base.OnMouseDown(button);
		}

		public void ButtonNextLevel_Click(UIButton sender)
		{
			// Get next level file name.
			string nextLevel;
			{
				var currentLevel = Component_Scene.First.HierarchyController.CreatedByResource.Owner.Name;
				var fileName = Path.GetFileName(currentLevel);
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