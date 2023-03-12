//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace NeoAxis
//{
//	/// <summary>
//	/// Class for calculation math expressions defined as string.
//	/// </summary>
//	/// <remarks>
//	/// <para>
//	/// Features:
//	/// - Operators: * / + - ^.
//	/// - Brackets support.
//	/// - Basic math functions: Abs( x ), Acos( x ), Asin( x ), Atan( x ), Atan2( y, x ), Ceiling( x ), Cos( x ), Cosh( x ), Exp( x ), Floor( x ), Log( x ), Log10( x ), Max( v1, v2 ), Min( v1, v2 ), Pow( x, y ), Round( x ), Sign( x ), Sin( x ), Sqrt( x ), Tan( x ), Tanh( x ).
//	/// - User defined functions.
//	/// - Parameters.
//	/// </para>
//	/// <para>
//	/// Examples:
//	/// "5 + 3 * 2"
//	/// "(5 + 3) * -2"
//	/// "x * (5 + y)"
//	/// "Pow(2, 3) + Sin(1)"
//	/// </para>
//	/// </remarks>
//	public class ExpressionCalculator
//	{
//		Dictionary<string, FunctionDefinition> functions = new Dictionary<string, FunctionDefinition>();

//		///////////////////////////////////////////

//		/// <summary>
//		/// Represents a function definition for <see cref="ExpressionCalculator"/>.
//		/// </summary>
//		public sealed class FunctionDefinition
//		{
//			internal string name;
//			internal FunctionDelegate body;
//			internal int parameterCount;

//			//

//			internal FunctionDefinition() { }

//			public string Name
//			{
//				get { return name; }
//			}

//			public FunctionDelegate Body
//			{
//				get { return body; }
//			}

//			public int ParameterCount
//			{
//				get { return parameterCount; }
//			}
//		}

//		///////////////////////////////////////////

//		public delegate double FunctionDelegate( string functionName, double[] values );

//		///////////////////////////////////////////

//		public ExpressionCalculator()
//		{
//		}

//		/// <summary>
//		/// Gets list of defined functions.
//		/// </summary>
//		public ICollection<FunctionDefinition> Functions
//		{
//			get { return functions.Values; }
//		}

//		/// <summary>
//		/// Get function by the name.
//		/// </summary>
//		/// <param name="name"></param>
//		/// <returns></returns>
//		public FunctionDefinition GetFunction( string name )
//		{
//			FunctionDefinition function;
//			if( functions.TryGetValue( name.ToLower(), out function ) )
//				return function;
//			return null;
//		}

//		/// <summary>
//		/// Registers function.
//		/// </summary>
//		/// <param name="name"></param>
//		/// <param name="body"></param>
//		/// <param name="parameterCount"></param>
//		public void RegisterFunction( string name, FunctionDelegate body, int parameterCount )
//		{
//			if( GetFunction( name ) != null )
//			{
//				Log.Fatal(
//					"ExpressionCalculator: RegisterFunction: Function with name \"{0}\" is already registered.",
//					name );
//			}

//			FunctionDefinition function = new FunctionDefinition();
//			function.name = name;
//			function.body = body;
//			function.parameterCount = parameterCount;
//			functions.Add( name.ToLower(), function );
//		}

//		///////////////////////////////////////////

//		enum SymbolTypes
//		{
//			Keyword,
//			Value,
//			Parameter,
//			Function,
//		}

//		class Symbol
//		{
//			public SymbolTypes type;
//			public char keyword;//for Keyword type
//			public double value;//for Value type
//			public string identifier;//for Parameter and Function types

//			public Symbol( SymbolTypes type, char keyword )
//			{
//				this.type = type;
//				this.keyword = keyword;
//			}

//			public Symbol( SymbolTypes type, double value )
//			{
//				this.type = type;
//				this.value = value;
//			}

//			public Symbol( SymbolTypes type, string identifier )
//			{
//				this.type = type;
//				this.identifier = identifier;
//			}

