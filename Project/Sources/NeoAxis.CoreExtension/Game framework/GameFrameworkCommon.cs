// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public interface IProcessDamage
	{
		void ProcessDamage( long whoFired, double damage, object anyData );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class NetworkLogicAbstract : Component
	{
		public virtual Component ServerGetObjectControlledByUser( ServerNetworkService_Users.UserInfo user, bool inputMustEnabled ) { return null; }
		public virtual ServerNetworkService_Users.UserInfo ServerGetUserByObjectControlled( Component obj, bool inputMustEnabled ) { return null; }
		public virtual void ServerChangeObjectControlled( ServerNetworkService_Users.UserInfo user, Component obj ) { }
	}

	public static class NetworkLogicUtility
	{
		public static NetworkLogicAbstract GetNetworkLogic( Component anyComponentInHierarchy )
		{
			return anyComponentInHierarchy.ParentRoot.GetComponent<NetworkLogicAbstract>();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum PhysicsModeEnum
	{
		None,
		Kinematic,
		Basic
	}
}
