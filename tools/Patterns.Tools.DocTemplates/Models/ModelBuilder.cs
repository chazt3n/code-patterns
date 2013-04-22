#region FreeBSD

// Copyright (c) 2013, John Batte
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Patterns.Tools.DocTemplates.Fx;

namespace Patterns.Tools.DocTemplates.Models
{
	public class ModelBuilder
	{
		private const BindingFlags _binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

		public static AssemblyModel BuildAssemblyModel(Assembly assembly, bool isPublishedOnNuGet = true)
		{
			AssemblyName name = assembly.GetName();

			var model = new AssemblyModel
			{
				Name = name.Name,
				Version = name.Version.ToString(),
				Description = ReadAssemblyDescription(assembly),
				IsPublishedOnNuGet = isPublishedOnNuGet
			};

			foreach (var type in assembly.GetTypes()) AddAssemblyType(model, type);

			return model;
		}

		public static void AddAssemblyType(AssemblyModel assembly, Type type)
		{
			AddAssemblyType(assembly, BuildTypeModel(type));
		}

		public static void AddAssemblyType(AssemblyModel assembly, TypeModel type)
		{
			NamespaceModel owner = BuildNamespace(assembly, type);
			owner.Add(type);
		}

		public static TypeModel BuildTypeModel(Type type, bool filterObjectMatches = true)
		{
			if (type.IsEnum) return BuildTypeModel<EnumModel>(type, filterObjectMatches);

			if (type.IsInterface) return BuildTypeModel<InterfaceModel>(type, filterObjectMatches);

			if (type.IsClass) return BuildTypeModel<ClassModel>(type, filterObjectMatches);

			return BuildTypeModel<StructModel>(type, filterObjectMatches);
		}

		public static TModel BuildTypeModel<TModel>(Type type,
			bool filterObjectMatches = true,
			Action<TModel, IEnumerable<FieldModel>> fieldHandler = null,
			Action<TModel, IEnumerable<PropertyModel>> propertyHandler = null,
			Action<TModel, IEnumerable<ConstructorModel>> constructorHandler = null,
			Action<TModel, IEnumerable<MethodModel>> methodHandler = null,
			Action<TModel, IDictionary<int, string>> enumValueHandler = null) where TModel : TypeModel, new()
		{
			var state = new TypeBuilderState<TModel>(type, filterObjectMatches);

			if (!(type.IsEnum || type.IsInterface)) BuildConstructors(state, constructorHandler);

			if (type.IsClass || type.IsStruct()) BuildFields(state, fieldHandler);

			if (!type.IsEnum) BuildProperties(state, propertyHandler);

			if (!type.IsEnum) BuildMethods(state, methodHandler);

			if (type.IsEnum) BuildEnumValues(state, enumValueHandler);

			return state.Model;
		}

		private static string ReadAssemblyDescription(Assembly assembly)
		{
			return assembly.GetCustomAttributesData()
				.Where(data => data.AttributeType == typeof (AssemblyDescriptionAttribute))
				.Select(data => data.ConstructorArguments.First().Value.ToString())
				.FirstOrDefault();
		}

		private static NamespaceModel BuildNamespace(AssemblyModel assemblyModel, TypeModel type)
		{
			bool emptyNamespace = string.IsNullOrEmpty(type.Namespace);
			string[] parts = emptyNamespace ? null : type.Namespace.Split('.');
			string firstPass = emptyNamespace ? null : parts[0];
			NamespaceModel firstMatch = assemblyModel.Namespaces.FirstOrDefault(item => item.NamespaceName == firstPass);

			if (firstMatch == null)
			{
				firstMatch = new NamespaceModel {NamespaceName = firstPass};
				assemblyModel.Add(firstMatch);
			}

			return MatchOrCreateNamespace(new NamespaceBuilderState(type.Namespace, parts, firstPass, firstMatch, 0));
		}

		private static NamespaceModel MatchOrCreateNamespace(NamespaceBuilderState state)
		{
			if (state.CurrentNamespace.NamespaceName == state.Target) return state.CurrentNamespace;

			int depth = state.Depth + 1;
			string pattern = new StringBuilder(state.Pattern).AppendFormat(".{0}", state.Parts.ElementAt(depth)).ToString();
			NamespaceModel match = state.CurrentNamespace.Namespaces.FirstOrDefault(item => item.NamespaceName == pattern);

			if (match == null)
			{
				match = new NamespaceModel {NamespaceName = pattern};
				state.CurrentNamespace.Add(match);
			}

			return MatchOrCreateNamespace(new NamespaceBuilderState(state.Target, state.Parts, pattern, match, depth));
		}

