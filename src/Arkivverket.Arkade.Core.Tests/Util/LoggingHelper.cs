using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Display;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    /*
     *  
     * Original code from https://github.com/damianh/CapturingLogOutputWithXunit2AndParallelTests
     * 
     * The MIT License (MIT)
     * 
     * Copyright (c) 2015 Damian Hickey
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy
     * of this software and associated documentation files (the "Software"), to deal
     * in the Software without restriction, including without limitation the rights
     * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
     * copies of the Software, and to permit persons to whom the Software is
     * furnished to do so, subject to the following conditions:
     * 
     * The above copyright notice and this permission notice shall be included in all
     * copies or substantial portions of the Software.
     * 
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
     * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
     * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
     * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
     * SOFTWARE.
     */
    internal static class LoggingHelper
    {
        private static readonly Subject<LogEvent>  LogEventSubject = new Subject<LogEvent>();
        private const string CaptureCorrelationIdKey = "CaptureCorrelationId";
        private static readonly MessageTemplateTextFormatter MessageTemplate = new MessageTemplateTextFormatter(
               "{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}", null);
        //private static readonly MessageTemplateTextFormatter s_formatter = new MessageTemplateTextFormatter(
        //        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}", null);

        static LoggingHelper()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .Observers(observable => observable.Subscribe(logEvent => LogEventSubject.OnNext(logEvent)))
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static IDisposable Capture(ITestOutputHelper testOutputHelper)
        {
            var captureId = Guid.NewGuid();

            Func<LogEvent, bool> filter = logEvent => 
                logEvent.Properties.ContainsKey(CaptureCorrelationIdKey) &&
                logEvent.Properties[CaptureCorrelationIdKey].ToString() == captureId.ToString();

            var subscription = LogEventSubject.Where(filter).Subscribe(logEvent =>
            {
                using(var writer = new StringWriter())
                {
                    MessageTemplate.Format(logEvent, writer);
                    testOutputHelper.WriteLine(writer.ToString());
                }
            });
            var pushProperty = LogContext.PushProperty(CaptureCorrelationIdKey, captureId);

            return new DisposableAction(() =>
            {
                subscription.Dispose();
                pushProperty.Dispose();
            });
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}