//			public override string ToString()
//			{
//				switch( type )
//				{
//				case SymbolTypes.Keyword: return "Keyword: " + keyword.ToString();
//				case SymbolTypes.Value: return "Value: " + value.ToString();
//				case SymbolTypes.Parameter: return "Parameter: " + identifier;
//				case SymbolTypes.Function: return "Function: " + identifier;
//				}
//				return base.ToString();
//			}
//		}

//		bool IsOperator( char c )
//		{
//			return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
//		}

//		bool IsKeyword( char c )
//		{
//			return IsOperator( c ) || c == '(' || c == ')' || c == ',';
//		}

//		List<Symbol> Separate( string input, Dictionary<string, int> parameterNamesLowerCase )//Set<string> parameterNamesLowerCase )
//		{
//			List<Symbol> result = new List<Symbol>();

//			StringBuilder builder = new StringBuilder();

//			int pos = 0;
//			while( pos < input.Length )
//			{
//				char c = input[ pos ];
//				if( c > 32 )
//				{
//					//parameter or function identifier
//					if( Char.IsLetter( c ) )
//					{
//						builder.Append( c );
//						for( int n = pos + 1; n < input.Length &&
//							( Char.IsLetter( input[ n ] ) || Char.IsDigit( input[ n ] ) ); n++ )
//						{
//							builder.Append( input[ n ] );
//						}

//						string name = builder.ToString();

//						if( parameterNamesLowerCase != null && parameterNamesLowerCase.ContainsKey( name.ToLower() ) )
//						{
//							result.Add( new Symbol( SymbolTypes.Parameter, name ) );
//						}
//						else if( GetFunction( name ) != null )
//						{
//							result.Add( new Symbol( SymbolTypes.Function, name ) );
//						}
//						else
//						{
//							throw new Exception( string.Format(
//								"Parameter or function with name \"{0}\" is not exists.", name ) );
//						}

//						pos += builder.Length;
//						builder.Length = 0;
//						continue;
//					}

//					bool previousIsOperatorOrOpenBracket = false;
//					if( result.Count != 0 )
//					{
//						Symbol previous = result[ result.Count - 1 ];
//						if( IsOperator( previous.keyword ) || previous.keyword == '(' )
//							previousIsOperatorOrOpenBracket = true;
//					}

//					//value
//					if( Char.IsDigit( c ) || c == '.' || ( ( c == '-' || c == '+' ) &&
//						( result.Count == 0 || previousIsOperatorOrOpenBracket ) ) )
//					{
//						builder.Append( c );
//						for( int n = pos + 1; n < input.Length &&
//							( Char.IsDigit( input[ n ] ) || input[ n ] == '.' ); n++ )
//						{
//							builder.Append( input[ n ] );
//						}

//						bool skip = false;
//						if( ( c == '-' || c == '+' ) && builder.Length == 1 )
//							skip = true;

//						if( !skip )
//						{
//							result.Add( new Symbol( SymbolTypes.Value, double.Parse( builder.ToString() ) ) );
//							pos += builder.Length;
//							builder.Length = 0;
//							continue;
//						}
//					}

//					//keyword
//					if( IsKeyword( c ) )
//					{
//						result.Add( new Symbol( SymbolTypes.Keyword, c ) );
//						pos++;
//						builder.Length = 0;
//						continue;
//					}

//					throw new Exception( "Invalid expression format." );
//				}
//				else
//					pos++;
//			}

//			return result;
//		}

//		List<Symbol> ConvertToPostfixNotation( List<Symbol> symbols )
//		{
//			List<Symbol> result = new List<Symbol>();
//			Stack<Symbol> stack = new Stack<Symbol>();

//			foreach( Symbol symbol in symbols )
//			{
//				if( symbol.type == SymbolTypes.Function )
//				{
//					stack.Push( symbol );
//					continue;
//				}

//				if( symbol.type == SymbolTypes.Value || symbol.type == SymbolTypes.Parameter )
//				{
//					result.Add( symbol );
//					continue;
//				}

