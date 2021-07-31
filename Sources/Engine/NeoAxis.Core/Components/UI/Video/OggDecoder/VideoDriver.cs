// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using NeoAxis;

using NeoAxis.OggVorbisTheora;

namespace OggDecoder
{
	abstract class VideoDriver : IDisposable
	{
		internal OggFile oggFile;

		theora.Info theoraInfo;
		theora.Comment theoraComment;
		ogg.StreamState theoraStreamState;
		theora.State theoraState;

		ogg.Packet oggPacket;

		int headerCount;

		theora.YUVBuffer yuvBuffer;
		bool yuvBufferFilled;

		//

		public VideoDriver()
		{
			theoraInfo = new theora.Info();
			theoraComment = new theora.Comment();
			theoraState = new theora.State();
			yuvBuffer = new theora.YUVBuffer();
			oggPacket = new ogg.Packet();
		}

		public virtual void Dispose()
		{
			if( oggPacket != null )
			{
				oggPacket.Dispose();
				oggPacket = null;
			}

			if( yuvBuffer != null )
			{
				yuvBuffer.Dispose();
				yuvBuffer = null;
			}

			if( theoraState != null )
			{
				theoraState.Dispose();
				theoraState = null;
			}

			if( theoraInfo != null )
			{
				theoraInfo.Dispose();
				theoraInfo = null;
			}

			if( theoraComment != null )
			{
				theoraComment.Dispose();
				theoraComment = null;
			}

			if( theoraStreamState != null )
			{
				theoraStreamState.Dispose();
				theoraStreamState = null;
			}

			GC.SuppressFinalize( this );
		}

		protected theora.YUVBuffer YUVBuffer
		{
			get { return yuvBuffer; }
		}

		//protected abstract void OnInit( Vec2i size );

		protected abstract void OnBlit();

		/// <summary>
		/// Blit only from main thread
		/// </summary>
		public void Blit()
		{
			if( yuvBufferFilled )
			{
				OnBlit();
				yuvBufferFilled = false;
			}
		}

		internal void InitTheora()
		{
			theoraState.decode_init( theoraInfo );
		}

		internal void PageIn( ogg.Page page )
		{
			theoraStreamState.pagein( page );
		}

		internal bool DecodePrimaryHeader( ogg.Packet p, ogg.StreamState s )
		{
			if( theoraInfo.decode_header( theoraComment, p ) >= 0 )
			{
				//This is the Theora stream, so store it
				theoraStreamState = s;
				headerCount = 1;
				return true;
			}
			return false;
		}

		internal bool CheckSecondHeader( ogg.Packet p, ref bool needMoreData )
		{
			needMoreData = false;
			if( headerCount == 3 )
			{
				//Call to allow driver to setup any needed structures
				//OnInit( new Vec2i( (int)theoraInfo.width, (int)theoraInfo.height ) );
				headerCount = 4;
				return false;
			}
			else if( headerCount == 4 )
				return false;

			int retVal = theoraStreamState.packetout( p );
			if( retVal == 1 )
			{
				if( theoraInfo.decode_header( theoraComment, p ) != 0 )
					throw new Exception( "invalid stream" );

				headerCount++;
				return true;
			}
			else if( retVal < 0 ) //error
				throw new Exception( "invalid stream" );

			//Need more data, so return true
			needMoreData = true;
			return true;
		}

		internal bool Decode()
		{
			if( theoraStreamState.packetout( oggPacket ) > 0 )
			{
				theoraState.decode_packetin( oggPacket );
				theoraState.decode_YUVout( yuvBuffer );
				yuvBufferFilled = true;
				return true;
			}
			else
				return false;
		}

		internal float GetCurrentStateTime()
		{
			return (float)theoraState.granule_time( theoraState.granulepos );
		}

		internal Vector2I GetSize()
		{
			if( theoraInfo == null )
				return Vector2I.Zero;
			return new Vector2I( (int)theoraInfo.width, (int)theoraInfo.height );
		}

		internal float GetFPS()
		{
			if( theoraInfo == null )
				return 0;
			return (float)( (double)theoraInfo.fps_numerator / (double)theoraInfo.fps_denominator );
		}

	}
}
