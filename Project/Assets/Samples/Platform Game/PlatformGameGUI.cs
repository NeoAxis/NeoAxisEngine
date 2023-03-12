using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.IO;
using NeoAxis;

namespace Project
{
	public class PlatformGameGUI : NeoAxis.UIControl
	{
        protected override void OnEnabledInSimulation()
        {
            base.OnEnabledInSimulation();
            
            //hide GUI controls on PC
            if(!SystemSettings.MobileDevice)
            {
				var controlNames = new string[] { "Left", "Right", "Jump", "Fire" }; 
            	
            	foreach(var controlName in controlNames)
            	{
					var control = GetComponent(controlName) as UIControl;
					if(control != null)
						control.Enabled = false;
				}
			}
        }

        
		bool IsAnyWindowOpened()
		{
			return ParentRoot.GetComponent<UIWindow>(true, true) != null;
		}

		bool IsInsideControl(string controlName, Vector2 screenPosition)
		{
			var control = GetComponent(controlName) as UIControl;
			if (control != null)
				return control.GetScreenRectangle().Contains(screenPosition);
			return false;
		}

		GameMode GetGameMode()
		{
			return Scene.First.GetComponent("Game Mode") as GameMode;
		}

		Character2DInputProcessing GetInputProcessing()
		{
			return Scene.First.GetComponentByPath(@"Character 2D\Character Input Processing") as Character2DInputProcessing;
		}

		void ProcessLeftRight()
		{
			var gameMode = GetGameMode();
			var inputProcessing = GetInputProcessing();

			if (gameMode != null && inputProcessing != null)
			{
				var leftPushed = false;
				var rightPushed = false;

				foreach (var pointer in inputProcessing.TouchPointers)
				{
					if (IsInsideControl("Left", pointer.Position))
						leftPushed = true;

					if (IsInsideControl("Right", pointer.Position))
						rightPushed = true;
				}
				
				if (leftPushed)
					inputProcessing.PerformMessage(gameMode, new InputMessageKeyDown(EKeys.Left));
				else
					inputProcessing.PerformMessage(gameMode, new InputMessageKeyUp(EKeys.Left));

				if (rightPushed)
					inputProcessing.PerformMessage(gameMode, new InputMessageKeyDown(EKeys.Right));
				else
					inputProcessing.PerformMessage(gameMode, new InputMessageKeyUp(EKeys.Right));
			}
		}

		void ProcessJump(TouchData e)
		{
			var gameMode = GetGameMode();
			var inputProcessing = GetInputProcessing();

			if (gameMode != null && inputProcessing != null)
			{
				if (e.Action == TouchData.ActionEnum.Down && IsInsideControl("Jump", e.Position))
					inputProcessing.PerformMessage(gameMode, new InputMessageKeyDown(EKeys.Space));
			}
		}

		void ProcessFire()
		{
			var gameMode = GetGameMode();
			var inputProcessing = GetInputProcessing();

			if (gameMode != null && inputProcessing != null)
			{
				var pushed = false;

				foreach (var pointer in inputProcessing.TouchPointers)
				{
					if (IsInsideControl("Fire", pointer.Position))
						pushed = true;
				}

				if (pushed)
					inputProcessing.PerformMessage(gameMode, new InputMessageMouseButtonDown(EMouseButtons.Left));
				else
					inputProcessing.PerformMessage(gameMode, new InputMessageMouseButtonUp(EMouseButtons.Left));
			}
		}

		void ProcessAutoTake()
		{
			var gameMode = GetGameMode();
			var inputProcessing = GetInputProcessing();

			if (gameMode != null && inputProcessing != null)
			{
				//get an object to interaction
				var interactionContext = gameMode.ObjectInteractionContext;
				if (interactionContext != null)
				{
					//call input message to the object in context
					var message = new InputMessageMouseButtonDown(EMouseButtons.Left);
					interactionContext.Obj.ObjectInteractionInputMessage(gameMode, message);
				}
			}
		}

		protected override bool OnTouch(TouchData e)
		{
			if (!IsAnyWindowOpened())
			{
				ProcessLeftRight();
				ProcessJump(e);
				ProcessFire();
			}

			return base.OnTouch(e);
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if (!IsAnyWindowOpened())
			{
				ProcessLeftRight();
				ProcessFire();

				if (SystemSettings.MobileDevice)
					ProcessAutoTake();
			}
		}
	}
}