//				if( symbol.type == SymbolTypes.Keyword )
//				{
//					if( IsOperator( symbol.keyword ) )
//					{
//						if( stack.Count == 0 || stack.Peek().keyword == '(' )
//						{
//							stack.Push( symbol );
//							continue;
//						}
//						else if( GetPriority( symbol.keyword ) > GetPriority( stack.Peek().keyword ) )
//						{
//							stack.Push( symbol );
//							continue;
//						}
//						else
//						{
//							while( stack.Count > 0 && GetPriority( symbol.keyword ) <= GetPriority( stack.Peek().keyword ) )
//								result.Add( stack.Pop() );
//							stack.Push( symbol );
//							continue;
//						}
//					}

//					if( symbol.keyword == '(' )
//					{
//						stack.Push( symbol );
//						continue;
//					}

//					if( symbol.keyword == ')' )
//					{
//						bool success = false;
//						while( stack.Count != 0 )
//						{
//							Symbol s = stack.Pop();
//							if( s.keyword == '(' )
//							{
//								success = true;
//								break;
//							}
//							result.Add( s );
//						}

//						if( success )
//						{
//							if( stack.Count != 0 )
//							{
//								Symbol top = stack.Peek();
//								if( top.type == SymbolTypes.Function )
//									result.Add( stack.Pop() );
//							}

//							continue;
//						}
//					}

//					if( symbol.keyword == ',' )
//					{
//						while( stack.Count > 0 && IsOperator( stack.Peek().keyword ) )
//							result.Add( stack.Pop() );
//						continue;
//					}
//				}

//				throw new Exception( "Invalid expression format." );
//			}

//			if( stack.Count > 0 )
//			{
//				foreach( Symbol symbol in stack )
//				{
//					if( symbol.type == SymbolTypes.Keyword && IsOperator( symbol.keyword ) )
//						result.Add( symbol );
//					else
//						throw new Exception( "Invalid expression format." );
//				}
//			}

//			return result;
//		}

//		int GetPriority( char keyword )
//		{
//			switch( keyword )
//			{
//			case '(': return 0;
//			case ')': return 1;
//			case '+': return 2;
//			case '-': return 3;
//			case '*': return 4;
//			case '/': return 4;
//			case '^': return 5;
//			default: return 999;
//			}
//		}

//		double CalculatePostfixNotation( List<Symbol> postfix )
//		{
//			Stack<Symbol> stack = new Stack<Symbol>();

//			foreach( Symbol symbol in postfix )
//			{
//				if( symbol.type == SymbolTypes.Value )
//				{
//					stack.Push( symbol );
//					continue;
//				}

//				if( symbol.type == SymbolTypes.Keyword )
//				{
//					if( stack.Count < 1 )
//						throw new Exception( "Invalid expression format." );

//					double b = Convert.ToDouble( stack.Pop().value );
//					double a = 0;
//					if( stack.Count > 0 )
//					{
//						a = Convert.ToDouble( stack.Pop().value );
//					}
//					else
//					{
//						if( symbol.keyword == '*' || symbol.keyword == '/' || symbol.keyword == '^' )
//							throw new Exception( "Invalid expression format." );
//					}

//					double v = 0;
//					switch( symbol.keyword )
//					{
//					case '+': v = a + b; break;
//					case '-': v = a - b; break;
//					case '*': v = a * b; break;
//					case '/': v = a / b; break;
//					case '^': v = Math.Pow( a, b ); break;
//					}

//					stack.Push( new Symbol( SymbolTypes.Value, v ) );
//					continue;
//				}

//				if( symbol.type == SymbolTypes.Function )
//				{
//					FunctionDefinition function = GetFunction( symbol.identifier );

//					if( stack.Count < function.parameterCount )
//						throw new Exception( "Invalid expression format." );

//					double[] values = new double[ function.parameterCount ];
//					for( int n = values.Length - 1; n >= 0; n-- )
//						values[ n ] = Convert.ToDouble( stack.Pop().value );

//					double v = function.body( function.name, values );

//					stack.Push( new Symbol( SymbolTypes.Value, v ) );
//					continue;
//				}


//				throw new Exception( "Invalid expression format." );
//			}

