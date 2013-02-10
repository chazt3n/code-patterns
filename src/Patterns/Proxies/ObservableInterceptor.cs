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
using System.Reactive.Subjects;

using Castle.DynamicProxy;

namespace Patterns.Proxies
{
	public class ObservableInterceptor : IInterceptor, IObservableInterceptor
	{
		private readonly Subject<IInvocation> _interceptionBeginning = new Subject<IInvocation>();
		private readonly Subject<IInvocation> _interceptionEnding = new Subject<IInvocation>();
		private readonly Subject<IInvocation> _invocations = new Subject<IInvocation>();

		public IObservable<IInvocation> InterceptionBeginning { get { return _interceptionBeginning; } }
		public IObservable<IInvocation> InterceptionEnding { get { return _interceptionEnding; } }
		public IObservable<IInvocation> Invocations { get { return _invocations; } }

		public void Intercept(IInvocation invocation)
		{
			_interceptionBeginning.OnNext(invocation);

			try
			{
				invocation.Proceed();
				_invocations.OnNext(invocation);
			}
			catch(Exception error)
			{
				_invocations.OnError(error);
			}

			_interceptionEnding.OnNext(invocation);
		}
	}
}