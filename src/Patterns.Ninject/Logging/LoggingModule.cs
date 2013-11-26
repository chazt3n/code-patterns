// Copyright (c) 2013, The Tribe
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

using System;
using System.Collections.Generic;
using System.Data;

using Common.Logging;

using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;

using Patterns.Configuration;
using Patterns.Logging;

namespace Patterns.Ninject.Logging
{
	public class LoggingModule : NinjectModule
	{
		/// <summary>
		///     The default log factory
		/// </summary>
		public static readonly Func<Type, ILog> DefaultLogFactory = type => LogManager.GetLogger(type);

		private readonly Func<Type, ILog> _logFactory;

		/// <summary>
		///     Initializes a new instance of the <see cref="LoggingModule" /> class.
		/// </summary>
		public LoggingModule() : this(DefaultLogFactory) {}

		protected LoggingModule(Func<Type, ILog> logFactory)
		{
			_logFactory = logFactory;
		}

		/// <summary>
		///     Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<Func<Type, ILog>>().ToConstant(_logFactory);
			Bind<LoggingInterceptor>().ToSelf();
			Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Type));

			//If no bindings are found for configuration source, set our flag to false.
			IEnumerable<IBinding> bindings = Kernel.GetBindings(typeof (IConfigurationSource));
			bool configSourceRegistered = bindings != null;

			Bind<ILoggingConfig>().ToMethod(context =>
			{
				try
				{
					//TODO: Handle this optional business. What do we want to do if this hasn't been loaded? Should we inject a blank loggingConfig? A default?
					IConfigurationSource configSource = configSourceRegistered ? Kernel.Get<ConfigurationSource>() : null;

					LoggingConfig loggingConfig = configSource != null
						                              ? configSource.GetSection<LoggingConfig>(LoggingConfig.SectionName)
						                              : null;
					return loggingConfig ?? new LoggingConfig();
				}
				catch
				{
					//TODO: Create Ninject error builder.
					//throw ErrorBuilder.BuildContainerException(registrationError, ConfigurationResources.MissingConfigSourceErrorHint);
					throw new DataException();
				}
			});
		}
	}
}