//			if( stack.Count != 1 )
//				throw new Exception( "Invalid expression format." );

//			Symbol s = stack.Pop();
//			return Convert.ToDouble( s.value );
//		}

//		///////////////////////////////////////////

//		/// <summary>
//		/// Represents a result of <see cref="ExpressionCalculator"/>.
//		/// </summary>
//		public class PreparedExpression
//		{
//			ExpressionCalculator calculator;
//			List<Symbol> postfix;

//			//

//			internal PreparedExpression( ExpressionCalculator calculator, object postfix )// List<Symbol> postfix )
//			{
//				this.calculator = calculator;
//				this.postfix = (List<Symbol>)postfix;
//			}

//			public bool Calculate( Dictionary<string, double> parameters, out double result, out string error )
//			{
//				result = 0;
//				error = null;

//				//convert parameter names to lower case
//				Dictionary<string, double> parametersLowerCase;
//				if( parameters != null )
//				{
//					parametersLowerCase = new Dictionary<string, double>( parameters.Count );
//					foreach( KeyValuePair<string, double> pair in parameters )
//						parametersLowerCase[ pair.Key.ToLower() ] = pair.Value;
//				}
//				else
//					parametersLowerCase = new Dictionary<string, double>();

//				//apply parameters
//				List<Symbol> postfixWithParameters = new List<Symbol>( postfix.Count );
//				for( int n = 0; n < postfix.Count; n++ )
//				{
//					Symbol symbol = postfix[ n ];

//					if( symbol.type == SymbolTypes.Parameter )
//					{
//						double value;
//						if( !parametersLowerCase.TryGetValue( symbol.identifier.ToLower(), out value ) )
//							value = 0;
//						symbol = new Symbol( SymbolTypes.Value, value );
//					}

//					postfixWithParameters.Add( symbol );
//				}

//				try
//				{
//					result = calculator.CalculatePostfixNotation( postfixWithParameters );
//					return true;
//				}
//				catch( Exception ex )
//				{
//					error = ex.Message;
//					return false;
//				}
//			}
//		}

//		///////////////////////////////////////////

//		/// <summary>
//		/// Prepares expression for faster calculation. Use when you need calculate one expression with different parameters many times.
//		/// </summary>
//		/// <param name="expression"></param>
//		/// <param name="parameterNames"></param>
//		/// <param name="error"></param>
//		/// <returns></returns>
//		public PreparedExpression Prepare( string expression, ICollection<string> parameterNames, out string error )
//		{
//			error = null;

//			try
//			{
//				Dictionary<string, int> parameterNamesLowerCase = null;
//				//Set<string> parameterNamesLowerCase = null;
//				if( parameterNames != null )
//				{
//					parameterNamesLowerCase = new Dictionary<string, int>( parameterNames.Count );
//					foreach( string parameterName in parameterNames )
//						parameterNamesLowerCase[ parameterName.ToLower() ] = 0;
//				}

//				List<Symbol> symbols = Separate( expression, parameterNamesLowerCase );
//				List<Symbol> postfix = ConvertToPostfixNotation( symbols );

//				PreparedExpression preparedExpression = new PreparedExpression( this, postfix );
//				return preparedExpression;
//			}
//			catch( Exception ex )
//			{
//				error = ex.Message;
//				return null;
//			}
//		}

//		/// <summary>
//		/// Calculates expression. Example: "1 + (2 * 3)", "5 * x + y".
//		/// </summary>
//		/// <param name="expression"></param>
//		/// <param name="parameters"></param>
//		/// <param name="result"></param>
//		/// <param name="error"></param>
//		/// <returns></returns>
//		public bool Calculate( string expression, Dictionary<string, double> parameters, out double result,
//			out string error )
//		{
//			error = null;
//			result = 0;

//			ICollection<string> parameterNames = null;
//			if( parameters != null )
//				parameterNames = parameters.Keys;

//			PreparedExpression preparedExpression = Prepare( expression, parameterNames, out error );
//			if( preparedExpression == null )
//				return false;

//			return preparedExpression.Calculate( parameters, out result, out error );
//		}

