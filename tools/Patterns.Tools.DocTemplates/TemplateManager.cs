#region FreeBSD

// Copyright (c) 2013, John Batte
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that
// the following conditions are met:
// 
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the
//    following disclaimer.
// 
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
//    following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

#endregion

using System.Resources;
using System.Text.RegularExpressions;

using Patterns.Tools.DocTemplates.Models;
using Patterns.Tools.DocTemplates.Templates;

using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace Patterns.Tools.DocTemplates
{
	public static class TemplateManager
	{
		private const string _templateNameKey = "name";

		private static readonly Regex _templateNamePattern =
			new Regex(string.Format(@"^(?<{0}>.+)\.cshtml$", _templateNameKey));

		private static readonly ResourceManager _templateResources = new ResourceManager(typeof (TemplateResources));

		static TemplateManager()
		{
			Razor.SetTemplateService(new TemplateService(new TemplateServiceConfiguration
			{
				Resolver = new DelegateTemplateResolver(GetTemplate)
			}));
		}

		public static string RunAssemblyTemplate(AssemblyModel model)
		{
			return Razor.Parse(TemplateResources.AssemblyTemplate, model);
		}

		private static string GetTemplate(string templateName)
		{
			Match nameMatch = _templateNamePattern.Match(templateName);
			string template = nameMatch.Success ? nameMatch.Groups[_templateNameKey].Value : templateName;
			return _templateResources.GetString(template);
		}
	}
}