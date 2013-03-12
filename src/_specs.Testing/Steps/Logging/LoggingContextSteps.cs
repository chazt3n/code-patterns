﻿#region FreeBSD

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
using System.Diagnostics;

using Castle.DynamicProxy;

using Common.Logging;

using Moq;

using Patterns.Logging;
using Patterns.Specifications.Models.Interception;
using Patterns.Specifications.Models.Logging;
using Patterns.Specifications.Models.Mocking;

using TechTalk.SpecFlow;

namespace Patterns.Specifications.Steps.Logging
{
	[Binding]
	public class LoggingContextSteps
	{
		private readonly LoggingContext _context;
		private readonly InterceptionContext _interception;
		private readonly MoqContext _moq;

		public LoggingContextSteps(LoggingContext context, InterceptionContext interception, MoqContext moq)
		{
			_context = context;
			_interception = interception;
			_moq = moq;
		}

		[Given(@"I have a default log config")]
		public void CreateDefaultConfig()
		{
			_context.Config = new LoggingConfig();
		}

		[Given(@"I have a log config set to trap errors")]
		public void CreateErrorTrappingConfig()
		{
			_context.Config = new LoggingConfig {TrapExceptions = true};
		}

		[Given(@"I have a log factory that returns a mocked ILog")]
		public void CreateMockLogFactory()
		{
			Mock<ILog> mockLog = _moq.Container.Mock<ILog>();
			mockLog.Setup(log => log.Info(It.IsAny<Action<FormatMessageHandler>>())).Callback(CreateLogCallback());
			mockLog.Setup(log => log.Trace(It.IsAny<Action<FormatMessageHandler>>())).Callback(CreateLogCallback());
			mockLog.Setup(log => log.Debug(It.IsAny<Action<FormatMessageHandler>>())).Callback(CreateLogCallback());
			mockLog.Setup(log => log.Warn(It.IsAny<Action<FormatMessageHandler>>())).Callback(CreateLogCallback());
			mockLog.Setup(log => log.Error(It.IsAny<Action<FormatMessageHandler>>())).Callback(CreateLogCallback());
			mockLog.Setup(log => log.Fatal(It.IsAny<Action<FormatMessageHandler>>())).Callback(CreateLogCallback());
			_context.LogFactory = type => mockLog.Object;
		}

		[Given(@"I have a logging interceptor")]
		public void CreateLoggingInterceptor()
		{
			_interception.Interceptor = new LoggingInterceptor(_context.Config, _context.LogFactory);
		}

		[Given(@"I have a dynamic proxy to the logging test subject")]
		public void CreateDynamicProxy()
		{
			var generator = new ProxyGenerator();
			_context.TestSubject = generator.CreateClassProxy<LoggingTestSubject>(_interception.Interceptor);
		}

		private static Action<Action<FormatMessageHandler>> CreateLogCallback()
		{
			return action => action((format, args) =>
			{
				string message = string.Format(format, args);
				Debug.WriteLine(message);
				return message;
			});
		}
	}
}