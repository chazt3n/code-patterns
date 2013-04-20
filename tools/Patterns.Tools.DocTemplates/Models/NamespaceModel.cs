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

using System;
using System.Collections.Generic;

namespace Patterns.Tools.DocTemplates.Models
{
	public class NamespaceModel
	{
		private static readonly IDictionary<Type, Action<NamespaceModel, object>> _modelBinders
			= new Dictionary<Type, Action<NamespaceModel, object>>
			{
				{typeof (ClassModel), (model, data) => model._classes.Add(data as ClassModel)},
				{typeof (InterfaceModel), (model, data) => model._interfaces.Add(data as InterfaceModel)},
				{typeof (StructModel), (model, data) => model._structs.Add(data as StructModel)},
				{typeof (EnumModel), (model, data) => model._enums.Add(data as EnumModel)}
			};

		private readonly IList<NamespaceModel> _namespaces;
		private readonly IList<InterfaceModel> _interfaces;
		private readonly IList<ClassModel> _classes;
		private readonly IList<EnumModel> _enums;
		private readonly IList<StructModel> _structs;

		public NamespaceModel()
		{
			_namespaces = new List<NamespaceModel>();
			_interfaces = new List<InterfaceModel>();
			_classes = new List<ClassModel>();
			_enums = new List<EnumModel>();
			_structs = new List<StructModel>();
		}

		public string NamespaceName { get; set; }

		public IEnumerable<NamespaceModel> Namespaces
		{
			get { return _namespaces; }
		}

		public IEnumerable<InterfaceModel> Interfaces
		{
			get { return _interfaces; }
		}

		public IEnumerable<ClassModel> Classes
		{
			get { return _classes; }
		}

		public IEnumerable<EnumModel> Enums
		{
			get { return _enums; }
		}

		public IEnumerable<StructModel> Structs
		{
			get { return _structs; }
		}

		public NamespaceModel Match(TypeModel type)
		{
			
		}

		internal void Add(NamespaceModel childNamespace)
		{
			_namespaces.Add(childNamespace);
		}

		internal void Add<TModel>(TModel model) where TModel : TypeModel
		{
			Type modelType = typeof (TModel);
			Action<NamespaceModel, object> modelBinder = _modelBinders.ContainsKey(modelType) ? _modelBinders[modelType] : null;
			if (modelBinder != null) modelBinder(this, model);
		}
	}
}