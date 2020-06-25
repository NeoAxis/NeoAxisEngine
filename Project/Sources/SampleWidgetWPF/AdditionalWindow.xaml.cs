using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NeoAxis;

namespace SampleWidgetWPF
{
	/// <summary>
	/// Interaction logic for AdditionalWindow.xaml
	/// </summary>
	public partial class AdditionalWindow : Window
	{
		public AdditionalWindow()
		{
			InitializeComponent();

			WidgetControl.InnerControl.ViewportCreated += WidgetControl1_ViewportCreated;
		}

		private void Button_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void WidgetControl1_ViewportCreated( NeoAxis.Widget.EngineViewportControl sender )
		{
			Component_Scene.First.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;

			//bind scene to viewport
			WidgetControl.InnerControl.Viewport.AttachedScene = Component_Scene.First;
		}

		protected virtual void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			if( WidgetControl.InnerControl.Viewport == viewport )
			{
				var defaultCamera = scene.CameraDefault.Value;
				if( defaultCamera == null )
					defaultCamera = scene.Mode.Value == Component_Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;

				var position = new Vector3( 27, 4, -9 );
				var lookTo = new Vector3( 25, -0.6, -10 );
				var up = Vector3.ZAxis;

				//var obj = scene.GetComponent<Component_ObjectInSpace>( "Mesh in Space 68" );
				//if( obj != null )
				//{
				//	var tr = obj.TransformV;
				//	position = tr.Position + tr.Rotation * new Vector3( 0.2, 0, 0.1 );
				//	lookTo = position + tr.Rotation * new Vector3( 1, 0, 0 );
				//	up = tr.Rotation * new Vector3( 0, 0, 1 );
				//}

				var camera = (Component_Camera)defaultCamera.Clone();
				//camera = new Component_Camera();
				camera.Transform = new NeoAxis.Transform( position, Quaternion.LookAt( ( lookTo - position ).GetNormalize(), up ) );
				camera.FixedUp = up;
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );

				processed = true;
			}
		}
	}
}