//		///////////////////////////////////////////

//		double Function_Abs( string functionName, double[] values )
//		{
//			return Math.Abs( values[ 0 ] );
//		}

//		double Function_Acos( string functionName, double[] values )
//		{
//			return Math.Acos( values[ 0 ] );
//		}

//		double Function_Asin( string functionName, double[] values )
//		{
//			return Math.Asin( values[ 0 ] );
//		}

//		double Function_Atan( string functionName, double[] values )
//		{
//			return Math.Atan( values[ 0 ] );
//		}

//		double Function_Atan2( string functionName, double[] values )
//		{
//			return Math.Atan2( values[ 0 ], values[ 1 ] );
//		}

//		double Function_Ceiling( string functionName, double[] values )
//		{
//			return Math.Ceiling( values[ 0 ] );
//		}

//		double Function_Cos( string functionName, double[] values )
//		{
//			return Math.Cos( values[ 0 ] );
//		}

//		double Function_Cosh( string functionName, double[] values )
//		{
//			return Math.Cosh( values[ 0 ] );
//		}

//		double Function_Exp( string functionName, double[] values )
//		{
//			return Math.Exp( values[ 0 ] );
//		}

//		double Function_Floor( string functionName, double[] values )
//		{
//			return Math.Floor( values[ 0 ] );
//		}

//		double Function_Log( string functionName, double[] values )
//		{
//			return Math.Log( values[ 0 ] );
//		}

//		double Function_Log10( string functionName, double[] values )
//		{
//			return Math.Log10( values[ 0 ] );
//		}

//		double Function_Max( string functionName, double[] values )
//		{
//			return Math.Max( values[ 0 ], values[ 1 ] );
//		}

//		double Function_Min( string functionName, double[] values )
//		{
//			return Math.Min( values[ 0 ], values[ 1 ] );
//		}

//		double Function_Pow( string functionName, double[] values )
//		{
//			return Math.Pow( values[ 0 ], values[ 1 ] );
//		}

//		double Function_Round( string functionName, double[] values )
//		{
//			return Math.Round( values[ 0 ] );
//		}

//		double Function_Sign( string functionName, double[] values )
//		{
//			return Math.Sign( values[ 0 ] );
//		}

//		double Function_Sin( string functionName, double[] values )
//		{
//			return Math.Sin( values[ 0 ] );
//		}

//		double Function_Sqrt( string functionName, double[] values )
//		{
//			return Math.Sqrt( values[ 0 ] );
//		}

//		double Function_Tan( string functionName, double[] values )
//		{
//			return Math.Tan( values[ 0 ] );
//		}

//		double Function_Tanh( string functionName, double[] values )
//		{
//			return Math.Tanh( values[ 0 ] );
//		}

//		/// <summary>
//		/// Registers basic math functions.
//		/// </summary>
//		/// <remarks>
//		/// Supported functions:
//		/// Abs( x ),
//		/// Acos( x ),
//		/// Asin( x ),
//		/// Atan( x ),
//		/// Atan2( y, x ),
//		/// Ceiling( x ),
//		/// Cos( x ),
//		/// Cosh( x ),
//		/// Exp( x ),
//		/// Floor( x ),
//		/// Log( x ),
//		/// Log10( x ),
//		/// Max( v1, v2 ),
//		/// Min( v1, v2 ),
//		/// Pow( x, y ),
//		/// Round( x ),
//		/// Sign( x ),
//		/// Sin( x ),
//		/// Sqrt( x ),
//		/// Tan( x ),
//		/// Tanh( x ).
//		/// </remarks>
//		public void RegisterBasicMathFunctions()
//		{
//			RegisterFunction( "Abs", Function_Abs, 1 );
//			RegisterFunction( "Acos", Function_Acos, 1 );
//			RegisterFunction( "Asin", Function_Asin, 1 );
//			RegisterFunction( "Atan", Function_Atan, 1 );
//			RegisterFunction( "Atan2", Function_Atan2, 2 );
//			RegisterFunction( "Ceiling", Function_Ceiling, 1 );
//			RegisterFunction( "Cos", Function_Cos, 1 );
//			RegisterFunction( "Cosh", Function_Cosh, 1 );
//			RegisterFunction( "Exp", Function_Exp, 1 );
//			RegisterFunction( "Floor", Function_Floor, 1 );
//			RegisterFunction( "Log", Function_Log, 1 );
//			//RegisterFunction( "Log(double a, double newBase);
//			RegisterFunction( "Log10", Function_Log10, 1 );
//			RegisterFunction( "Max", Function_Max, 2 );
//			RegisterFunction( "Min", Function_Min, 2 );
//			RegisterFunction( "Pow", Function_Pow, 2 );
//			RegisterFunction( "Round", Function_Round, 1 );
//			RegisterFunction( "Sign", Function_Sign, 1 );
//			RegisterFunction( "Sin", Function_Sin, 1 );
//			RegisterFunction( "Sqrt", Function_Sqrt, 1 );
//			RegisterFunction( "Tan", Function_Tan, 1 );
//			RegisterFunction( "Tanh", Function_Tanh, 1 );
//		}

