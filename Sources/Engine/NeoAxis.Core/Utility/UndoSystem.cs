// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents undo/redo system.
	/// </summary>
	public class UndoSystem
	{
		int maxLevel;
		List<Action> redoActions = new List<Action>();
		List<Action> undoActions = new List<Action>();

		///////////////////////////////////////////

		/// <summary>
		/// Represents an action of undo/redo system.
		/// </summary>
		public abstract class Action
		{
			protected internal abstract void DoUndo();
			protected internal abstract void DoRedo();

			protected internal abstract void Destroy();
		}

		///////////////////////////////////////////

		public event EventHandler ListOfActionsChanged;
		public event EventHandler WasCleaned;

		public delegate void ActionDelegate( Action action );
		public event ActionDelegate ActionUndo;
		public event ActionDelegate ActionRedo;
		public event ActionDelegate ActionDestroy;

		public UndoSystem( int maxLevel )
		{
			this.maxLevel = maxLevel;
		}

		//!!!!!вызывать. а надо ли
		public void Dispose()
		{
			Clear();
		}

		public int MaxLevel
		{
			get { return maxLevel; }
			set { maxLevel = value; }
		}

		public void Clear()
		{
			bool existsData = undoActions.Count != 0 || redoActions.Count != 0;

			foreach( Action action in redoActions )
			{
				action.Destroy();
				ActionDestroy?.Invoke( action );
			}
			redoActions.Clear();

			foreach( Action action in undoActions )
			{
				action.Destroy();
				ActionDestroy?.Invoke( action );
			}
			undoActions.Clear();

			if( existsData )
			{
				ListOfActionsChanged?.Invoke( this, EventArgs.Empty );
			}

			WasCleaned?.Invoke( this, EventArgs.Empty );
		}

		public void CommitAction( Action action )
		{
			foreach( Action a in redoActions )
			{
				a.Destroy();
				ActionDestroy?.Invoke( a );
			}
			redoActions.Clear();

			if( undoActions.Count + 1 >= maxLevel )
			{
				undoActions[ 0 ].Destroy();
				ActionDestroy?.Invoke( undoActions[ 0 ] );
				undoActions.RemoveAt( 0 );
			}

			undoActions.Add( action );

			ListOfActionsChanged?.Invoke( this, EventArgs.Empty );
		}

		public Action GetTopUndoAction()
		{
			if( undoActions.Count == 0 )
				return null;
			return undoActions[ undoActions.Count - 1 ];
		}

		public Action GetTopRedoAction()
		{
			if( redoActions.Count == 0 )
				return null;
			return redoActions[ redoActions.Count - 1 ];
		}

		public bool DoUndo()
		{
			if( undoActions.Count == 0 )
				return false;

			//remove from undo list
			Action action = undoActions[ undoActions.Count - 1 ];
			undoActions.RemoveAt( undoActions.Count - 1 );

			//do undo
			action.DoUndo();
			ActionUndo?.Invoke( action );

			//add to redo list
			if( redoActions.Count + 1 >= maxLevel )
			{
				redoActions[ 0 ].Destroy();
				ActionDestroy?.Invoke( redoActions[ 0 ] );
				redoActions.RemoveAt( 0 );
			}
			redoActions.Add( action );

			ListOfActionsChanged?.Invoke( this, EventArgs.Empty );

			return true;
		}

		public bool DoRedo()
		{
			if( redoActions.Count == 0 )
				return false;

			//remove from redo list
			Action action = redoActions[ redoActions.Count - 1 ];
			redoActions.RemoveAt( redoActions.Count - 1 );

			//do redo
			action.DoRedo();
			ActionRedo?.Invoke( action );

			//add to undo list
			if( undoActions.Count + 1 >= maxLevel )
			{
				undoActions[ 0 ].Destroy();
				ActionDestroy?.Invoke( undoActions[ 0 ] );
				undoActions.RemoveAt( 0 );
			}
			undoActions.Add( action );

			ListOfActionsChanged?.Invoke( this, EventArgs.Empty );

			return true;
		}

		public string[] DumpDebugToLines()
		{
			List<string> lines = new List<string>();

			lines.Add( "UndoSystem" );

			lines.Add( "" );
			lines.Add( "Undo actions:" );
			for( int n = 0; n < undoActions.Count; n++ )
				lines.Add( undoActions[ n ].ToString() );

			lines.Add( "" );
			lines.Add( "Redo actions:" );
			for( int n = redoActions.Count - 1; n >= 0; n-- )
				lines.Add( redoActions[ n ].ToString() );

			return lines.ToArray();
		}
	}
}
