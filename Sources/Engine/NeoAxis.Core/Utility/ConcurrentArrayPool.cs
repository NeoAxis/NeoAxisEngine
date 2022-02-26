// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	class ConcurrentArrayPool<T>
	{
		int arraysLength;
		Stack<T[]> stack = new Stack<T[]>();

		//

		public ConcurrentArrayPool( int arraysLength )
		{
			this.arraysLength = arraysLength;
		}

		public T[] Get()
		{
			lock( stack )
			{
				if( stack.Count == 0 )
					stack.Push( new T[ arraysLength ] );
				return stack.Pop();
			}
		}

		public void Free( T[] array )
		{
			lock( stack )
				stack.Push( array );
		}
	}
}
