// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// An instance of flashlight item.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Item\Flashlight", 550 )]
	[NewObjectDefaultName( "Flashlight" )]
	public class Flashlight : Item
	{
		Light lightCached;

		//

		public enum ActiveEnum
		{
			Auto,
			False,
			True,
		}

		/// <summary>
		/// Whether to active.
		/// </summary>
		[DefaultValue( ActiveEnum.Auto )]
		public Reference<ActiveEnum> Active
		{
			get { if( _active.BeginGet() ) Active = _active.Get( this ); return _active.value; }
			set { if( _active.BeginSet( this, ref value ) ) { try { ActiveChanged?.Invoke( this ); } finally { _active.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Active"/> property value changes.</summary>
		public event Action<Flashlight> ActiveChanged;
		ReferenceField<ActiveEnum> _active = ActiveEnum.Auto;

		///////////////////////////////////////////////

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			ItemType = new ReferenceNoValue( @"Content\Items\NeoAxis\Flashlight\Flashlight.itemtype" );
		}

		public bool IsActive()
		{
			var active = Active.Value;
			if( active == ActiveEnum.Auto )
			{
				if( Parent != null && Parent is Character )
					return true;
				else
					return false;
			}
			return active == ActiveEnum.True;
		}

		[Browsable( false )]
		public FlashlightType FlashlightType
		{
			get { return TypeCached as FlashlightType; }
		}

		void CreateLight()
		{
			lightCached = null;
			foreach( var old in GetComponents<Light>() )
				old.RemoveFromParent( false );

			var type = FlashlightType;
			if( type != null )
			{
				var lightInType = type.GetComponent<Light>();
				if( lightInType != null )
				{
					var light = lightInType.Clone() as Light;
					if( light != null )
					{
						light.Enabled = false;
						light.SaveSupport = false;
						light.CanBeSelected = false;
						AddComponent( light );

						var transformOffset = ObjectInSpaceUtility.Attach( this, light, TransformOffset.ModeEnum.Elements );
						if( transformOffset != null )
						{
							var trType = lightInType.TransformV;
							transformOffset.PositionOffset = trType.Position;
							transformOffset.RotationOffset = trType.Rotation;
							transformOffset.ScaleOffset = trType.Scale;

							////update space bounds after changes settings of attachment
							//light.SpaceBoundsUpdate();
						}

						light.Enabled = true;
					}
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
				CreateLight();
		}

		protected override void OnTypeChanged()
		{
			base.OnTypeChanged();

			CreateLight();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( lightCached == null )
				lightCached = GetComponent<Light>();
			if( lightCached != null )
			{
				lightCached.Enabled = IsActive();

				var type = FlashlightType;
				if( type != null )
					ReplaceMaterial = lightCached.Enabled ? null : type.InactiveMaterial;
			}
		}
	}
}
