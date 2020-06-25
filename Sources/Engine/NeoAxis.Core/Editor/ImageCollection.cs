using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Collection of images for ImageListAdv.
	/// </summary>
	[Serializable]
	[Editor(typeof(ImageCollectionEditor), typeof(UITypeEditor))]
	public class ImageCollection : CollectionBase, IList, ICollection, IEnumerable
	{
		private class CustomImageTypeDescriptor : CustomTypeDescriptor
		{
			public CustomImageTypeDescriptor( ICustomTypeDescriptor parent )
				: base( parent )
			{
			}

			public override PropertyDescriptorCollection GetProperties()
			{
				return FilterProperties( base.GetProperties() );
			}

			public override PropertyDescriptorCollection GetProperties( Attribute[] attributes )
			{
				return FilterProperties( base.GetProperties( attributes ) );
			}

			private static PropertyDescriptorCollection FilterProperties( PropertyDescriptorCollection props )
			{
				PropertyDescriptor obj = props["Tag"];
				ArrayList arrayList = new ArrayList( props );
				arrayList.Remove( obj );
				return new PropertyDescriptorCollection( (PropertyDescriptor[])arrayList.ToArray( typeof( PropertyDescriptor ) ), true );
			}
		}

		private class ImageTypeDescriptionProvider : TypeDescriptionProvider
		{
			public ImageTypeDescriptionProvider()
				: base( TypeDescriptor.GetProvider( typeof( Image ) ) )
			{
			}

			public override ICustomTypeDescriptor GetTypeDescriptor( Type objectType, object instance )
			{
				ICustomTypeDescriptor typeDescriptor = base.GetTypeDescriptor( objectType, instance );
				return new CustomImageTypeDescriptor( typeDescriptor );
			}
		}

#if !ANDROID
		internal class ImageCollectionEditor : CollectionEditor
		{
			private static ImageEditorAdv editor;

			private static TypeDescriptionProvider imageTypeDescProvider;

			static ImageCollectionEditor()
			{
				editor = new ImageEditorAdv();
				imageTypeDescProvider = new ImageTypeDescriptionProvider();
			}

			public ImageCollectionEditor( Type type )
				: base( type )
			{
			}

			protected override object CreateInstance( Type type )
			{
				return editor.EditValue( base.Context, null );
			}

			protected override IList GetObjectsFromInstance( object instance )
			{
				ArrayList arrayList = null;
				if( instance != null )
				{
					arrayList = new ArrayList();
					if( instance is Image )
					{
						arrayList.Add( instance );
					}
					else if( instance is IList )
					{
						arrayList.AddRange( (ICollection)instance );
					}
				}
				return arrayList;
			}

			protected override void DestroyInstance( object instance )
			{
				Image image = instance as Image;
				if( image != null )
					( base.Context.Instance as ImageListAdv )?.Images.Remove( image );
				base.DestroyInstance( instance );
			}

			public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
			{
				object result = null;
				try
				{
					TypeDescriptor.AddProvider( imageTypeDescProvider, typeof( Image ) );
					return result;
				}
				finally
				{
					result = base.EditValue( context, provider, value );
					TypeDescriptor.RemoveProvider( imageTypeDescProvider, typeof( Image ) );
				}
			}
		}

		[ComVisible( false )]
		[System.Security.SuppressUnmanagedCodeSecurity]
		internal class NativeMethods
		{
			[DllImport( "user32.dll", CharSet = CharSet.Auto )]
			internal static extern IntPtr GetFocus();

			[DllImport( "user32.dll", CharSet = CharSet.Auto )]
			internal static extern IntPtr SetFocus( IntPtr hWnd );

		}

		internal class ImageEditorAdv : ImageEditor
		{
			private OpenFileDialog fileDialog;

			private static Type[] imageExtenders;

			static ImageEditorAdv()
			{
				imageExtenders = new Type[2]
				{
					typeof(BitmapEditor),
					typeof(MetafileEditor)
				};
			}

			public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
			{
				ArrayList arrayList = new ArrayList();
				if( provider != null && (IWindowsFormsEditorService)provider.GetService( typeof( IWindowsFormsEditorService ) ) != null )
				{
					if( fileDialog == null )
					{
						fileDialog = new OpenFileDialog();
						fileDialog.Multiselect = true;
						string text = ImageEditor.CreateFilterEntry( this );
						for( int i = 0; i < imageExtenders.Length; i++ )
						{
							ImageEditor imageEditor = (ImageEditor)Activator.CreateInstance( imageExtenders[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null );
							Type type = GetType();
							Type type2 = imageEditor.GetType();
							if( !type.Equals( type2 ) && imageEditor != null && type.IsInstanceOfType( imageEditor ) )
							{
								text = text + "|" + ImageEditor.CreateFilterEntry( imageEditor );
							}
						}
						fileDialog.Filter = text;
					}
					IntPtr focus = NativeMethods.GetFocus();
					try
					{
						if( fileDialog.ShowDialog() != DialogResult.OK )
						{
							return arrayList;
						}
						string[] fileNames = fileDialog.FileNames;
						foreach( string text2 in fileNames )
						{
							bool flag = false;
							if( Path.GetExtension( text2 ) == ".ico" )
							{
								try
								{
									Icon icon = new Icon( text2 );
									arrayList.Add( ImageListAdv.IconToImageAlphaCorrect( icon ) );
									flag = true;
								}
								catch( ArgumentException )
								{
								}
							}
							if( !flag )
							{
								FileStream stream = new FileStream( text2, FileMode.Open, FileAccess.Read, FileShare.Read );
								arrayList.Add( LoadFromStream( stream ) );
							}
						}
						return arrayList;
					}
					finally
					{
						if( focus != IntPtr.Zero )
						{
							NativeMethods.SetFocus( focus );
						}
					}
				}
				return arrayList;
			}
		}
#else //ANDROID
		internal class ImageCollectionEditor 
		{
		}

#endif //ANDROID

			private class ImageInfo
		{
			public Image Image { get; set; }

			public string Key { get; set; }

			public ImageInfo( Image image, string key )
			{
				Image = image;
				Key = key;
			}
		}

		public bool Empty => base.Count == 0;

		public bool IsReadOnly => false;

		public Image this[int index]
		{
			get
			{
				if( index < 0 || index > base.Count - 1 )
				{
					throw new ArgumentOutOfRangeException( "index" );
				}
				return ( (ImageInfo)base.InnerList[index] ).Image.Clone() as Image;
			}
			set
			{
				if( index < 0 || index > base.Count - 1 )
				{
					throw new ArgumentOutOfRangeException( "index" );
				}
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}
				( (ImageInfo)base.InnerList[index] ).Image = value;
			}
		}

		public Image this[string key]
		{
			get
			{
				ImageInfo imageInfo1 = null;
				foreach( ImageInfo imageInfo2 in InnerList )
				{
					if( imageInfo2.Key == key )
					{
						imageInfo1 = imageInfo2;
						break;
					}
				}
				Image image = null;
				if( imageInfo1 != null )
					image = imageInfo1.Image.Clone() as Image;
				return image;
			}
			set
			{
				ImageInfo imageInfo1 = null;
				foreach( ImageInfo imageInfo2 in InnerList )
				{
					if( imageInfo2.Key == key )
					{
						imageInfo1 = imageInfo2;
						break;
					}
				}
				if( imageInfo1 != null )
					imageInfo1.Image = value;
				else
					this.Add( key, value );
			}
		}

		public StringCollection Keys
		{
			get
			{
				StringCollection stringCollection = new StringCollection();
				for( int i = 0; i < base.Count; i++ )
				{
					string key = ( (ImageInfo)base.InnerList[i] ).Key;
					if( key != null )
					{
						stringCollection.Add( key );
					}
				}
				return stringCollection;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if( !( value is Image ) )
				{
					throw new ArgumentException( "value" );
				}
				this[index] = (Image)value;
			}
		}

		public void Add( Icon value )
		{
			if( value == null )
			{
				throw new ArgumentNullException( "value" );
			}
			base.InnerList.Add( new ImageInfo( ImageListAdv.IconToImageAlphaCorrect( value ), null ) );
		}

		public void Add( Image value )
		{
			if( value == null )
			{
				throw new ArgumentNullException( "value" );
			}
			base.InnerList.Add( new ImageInfo( value, null ) );
		}

		public bool Contains( Image image )
		{
			if( image == null )
			{
				throw new ArgumentNullException( "image" );
			}
			foreach( ImageInfo inner in base.InnerList )
			{
				if( inner.Image == image )
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOf( Image image )
		{
			if( image == null )
			{
				throw new ArgumentNullException( "image" );
			}
			for( int i = 0; i < base.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)base.InnerList[i] ).Image == image )
				{
					return i;
				}
			}
			return -1;
		}

		public void Remove( Image image )
		{
			if( image == null )
			{
				throw new ArgumentNullException( "image" );
			}
			int num = 0;
			while( true )
			{
				if( num >= base.InnerList.Count )
				{
					return;
				}
				if( ( (ImageInfo)base.InnerList[num] ).Image == image )
				{
					break;
				}
				num++;
			}
			base.InnerList.RemoveAt( num );
		}

		public void Add( string key, Icon icon )
		{
			base.InnerList.Add( new ImageInfo( ImageListAdv.IconToImageAlphaCorrect( icon ), key ) );
		}

		public void Add( string key, Image image )
		{
			base.InnerList.Add( new ImageInfo( image, key ) );
		}

		public void AddRange( Image[] images )
		{
			if( images == null )
			{
				throw new ArgumentNullException( "images" );
			}
			foreach( Image value in images )
			{
				Add( value );
			}
		}

		public bool ContainsKey( string key )
		{
			if( key == null )
			{
				throw new ArgumentNullException( "image" );
			}
			foreach( ImageInfo inner in base.InnerList )
			{
				if( inner.Key == key )
				{
					return true;
				}
			}
			return false;
		}

		public new IEnumerator GetEnumerator()
		{
			Image[] array = new Image[base.Count];
			for( int i = 0; i < array.Length; i++ )
			{
				array[i] = this[i];
			}
			return array.GetEnumerator();
		}

		public int IndexOfKey( string key )
		{
			if( key == null )
			{
				throw new ArgumentNullException( "image" );
			}
			for( int i = 0; i < base.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)base.InnerList[i] ).Key == key )
				{
					return i;
				}
			}
			return -1;
		}

		public void RemoveByKey( string key )
		{
			if( key == null )
			{
				throw new ArgumentNullException( "image" );
			}
			int num = 0;
			while( true )
			{
				if( num >= base.InnerList.Count )
				{
					return;
				}
				if( ( (ImageInfo)base.InnerList[num] ).Key == key )
				{
					break;
				}
				num++;
			}
			base.InnerList.RemoveAt( num );
		}

		public void SetKeyName( int index, string name )
		{
			if( name == null )
				throw new ArgumentNullException( "name" );

			( (ImageInfo)base.InnerList[index] ).Key = name;
		}

		void ICollection.CopyTo( Array dest, int index )
		{
			for( int i = 0; i < base.Count; i++ )
			{
				dest.SetValue( this[i], index++ );
			}
		}

		int IList.Add( object value )
		{
			if( !( value is Image ) )
			{
				throw new ArgumentException( "value" );
			}
			Add( (Image)value );
			return base.Count - 1;
		}

		bool IList.Contains( object image )
		{
			if( image is Image )
			{
				return Contains( (Image)image );
			}
			return false;
		}

		int IList.IndexOf( object image )
		{
			if( image is Image )
			{
				return IndexOf( (Image)image );
			}
			return -1;
		}

		void IList.Insert( int index, object value )
		{
			if( !( value is Image ) )
			{
				throw new ArgumentException( "value" );
			}
			base.InnerList.Insert( index, new ImageInfo( (Image)value, null ) );
		}

		void IList.Remove( object image )
		{
			if( image is Image )
			{
				Remove( (Image)image );
			}
		}
	}
}

//#else

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Drawing;

//namespace NeoAxis.Editor
//{
//	public class ImageCollection : CollectionBase, IList, ICollection, IEnumerable
//	{
//		public Image this[ int index ]
//		{
//			get
//			{
//				throw new NotImplementedException();
//			}
//		}

//		public Image this[ string index ]
//		{
//			get
//			{
//				throw new NotImplementedException();
//			}
//		}

//		public IEnumerable<string> Keys { get; internal set; }

//		internal void Add( string key, Image bitmap )
//		{
//			throw new NotImplementedException();
//		}

//		internal void Add( Image bitmap )
//		{
//			throw new NotImplementedException();
//		}

//		internal void Add( Bitmap bitmap )
//		{
//			throw new NotImplementedException();
//		}

//		internal void AddRange( Image[] array )
//		{
//			throw new NotImplementedException();
//		}

//		internal bool ContainsKey( string key )
//		{
//			throw new NotImplementedException();
//		}

//		internal int IndexOfKey( object key )
//		{
//			throw new NotImplementedException();
//		}

//		internal void SetKeyName( object p, object key )
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