//		///////////////////////////////////////////

//		//public static void Test()
//		//{
//		//	ExpressionCalculator calculator = new ExpressionCalculator();
//		//	calculator.RegisterBasicMathFunctions();

//		//	Dictionary<string, double> parameters = new Dictionary<string, double>();
//		//	parameters.Add( "x", 10 );
//		//	parameters.Add( "Y", -3 );

//		//	Dictionary<string, double> tests = new Dictionary<string, double>();
//		//	tests.Add( "5 + 3 * 2", 11 );
//		//	tests.Add( "5 * 3 * 2", 30 );
//		//	tests.Add( "5 / 3 * 2", 3.333333333333333 );
//		//	tests.Add( "(5 + 3) * 2", 16 );
//		//	tests.Add( "- (5 +3)", -8 );
//		//	tests.Add( "(5 + 3) * -2", -16 );
//		//	tests.Add( "-3 + -4*(-4/2) + -3 + +3", 5 );
//		//	tests.Add( "(5 + 3) - (5 - 3)", 6 );
//		//	tests.Add( "x * (5 + y)", 20 );
//		//	tests.Add( "Sin(1)", 0.8414709848078965066525023216303 );
//		//	tests.Add( "2 + Sin(Cos(0))", 2.8414709848078965066525023216303 );
//		//	tests.Add( "3 + Cos(5 + x) * 2", 1.4806241742823574523037071927334 );
//		//	tests.Add( "Pow(2, 3)", 8 );
//		//	tests.Add( "Max( 3 + x, 4 + x)", 14 );
//		//	tests.Add( "4 + Pow(  4,5) * 4", 4100 );
//		//	tests.Add( "Pow( x + 10,2)", 400 );
//		//	tests.Add( "4 + Pow(  x + 10,2) * 4", 1604 );
//		//	tests.Add( "4 + Pow(  (x + 10) * 2 - Pow(2,2), 2) * 4", 5188 );
//		//	tests.Add( "4 + Pow(  (x + 10) * 2 + Pow(Pow(2 *2, 2),2), Cos(0) + 1) * 4", 350468 );

//		//	string s = "";

//		//	foreach( KeyValuePair<string, double> test in tests )
//		//	{
//		//		string expresssion = test.Key;
//		//		double trueResult = test.Value;

//		//		double returnValue;
//		//		string error;
//		//		bool success = calculator.Calculate( expresssion, parameters, out returnValue, out error );

//		//		string resultStr;
//		//		if( success )
//		//		{
//		//			if( Math.Abs( returnValue - trueResult ) > .000001 )
//		//				resultStr = "INVALID RESULT";
//		//			else
//		//				resultStr = returnValue.ToString();
//		//		}
//		//		else
//		//			resultStr = "ERROR: " + error;
//		//		s += string.Format( "\"{0}\" = {1}\n", expresssion, resultStr );
//		//	}

//		//	Log.Fatal( "ExpressionCalculator: Test: \n" + s );
//		//}
//	}
//}
