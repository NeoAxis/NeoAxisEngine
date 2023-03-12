//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	/// <summary>
//	/// A dialogue creation component.
//	/// </summary>
//	//!!!!
//	//[AddToResourcesWindow( @"Base\Game framework\Dialogue Tree", -4000 )]
//	//[NewObjectDefaultName( "Dialogue Tree" )]
//	public class DialogueTree : Component
//	{
//		public Message currentMessage;

//		public delegate void MessageFunction( Message message );

//		/////////////////////////////////////////

//		public class Message
//		{
//			public Message PreviousMessage;
//			public Answer PreviousAnswer;

//			public string Text = "";
//			public List<Answer> Answers = new List<Answer>();

//			////////////

//			public class Answer
//			{
//				public string Text = "";
//				public MessageFunction NextMessage;
//				public object Parameter;

//				public Answer( string text, MessageFunction nextMessage = null, object parameter = null )
//				{
//					Text = text;
//					NextMessage = nextMessage;
//					Parameter = parameter;
//				}
//			}

//			////////////

//			public Answer AddAnswer( string text, MessageFunction nextMessage = null, object parameter = null )
//			{
//				var answer = new Answer( text, nextMessage, parameter );
//				Answers.Add( answer );
//				return answer;
//			}
//		}

//		/////////////////////////////////////////

//		//!!!!zzzzz;//какое сообщение

//		//!!!!zzzzz;//как сериализовать

//		//!!!!zzzzz;//single, multiplayer. оно только на сервере

//		void Hello( Message message )
//		{
//			message.Text = "Hello!";

//			message.AddAnswer( "Hello! Who are you?", WhoAreYou );
//			message.AddAnswer( "Bye", null );
//		}

//		void WhoAreYou( Message message )
//		{
//			message.Text = "I am Jorge.";

//			message.AddAnswer( "Bye", null );
//		}

//		public void Start( MessageFunction startFunction = null )
//		{
//			var startFunction2 = startFunction ?? Hello;

//			//!!!!zzzzz;
//		}



//		////!!!!
//		////!!!!как сериализровать
//		//public class DialogueInstance
//		//{
//		//	public string 

//		//	public object AnyData;
//		//}

//		//void Hello( DialogueInstance instance )
//		//{
//		//	instance.SetMessage( "Hello!" );

//		//	instance.AddAnswer( "Hello! Who are you?", WhoAreYou );
//		//	instance.AddAnswer( "Bye", null );
//		//}

//		//void WhoAreYou( DialogueInstance instance )
//		//{
//		//	instance.SetMessage( "I am Jorge." );

//		//	instance.AddAnswer( "Bye", null );
//		//}


//	}
//}