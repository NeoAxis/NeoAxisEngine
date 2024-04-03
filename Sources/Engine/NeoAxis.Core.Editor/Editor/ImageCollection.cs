#if !DEPLOY
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;

namespace NeoAxis.Editor
{
	class ImageCollection : CollectionBase, IList, ICollection, IEnumerable
	{
		class ImageInfo
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

		public Image this[ int index ]
		{
			get
			{
				if( index < 0 || index > base.Count - 1 )
					throw new ArgumentOutOfRangeException( "index" );
				return ( (ImageInfo)base.InnerList[ index ] ).Image;
				//return ( (ImageInfo)base.InnerList[ index ] ).Image.Clone() as Image;
			}
			set
			{
				if( index < 0 || index > base.Count - 1 )
					throw new ArgumentOutOfRangeException( "index" );
				if( value == null )
					throw new ArgumentNullException( "value" );
				( (ImageInfo)base.InnerList[ index ] ).Image = value;
			}
		}

		public Image this[ string key ]
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
				{
					image = imageInfo1.Image;
					//image = imageInfo1.Image.Clone() as Image;
				}
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
					string key = ( (ImageInfo)base.InnerList[ i ] ).Key;
					if( key != null )
						stringCollection.Add( key );
				}
				return stringCollection;
			}
		}

		object IList.this[ int index ]
		{
			get { return this[ index ]; }
			set
			{
				if( !( value is Image ) )
					throw new ArgumentException( "value" );
				this[ index ] = (Image)value;
			}
		}

		public void Add( Icon value )
		{
			if( value == null )
				throw new ArgumentNullException( "value" );
			base.InnerList.Add( new ImageInfo( value.ToBitmap(), null ) );
		}

		public void Add( Image value )
		{
			if( value == null )
				throw new ArgumentNullException( "value" );
			base.InnerList.Add( new ImageInfo( value, null ) );
		}

		public bool Contains( Image image )
		{
			if( image == null )
				throw new ArgumentNullException( "image" );
			foreach( ImageInfo inner in base.InnerList )
			{
				if( inner.Image == image )
					return true;
			}
			return false;
		}

		public int IndexOf( Image image )
		{
			if( image == null )
				throw new ArgumentNullException( "image" );
			for( int i = 0; i < base.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)base.InnerList[ i ] ).Image == image )
					return i;
			}
			return -1;
		}

		public void Remove( Image image )
		{
			if( image == null )
				throw new ArgumentNullException( "image" );
			int num = 0;
			while( true )
			{
				if( num >= base.InnerList.Count )
					return;
				if( ( (ImageInfo)base.InnerList[ num ] ).Image == image )
					break;
				num++;
			}
			base.InnerList.RemoveAt( num );
		}

		public void Add( string key, Icon icon )
		{
			base.InnerList.Add( new ImageInfo( icon.ToBitmap(), key ) );
		}

		public void Add( string key, Image image )
		{
			base.InnerList.Add( new ImageInfo( image, key ) );
		}

		public void AddRange( Image[] images )
		{
			if( images == null )
				throw new ArgumentNullException( "images" );
			foreach( var value in images )
				Add( value );
		}

		public bool ContainsKey( string key )
		{
			if( key == null )
				throw new ArgumentNullException( "image" );
			foreach( ImageInfo inner in base.InnerList )
			{
				if( inner.Key == key )
					return true;
			}
			return false;
		}

		public new IEnumerator GetEnumerator()
		{
			var array = new Image[ base.Count ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = this[ i ];
			return array.GetEnumerator();
		}

		public int IndexOfKey( string key )
		{
			if( key == null )
				throw new ArgumentNullException( "image" );
			for( int i = 0; i < base.InnerList.Count; i++ )
			{
				if( ( (ImageInfo)base.InnerList[ i ] ).Key == key )
					return i;
			}
			return -1;
		}

		public void RemoveByKey( string key )
		{
			if( key == null )
				throw new ArgumentNullException( "image" );
			int num = 0;
			while( true )
			{
				if( num >= base.InnerList.Count )
					return;
				if( ( (ImageInfo)base.InnerList[ num ] ).Key == key )
					break;
				num++;
			}
			base.InnerList.RemoveAt( num );
		}

		public void SetKeyName( int index, string name )
		{
			if( name == null )
				throw new ArgumentNullException( "name" );

			( (ImageInfo)base.InnerList[ index ] ).Key = name;
		}

		void ICollection.CopyTo( Array dest, int index )
		{
			for( int i = 0; i < base.Count; i++ )
				dest.SetValue( this[ i ], index++ );
		}

		int IList.Add( object value )
		{
			if( !( value is Image ) )
				throw new ArgumentException( "value" );
			Add( (Image)value );
			return base.Count - 1;
		}

		bool IList.Contains( object image )
		{
			if( image is Image )
				return Contains( (Image)image );
			return false;
		}

		int IList.IndexOf( object image )
		{
			if( image is Image )
				return IndexOf( (Image)image );
			return -1;
		}

		void IList.Insert( int index, object value )
		{
			if( !( value is Image ) )
				throw new ArgumentException( "value" );
			base.InnerList.Insert( index, new ImageInfo( (Image)value, null ) );
		}

		void IList.Remove( object image )
		{
			if( image is Image )
				Remove( (Image)image );
		}
	}
}

#endif