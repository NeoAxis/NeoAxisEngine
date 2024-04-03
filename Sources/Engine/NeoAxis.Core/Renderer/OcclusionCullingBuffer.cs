// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	public class OcclusionCullingBuffer : IDisposable
	{
		IntPtr nativeObject;
		Vector2I size;
		bool ortho;

		///////////////////////////////////////////////

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_Create", CallingConvention = NativeUtility.convention )]
		static extern IntPtr MaskedOcclusionCulling_Create();

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_Destroy", CallingConvention = NativeUtility.convention )]
		static extern void MaskedOcclusionCulling_Destroy( IntPtr instance );

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_Init", CallingConvention = NativeUtility.convention )]
		static extern void MaskedOcclusionCulling_Init( IntPtr instance, int width, int height, [MarshalAs( UnmanagedType.U1 )] bool ortho );

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_ClearBuffer", CallingConvention = NativeUtility.convention )]
		static extern void MaskedOcclusionCulling_ClearBuffer( IntPtr instance );

		[Flags]
		public enum CullingResult
		{
			Visible = 0x0,
			Occluded = 0x1,
			ViewCulled = 0x3
		};

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_RenderTriangles", CallingConvention = NativeUtility.convention )]
		static unsafe extern CullingResult MaskedOcclusionCulling_RenderTriangles( IntPtr instance, float* inVtx, uint* inTris, int nTris, float* modelToClipMatrix/*, [MarshalAs( UnmanagedType.U1 )] bool stride16*/ );

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_TestTriangles", CallingConvention = NativeUtility.convention )]
		static unsafe extern CullingResult MaskedOcclusionCulling_TestTriangles( IntPtr instance, float* inVtx, uint* inTris, int nTris, float* modelToClipMatrix );

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_TestRect", CallingConvention = NativeUtility.convention )]
		static extern CullingResult MaskedOcclusionCulling_TestRect( IntPtr instance, float xmin, float ymin, float xmax, float ymax, float wmin );

		[DllImport( NativeUtility.library, EntryPoint = "MaskedOcclusionCulling_ComputePixelDepthBuffer", CallingConvention = NativeUtility.convention )]
		static unsafe extern void MaskedOcclusionCulling_ComputePixelDepthBuffer( IntPtr instance, float* depthData );

		///////////////////////////////////////////////

		public static bool Supported
		{
			get
			{
				//!!!!
				return
					SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
					SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP;
			}
		}

		public static OcclusionCullingBuffer Create()
		{
			NativeUtility.LoadUtilsNativeWrapperLibrary();

			var obj = new OcclusionCullingBuffer();
			obj.nativeObject = MaskedOcclusionCulling_Create();
			return obj;
		}

		public void Dispose()
		{
			if( nativeObject != IntPtr.Zero )
			{
				MaskedOcclusionCulling_Destroy( nativeObject );
				nativeObject = IntPtr.Zero;
			}
		}

		public void Init( Vector2I size, bool ortho )
		{
			this.size = size;
			this.ortho = ortho;
			MaskedOcclusionCulling_Init( nativeObject, size.X, size.Y, ortho );
		}

		public void ClearBuffer()
		{
			MaskedOcclusionCulling_ClearBuffer( nativeObject );
		}

		public unsafe CullingResult RenderTriangles( float* inVtx, uint* inTris, int nTris, float* modelToClipMatrix )//, bool stride16 )
		{
			return MaskedOcclusionCulling_RenderTriangles( nativeObject, inVtx, inTris, nTris, modelToClipMatrix );//, stride16 );
		}

		public unsafe CullingResult TestTriangles( float* inVtx, uint* inTris, int nTris, float* modelToClipMatrix )
		{
			return MaskedOcclusionCulling_TestTriangles( nativeObject, inVtx, inTris, nTris, modelToClipMatrix );
		}

		public unsafe CullingResult TestRect( float xmin, float ymin, float xmax, float ymax, float wmin )
		{
			return MaskedOcclusionCulling_TestRect( nativeObject, xmin, ymin, xmax, ymax, wmin );
		}

		public unsafe void ComputePixelDepthBuffer( float* depthData )
		{
			MaskedOcclusionCulling_ComputePixelDepthBuffer( nativeObject, depthData );
		}

		public Vector2I Size
		{
			get { return size; }
		}

		public bool Ortho
		{
			get { return ortho; }
		}

		public static Vector2I GetSizeByHeight( Vector2I viewportSize, int sizeParameter )
		{
			var sizeParameter2 = Math.Min( sizeParameter, viewportSize.Y );

			var aspect = (float)viewportSize.X / (float)viewportSize.Y;

			var demandedSize = new Vector2I( (int)( (float)sizeParameter2 * aspect ), sizeParameter2 );
			if( demandedSize.X % 8 != 0 )
				demandedSize.X = demandedSize.X / 8 * 8 + 8;
			if( demandedSize.Y % 4 != 0 )
				demandedSize.Y = demandedSize.X / 4 * 4 + 4;

			return demandedSize;
		}

		public IntPtr NativeObject
		{
			get { return nativeObject; }
		}
	}
}
