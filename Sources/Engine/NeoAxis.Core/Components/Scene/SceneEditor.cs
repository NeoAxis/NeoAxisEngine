//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace NeoAxis.Editor
{
	public class SceneEditorGetMouseOverObjectToSelectByClickContext
	{
		public bool CheckOnlyObjectsWithEnabledSelectionByCursorFlag = true;

		public object ResultObject;//public ObjectInSpace ResultObject;
		public Vector3? ResultPosition;
		public Viewport.LastFrameScreenLabelItem ScreenLabelItem;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class SceneEditorWorkareaMode : DocumentWindowWithViewportWorkareaMode
	{
		protected SceneEditorWorkareaMode( ISceneEditor documentWindow )
			: base( documentWindow )
		{
		}

		public new ISceneEditor DocumentWindow
		{
			get { return (ISceneEditor)base.DocumentWindow; }
		}

		protected virtual bool OnGetObjectsToSelectByRectangle( Rectangle rectangle, ref List<object> objectsToSelect ) { return false; }
		public delegate void GetObjectsToSelectByRectangleDelegate( SceneEditorWorkareaMode sender, Rectangle rectangle, ref bool handled, ref List<object> objectsToSelect );
		public event GetObjectsToSelectByRectangleDelegate GetObjectsToSelectByRectangle;
		internal bool PerformGetObjectsToSelectByRectangle( Rectangle rectangle, ref List<object> objectsToSelect )
		{
			var handled = OnGetObjectsToSelectByRectangle( rectangle, ref objectsToSelect );
			if( !handled )
				GetObjectsToSelectByRectangle?.Invoke( this, rectangle, ref handled, ref objectsToSelect );
			return handled;
		}

		protected virtual bool OnGetMouseOverObjectToSelectByClick( SceneEditorGetMouseOverObjectToSelectByClickContext context ) { return false; }
		public delegate void GetMouseOverObjectToSelectByClickDelegate( SceneEditorWorkareaMode sender, SceneEditorGetMouseOverObjectToSelectByClickContext context );
		public event GetMouseOverObjectToSelectByClickDelegate GetMouseOverObjectToSelectByClick;
		internal bool PerformGetMouseOverObjectToSelectByClick( SceneEditorGetMouseOverObjectToSelectByClickContext context )
		{
			var handled = OnGetMouseOverObjectToSelectByClick( context );
			if( !handled )
				GetMouseOverObjectToSelectByClick?.Invoke( this, context );
			return handled;
		}

		protected virtual bool OnTransformToolCreateObject( object forObject, ref TransformToolObject transformToolObject ) { return false; }
		public delegate void TransformToolCreateObjectDelegate( SceneEditorWorkareaMode sender, object forObject, ref bool handled, ref TransformToolObject transformToolObject );
		public event TransformToolCreateObjectDelegate TransformToolCreateObject;
		internal bool PerformTransformToolCreateObject( object forObject, ref TransformToolObject transformToolObject )
		{
			var handled = OnTransformToolCreateObject( forObject, ref transformToolObject );
			if( !handled )
				TransformToolCreateObject?.Invoke( this, forObject, ref handled, ref transformToolObject );
			return handled;
		}

		protected virtual bool OnTransformToolModifyBegin() { return false; }
		public delegate void TransformToolModifyBeginDelegate( SceneEditorWorkareaMode sender, ref bool handled );
		public event TransformToolModifyBeginDelegate TransformToolModifyBegin;
		internal bool PerformTransformToolModifyBegin()
		{
			var handled = OnTransformToolModifyBegin();
			if( !handled )
				TransformToolModifyBegin?.Invoke( this, ref handled );
			return handled;
		}

		protected virtual bool OnTransformToolModifyCommit() { return false; }
		public delegate void TransformToolModifyCommitDelegate( SceneEditorWorkareaMode sender, ref bool handled );
		public event TransformToolModifyCommitDelegate TransformToolModifyCommit;
		internal bool PerformTransformToolModifyCommit()
		{
			var handled = OnTransformToolModifyCommit();
			if( !handled )
				TransformToolModifyCommit?.Invoke( this, ref handled );
			return handled;
		}

		protected virtual bool OnTransformToolModifyCancel() { return false; }
		public delegate void TransformToolModifyCancelDelegate( SceneEditorWorkareaMode sender, ref bool handled );
		public event TransformToolModifyCancelDelegate TransformToolModifyCancel;
		internal bool PerformTransformToolModifyCancel()
		{
			var handled = OnTransformToolModifyCancel();
			if( !handled )
				TransformToolModifyCancel?.Invoke( this, ref handled );
			return handled;
		}

		protected virtual bool OnTransformToolCloneAndSelectObjects() { return false; }
		public delegate void TransformToolCloneAndSelectObjectsDelegate( SceneEditorWorkareaMode sender, ref bool handled );
		public event TransformToolCloneAndSelectObjectsDelegate TransformToolCloneAndSelectObjects;
		internal bool PerformTransformToolCloneAndSelectObjects()
		{
			var handled = OnTransformToolCloneAndSelectObjects();
			if( !handled )
				TransformToolCloneAndSelectObjects?.Invoke( this, ref handled );
			return handled;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class SceneEditorUtility
	{
		public delegate void CreateObjectWhatTypeWillCreatedEventDelegate( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type );
		public static event CreateObjectWhatTypeWillCreatedEventDelegate CreateObjectWhatTypeWillCreatedEvent;

		internal static void PerformCreateObjectWhatTypeWillCreatedEvent( Metadata.TypeInfo objectType, string referenceToObject, ref Metadata.TypeInfo type )
		{
			CreateObjectWhatTypeWillCreatedEvent?.Invoke( objectType, referenceToObject, ref type );
		}


		public delegate void CreateObjectByCreationDataEventDelegate( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject );
		public static event CreateObjectByCreationDataEventDelegate CreateObjectByCreationDataEvent;

		internal static void PerformCreateObjectByCreationDataEvent( Metadata.TypeInfo objectType, string referenceToObject, object anyData, Component createTo, ref Component newObject )
		{
			CreateObjectByCreationDataEvent?.Invoke( objectType, referenceToObject, anyData, createTo, ref newObject );
		}
	}
}
//#endif