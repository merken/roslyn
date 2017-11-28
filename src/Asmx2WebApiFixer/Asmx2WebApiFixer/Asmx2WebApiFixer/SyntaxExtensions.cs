using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asmx2WebApiFixer
{
    public static class SyntaxExtensions
    {
        public static bool ContainsAttributeInList(this SyntaxList<AttributeListSyntax> attributeLists, string attribute)
        {
            return attributeLists.SelectMany(a => a.Attributes).Select(at => at.Name.ToFullString()).Any(fs => fs == attribute);
        }
    }
}
