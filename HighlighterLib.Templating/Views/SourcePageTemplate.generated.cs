﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HighlighterLib.Templating.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/SourcePageTemplate.cshtml")]
    public partial class SourcePageTemplate : System.Web.Mvc.WebViewPage<dynamic>
    {
        public SourcePageTemplate()
        {
        }
        public override void Execute()
        {
WriteLiteral(@"<!DOCTYPE html>

<html>
<head>
    <meta name=""viewport"" content=""width=device-width"" />
    <title>SourcePageTemplate</title>
    <link rel=""stylesheet"" type=""text/css"" href=""~/Content/Style.css""></>
</head>
<body>
    <div>

        <div id=""formattedCode"" class=""formattedCode"">");


            
            #line 12 "..\..\Views\SourcePageTemplate.cshtml"
                                                 Write(Html.Raw(Model.CodeElements));

            
            #line default
            #line hidden
WriteLiteral(@"</div>

        <script src=""http://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.0.min.js""></script>
        <script type=""text/javascript"">
            $(""#formattedCode"").find(""*"").each(function () {
                this.onmouseover = function () {
                    var hoverId = $(this).data.hover;
                    if(hoverId) {
                        $(""."" + hoverId).addClass(""highlighted"");
                    }
                };
                this.onmouseout = function () {
                    var hoverId = $(this).data.hover;
                    if (hoverId) {
                        $(""."" + hoverId).removeClass(""highlighted"");
                    }
                };
            });
        </script>
    </div>
</body>
</html>
");


        }
    }
}
#pragma warning restore 1591
