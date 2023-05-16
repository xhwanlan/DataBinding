#if RELEASE

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBinding.SourceGenerator
{
    [Generator]
    public class DataBindingPropertyGenerator : ISourceGenerator
    {
        void ISourceGenerator.Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
            {
                return;
            }

            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("DataBinding.DataBindingPropertyAttribute");
            INamedTypeSymbol notifySymbol = context.Compilation.GetTypeByMetadataName("DataBinding.IDataBindingObject");

            List<IFieldSymbol> fieldSymbols = new List<IFieldSymbol>();
            foreach (FieldDeclarationSyntax field in receiver.CandidateFields)
            {
                SemanticModel model = context.Compilation.GetSemanticModel(field.SyntaxTree);
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    IFieldSymbol fieldSymbol = model.GetDeclaredSymbol(variable) as IFieldSymbol;
                    if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                    {
                        fieldSymbols.Add(fieldSymbol);
                    }
                }
            }

            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in fieldSymbols.GroupBy(f => f.ContainingType))
            {
                String classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, notifySymbol);
                if (!String.IsNullOrWhiteSpace(classSource))
                {
                    context.AddSource($"{group.Key.Name}_AutoBinding.cs", SourceText.From(classSource, Encoding.UTF8));
                }
            }
        }

        private String ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attributeSymbol, ISymbol notifySymbol)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return String.Empty;
            }

            String namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            StringBuilder source = new StringBuilder($@"using System;
namespace {namespaceName}
{{
    {classSymbol.DeclaredAccessibility.ToString().ToLowerInvariant()} partial class {classSymbol.Name}");

            if (!classSymbol.AllInterfaces.Any(symbol => symbol.Equals(notifySymbol, SymbolEqualityComparer.Default)))
            {
                source.Append($@" : {notifySymbol.ToDisplayString()}");
            }

            source.Append($@"
    {{
");

            foreach (IFieldSymbol fieldSymbol in fields)
            {
                ProcessField(source, fieldSymbol, attributeSymbol);
            }

            source.Append(
                @"    } 
}");
            return source.ToString();
        }

        private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            String fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            AttributeData attributeData = fieldSymbol.GetAttributes().Single(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));

            String propertyName = GetName(attributeData.ConstructorArguments.First().Value);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                return;
            }

            source.Append($@"        public {fieldType} {propertyName} 
        {{
            get 
            {{
                return this.{fieldName};
            }}
            set
            {{");

            source.Append($@"
                ((IDataBindingObject)this).SetPropertyValue(ref this.{fieldName}, value, nameof({propertyName}));");

            source.Append(@"
            }
        }
");

            String GetName(Object obj)
            {
                if (obj is String name)
                {
                    return name;
                }
                return String.Empty;
            }
        }

        void ISourceGenerator.Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<FieldDeclarationSyntax> CandidateFields { get; } = new List<FieldDeclarationSyntax>();

            void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.AttributeLists.Count > 0)
                {
                    CandidateFields.Add(fieldDeclarationSyntax);
                }
            }
        }
    }
}
#endif