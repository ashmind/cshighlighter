﻿using HighlighterLib.Templating.Models;
using HighlighterLib.Templating.Properties;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HighlighterLib.Templating
{
    public static class Render
    {
        private const string CssResourceName = "Style";
        private const string JsResourceName = "HightlightingScript";

        static Render()
        {
            var config = new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof(HtmlTemplateBase<>)
            };

            var service = new TemplateService(config);
            Razor.SetTemplateService(service);
            Razor.Compile(Resources.FormattedSingleFile, "singleFile");
            Razor.Compile(Resources.Directory, "directory");
        }

        public static string SinglePage(string htmlContent)
        {
            var m = new Models.SingleFileModel()
            {
                Stylesheets = StaticContent.GetStyles(),
                PreformattedHtml = htmlContent,
                Javascript = StaticContent.GetScripts()
            };
            return RenderSingleFile(m);
        }

        public static string SinglePage(string htmlContent, IEnumerable<Uri> cssPaths, IEnumerable<Uri> jsPaths)
        {
            var m = new Models.SingleFileModel()
            {
                StylesheetUris = cssPaths,
                PreformattedHtml = htmlContent,
                JavascriptUris = jsPaths
            };
            return RenderSingleFile(m);
        }

        public static string Directory(IEnumerable<string> files)
        {
            return Razor.Run("directory", files);
        }

        private static string RenderSingleFile(SingleFileModel m)
        {
            return Razor.Run("singleFile", m);
        }
        
    }
}