// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Base class for objects that prepare a result.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Component_ResultCompile<T> : Component where T : class
	{
		T result;
		//ThreadSafeExchangeAny<T> result;
		ThreadSafeExchangeBool resultCompileProcessing;
		volatile ResultCompileUpdateModes resultCompileUpdateMode = ResultCompileUpdateModes.WhenEnabled;

		volatile bool shouldRecompile;

		/////////////////////////////////////////

		public enum ResultCompileUpdateModes
		{
			EachGet,
			WhenEnabled,
			Manual,
		}

		/////////////////////////////////////////

		/// <summary>
		/// Function for the implementation of the calculation of the result.
		/// </summary>
		protected virtual void OnResultCompile() { }
		//protected abstract void OnResultCompile();

		/// <summary>
		/// Performs recompilation of the result.
		/// </summary>
		public void ResultCompile()
		{
			if( resultCompileProcessing.Set( true ) )
				return;

			try
			{
				//!!!!а может как обновление вызывать? т.е. не полностью диспосить
				ResultDispose();

				ResultCompileBegin?.Invoke( this );
				OnResultCompile();
				ResultCompileEnd?.Invoke( this );

				shouldRecompile = false;
			}
			finally
			{
				resultCompileProcessing.Set( false );
			}
		}
		/// <summary>
		/// Occurs before compiling the result.
		/// </summary>
		public event Action<Component_ResultCompile<T>> ResultCompileBegin;
		/// <summary>
		/// Occurs after the completion of the compilation of the result.
		/// </summary>
		public event Action<Component_ResultCompile<T>> ResultCompileEnd;

		/////////////////////////////////////////

		internal override void OnDispose_After()
		{
			ResultDispose();

			base.OnDispose_After();
		}

		/// <summary>
		/// Clears the result.
		/// </summary>
		public virtual void ResultDispose()
		{
			T r = result;
			result = default;
			//T r = result.Set( default );
			if( r != null )
			{
				//!!!!проверить во всех ли нужных Dispose'ах есть мультипоточность

				IDisposable d = r as IDisposable;
				if( d != null )
					d.Dispose();
			}
		}

		/// <summary>
		/// The result provided by the object.
		/// </summary>
		[Browsable( false )]
		public T Result
		{
			get
			{
				//!!!!!как часто вызываться будет?
				//!!!!!а если не включена?

				//!!!!так?
				if( ResultCompileUpdateMode == ResultCompileUpdateModes.EachGet || ( shouldRecompile && EnabledInHierarchy ) )
					ResultCompile();

				return result;
				//return result.Get();
			}
			set
			{
				result = value;
				//result.Set( value );
				ResultChanged?.Invoke( this );
			}
		}
		/// <summary>
		/// Occurs after changing the result value.
		/// </summary>
		/// <summary>Occurs when the <see cref="Result"/> property value changes.</summary>
		public event Action<Component_ResultCompile<T>> ResultChanged;

		/// <summary>
		/// Mode update of the object.
		/// </summary>
		[Serialize]
		[DefaultValue( (int)ResultCompileUpdateModes.WhenEnabled )]
		[Browsable( false )]
		public virtual ResultCompileUpdateModes ResultCompileUpdateMode
		{
			get { return resultCompileUpdateMode; }
			set { resultCompileUpdateMode = value; }
		}

		/// <summary>
		/// Whether to update the resulting object data.
		/// </summary>
		[Browsable( false )]
		public bool ShouldRecompile
		{
			get { return shouldRecompile; }
			set { shouldRecompile = value; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				if( ResultCompileUpdateMode == ResultCompileUpdateModes.WhenEnabled )
					ResultCompile();
			}
			else
				ResultDispose();
		}
	}
}