		private static void BuildEnumValues<TModel>(TypeBuilderState<TModel> state, Action<TModel, IDictionary<int, string>> enumValueHandler)
			where TModel : TypeModel, new()
		{
			enumValueHandler = enumValueHandler ?? ((model, values) =>
			{
				var enumModel = model as IHaveEnumValues;
				if (enumModel != null) enumModel.Values = values;
			});

			enumValueHandler(state.Model, Enum.GetValues(state.Type).Cast<int>().ToDictionary(key => key, key => Enum.GetName(state.Type, key)));
		}

		private static void BuildMethods<TModel>(TypeBuilderState<TModel> state, Action<TModel, IEnumerable<MethodModel>> methodHandler)
			where TModel : TypeModel, new()
		{
			methodHandler = methodHandler ?? ((model, methods) =>
			{
				var methodModel = model as IHaveMethods;
				if (methodModel != null) methodModel.Methods = methods;
			});

			methodHandler(state.Model, state.Type.GetMethods(_binding)
				.Where(info => !state.FilterObjectMatches || !IsDefinedByObject(info))
				.Select(info => new MethodModel
				{
					Name = info.Name
				}));
		}

		private static void BuildConstructors<TModel>(TypeBuilderState<TModel> state, Action<TModel, IEnumerable<ConstructorModel>> constructorHandler)
			where TModel : TypeModel, new()
		{
			constructorHandler = constructorHandler ?? ((model, constructors) =>
			{
				var constructorModel = model as IHaveConstructors;
				if (constructorModel != null) constructorModel.Constructors = constructors;
			});

			constructorHandler(state.Model, state.Type.GetMembers(_binding).OfType<ConstructorInfo>()
				.Where(info => !state.FilterObjectMatches || !IsDefinedByObject(info))
				.Select(info => new ConstructorModel()));
		}

		private static void BuildFields<TModel>(TypeBuilderState<TModel> state, Action<TModel, IEnumerable<FieldModel>> fieldHandler)
			where TModel : TypeModel, new()
		{
			fieldHandler = fieldHandler ?? ((model, fields) =>
			{
				var fieldModel = model as IHaveFields;
				if (fieldModel != null) fieldModel.Fields = fields;
			});

			fieldHandler(state.Model, state.Type.GetFields(_binding)
				.Where(info => !state.FilterObjectMatches || !IsDefinedByObject(info))
				.Select(info => new FieldModel
				{
					Name = info.Name
				}));
		}

		private static void BuildProperties<TModel>(TypeBuilderState<TModel> state, Action<TModel, IEnumerable<PropertyModel>> propertyHandler)
			where TModel : TypeModel, new()
		{
			propertyHandler = propertyHandler ?? ((model, properties) =>
			{
				var propertyModel = model as IHaveProperties;
				if (propertyModel != null) propertyModel.Properties = properties;
			});

			propertyHandler(state.Model, state.Type.GetProperties(_binding)
				.Where(info => !state.FilterObjectMatches || !IsDefinedByObject(info))
				.Select(info => new PropertyModel
				{
					Name = info.Name
				}));
		}

		private static bool IsDefinedByObject(MemberInfo info)
		{
			return typeof (object).GetMembers(_binding)
				.Any(member => member.MemberType == info.MemberType && member.DeclaringType == info.DeclaringType && member.Name == info.Name);
		}

		private struct TypeBuilderState<TModel> where TModel : TypeModel, new()
		{
			public TypeBuilderState(Type type, bool filterObjectMatches) : this()
			{
				Model = new TModel {Name = type.Name, FullName = type.FullName, Namespace = type.Namespace};
				Type = type;
				FilterObjectMatches = filterObjectMatches;
			}

			public Type Type { get; private set; }
			public bool FilterObjectMatches { get; private set; }
			public TModel Model { get; private set; }
		}

		private struct NamespaceBuilderState
		{
			public NamespaceBuilderState(string target, IEnumerable<string> parts, string pattern, NamespaceModel currentNamespace, int depth) : this()
			{
				Parts = parts;
				CurrentNamespace = currentNamespace;
				Depth = depth;
				Pattern = pattern;
				Target = target;
			}

			public IEnumerable<string> Parts { get; private set; }
			public NamespaceModel CurrentNamespace { get; private set; }
			public int Depth { get; private set; }
			public string Pattern { get; private set; }
			public string Target { get; private set; }
		}
	}
}