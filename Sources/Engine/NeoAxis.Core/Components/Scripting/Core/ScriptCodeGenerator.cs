// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !NO_EMIT
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis
{
	static class ScriptCodeGenerator
	{
		public static void CheckForSyntaxErrors( string code )
		{
			SyntaxTree methodTree = CSharpSyntaxTree.ParseText( code );
			var diagnostics = methodTree.GetDiagnostics();

			if( diagnostics.Any( d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error ) )
				throw new ScriptCompilerException( ScriptUtility.FormatCompilationError( diagnostics, true ) );
		}

		public static string GenerateWrappedScript( IEnumerable<string> methods, IEnumerable<string> usingNamespaces, string inheritFrom )//, Type contextType )
		{
			var tree = SyntaxFactory.CompilationUnit();

			//using
			foreach( var @using in usingNamespaces )
				tree = tree.AddUsings( SyntaxFactory.UsingDirective( SyntaxFactory.ParseName( @using ) ) );

			//classes
			foreach( var method in methods )
				tree = tree.AddMembers( GenerateClassWithMethod( method, inheritFrom/*, contextType*/ ) );

			//comment
			tree = tree.WithLeadingTrivia( SyntaxFactory.Comment( "// Auto-generated file" ) );

			var code = tree.NormalizeWhitespace().ToFullString();
			return code;
		}

		static List<(string Name, string Type)> GetContextVars()// Type contextType )
		{
			var contextType = typeof( CSharpScript.Context );

			var result = new List<(string Name, string Type)>();

			var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
			var fieldsAndProps = contextType.GetFields( bindingFlags ).Cast<MemberInfo>().Concat( contextType.GetProperties( bindingFlags ) );
			foreach( var member in fieldsAndProps )
				result.Add( (member.Name, Type: member.GetUnderlyingType().FullName) );

			return result;
		}

		static ClassDeclarationSyntax GenerateClassWithMethod( string methodCode, string inheritFrom = null )//, Type contextType = null )
		{
			// class decl

			var classDeclaration = SyntaxFactory.ClassDeclaration( GetUniqueName() )
				.AddModifiers( SyntaxFactory.Token( SyntaxKind.PublicKeyword ) );

			if( !string.IsNullOrEmpty( inheritFrom ) )
			{
				classDeclaration = classDeclaration.AddBaseListTypes(
					SyntaxFactory.SimpleBaseType( SyntaxFactory.ParseTypeName( inheritFrom ) ) );
			}

			// add atribute 

			var arguments = SyntaxFactory.ParseAttributeArgumentList( "(\"" + ToBase64( methodCode ) + "\")" );
			var attribute = SyntaxFactory.Attribute( SyntaxFactory.ParseName( nameof( CSharpScriptGeneratedAttribute ) ), arguments );
			var attributeList = SyntaxFactory.AttributeList( SyntaxFactory.SingletonSeparatedList( attribute ) )
				.WithTrailingTrivia( SyntaxFactory.CarriageReturnLineFeed );

			classDeclaration = classDeclaration.AddAttributeLists( attributeList );

			// generate context vars

			foreach( var contextVar in GetContextVars( /*contextType*/ ) )
			{
				var variableDeclaration = SyntaxFactory.VariableDeclaration( SyntaxFactory.ParseTypeName( contextVar.Type ) )
					.AddVariables( SyntaxFactory.VariableDeclarator( contextVar.Name ) );
				var fieldDeclaration = SyntaxFactory.FieldDeclaration( variableDeclaration )
					.AddModifiers( SyntaxFactory.Token( SyntaxKind.PublicKeyword ) );

				// we can add property here see SyntaxFactory.PropertyDeclaration

				classDeclaration = classDeclaration.AddMembers( fieldDeclaration );
			}

			// add methods (and not only methods ?)

			SyntaxTree methodTree = CSharpSyntaxTree.ParseText( methodCode );

			var membersDeclaration = methodTree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();
			foreach( var member in membersDeclaration )
				classDeclaration = classDeclaration.AddMembers( member );

			return classDeclaration;
		}

		static string GetUniqueName()
		{
			return "DynamicClass_" + Guid.NewGuid().ToString().Replace( "-", "_" );
		}

		static string ToBase64( string code )
		{
			byte[] bytes = Encoding.UTF8.GetBytes( code );
			return Convert.ToBase64String( bytes, Base64FormattingOptions.None );
		}

		public static MethodDeclarationSyntax GenerateMethodFromReflection( string methodName, ParameterInfo[] parameters )
		{
			var returnParam = parameters.FirstOrDefault( p => p.IsRetval );
			var returnTypeName = returnParam != null ? returnParam.ParameterType.Name : "void";

			var parameterList = SyntaxFactory.ParameterList( SyntaxFactory.SeparatedList( GetParametersList( parameters ) ) );
			return SyntaxFactory.MethodDeclaration(
				attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
				modifiers: SyntaxFactory.TokenList( SyntaxFactory.Token( SyntaxKind.PublicKeyword ) ),
				returnType: SyntaxFactory.ParseTypeName( returnTypeName ),
				explicitInterfaceSpecifier: null,
				identifier: SyntaxFactory.Identifier( methodName ),
				typeParameterList: null,
				parameterList: parameterList,
				constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
				body: SyntaxFactory.Block(),
				semicolonToken: SyntaxFactory.Token( SyntaxKind.SemicolonToken )
			).WithAdditionalAnnotations( Formatter.Annotation );// Annotate that this node should be formatted
		}

		static List<ParameterSyntax> GetParametersList( ParameterInfo[] parameters )
		{
			var result = new List<ParameterSyntax>();

			foreach( var p in parameters )
			{
				if( !p.IsRetval )
				{
					bool byRef = p.ParameterType.IsByRef;

					var unrefParameterType = p.ParameterType;
					if( byRef )
						unrefParameterType = unrefParameterType.GetElementType();

					TypeSyntax type;
					var shortName = MetadataManager.GetNetTypeShortName( unrefParameterType );
					if( !string.IsNullOrEmpty( shortName ) )
						type = SyntaxFactory.ParseTypeName( shortName );
					//else if( p.ParameterType == typeof( Component ) )
					//	type = SyntaxFactory.ParseTypeName( p.ParameterType.FullName );
					else
					{
						type = SyntaxFactory.ParseTypeName( p.ParameterType.FullName.Replace( '+', '.' ).Replace( "&", "" ) );
						//type = SyntaxFactory.ParseTypeName( p.ParameterType.Name );
					}

					var parameter = SyntaxFactory.Parameter(
						attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
						modifiers: GetParameterModifiers( p ),
						type,
						identifier: SyntaxFactory.Identifier( p.Name ),
						@default: null );
					result.Add( parameter );

					//var parameter = SyntaxFactory.Parameter(
					//	attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
					//	modifiers: GetParameterModifiers( p ),
					//	type: SyntaxFactory.ParseTypeName( p.ParameterType.FullName ),
					//	identifier: SyntaxFactory.Identifier( p.Name ),
					//	@default: null );
					//result.Add( parameter );
				}
			}

			return result;
		}

		static SyntaxTokenList GetParameterModifiers( ParameterInfo parameter )
		{
			if( parameter.IsOut )
				return SyntaxFactory.TokenList( SyntaxFactory.Token( SyntaxKind.OutKeyword ) );
			if( parameter.ParameterType.IsByRef )
				return SyntaxFactory.TokenList( SyntaxFactory.Token( SyntaxKind.RefKeyword ) );
			return SyntaxFactory.TokenList();
		}

		async static Task<Document> AddMethodToClass2( Document document, MethodDeclarationSyntax method )
		{
			var root = await document.GetSyntaxRootAsync().ConfigureAwait( false );
			var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
			if( classDeclaration == null )
				throw new Exception( "Class not found in document" );

			var newClassDeclaration = classDeclaration.AddMembers( method );
			document = document.WithSyntaxRoot( root.ReplaceNode( classDeclaration, newClassDeclaration ) );
			return await Formatter.FormatAsync( document, Formatter.Annotation ).ConfigureAwait( false );
		}

		public static Document AddMethodToClass( Document document, MethodDeclarationSyntax method )
		{
			return AddMethodToClass2( document, method ).Result;
		}
	}
}
#endif