using System.Diagnostics;
using AspectInjector.Broker;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;

namespace 記帳程式後端.Aspect
{
    [AspectInjector.Broker.Aspect(AspectInjector.Broker.Scope.Global)]
    public class WriteLog
    {
        private static readonly object lockObject = new object();

       

        


            // 關鍵修正：加上 Targets = Target.Method
        [AspectInjector.Broker.Advice(AspectInjector.Broker.Kind.Around, Targets = Target.Method)]
        public object OnMethodExecution(
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Name)] string methodName,
            [Argument(Source.Type)] Type declaringType)
        {

            var logger = Serilog.Log.Logger;
            var stopwatch = Stopwatch.StartNew();
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            var fullMethodName = $"{declaringType.Name}.{methodName}";

            // 使用 Serilog 的上下文豐富功能
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("MethodName", fullMethodName))
            using (LogContext.PushProperty("ClassName", declaringType.Name))
            {
                logger.Information("方法開始執行");

                if (args?.Length > 0)
                {
                    var formattedArgs = FormatArguments(args);
                    logger.Debug("方法參數: {Arguments}", formattedArgs);
                }
                else
                {
                    logger.Debug("方法無參數");
                }

                try
                {
                    var result = target(args);
                    stopwatch.Stop();

                    logger.Information("方法執行成功 (耗時: {ElapsedMs}ms)", stopwatch.ElapsedMilliseconds);

                    if (result != null)
                    {
                        var resultType = result.GetType();
                        if (IsSimpleType(resultType))
                        {
                            logger.Debug("方法返回值: {Result}", result);
                        }
                        else
                        {
                            try
                            {
                                var serializedResult = JsonConvert.SerializeObject(result,
                                    Formatting.None,
                                    new JsonSerializerSettings
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                        MaxDepth = 3,
                                        NullValueHandling = NullValueHandling.Ignore
                                    });

                                // 限制返回值長度
                                if (serializedResult.Length > 1000)
                                {
                                    serializedResult = serializedResult.Substring(0, 997) + "...";
                                }

                                logger.Debug("方法返回值: {Result}", serializedResult);
                            }
                            catch (Exception ex)
                            {
                                logger.Warning("方法返回值序列化失敗: {Error}", ex.Message);
                            }
                        }
                    }
                    else
                    {
                        logger.Debug("方法返回值: null");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    logger.Error(ex, "方法執行異常 (耗時: {ElapsedMs}ms)", stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
        }

        private object FormatArguments(object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                return "無參數";

            var argumentsDict = new Dictionary<string, object>();

            for (int i = 0; i < arguments.Length; i++)
            {
                var argument = arguments[i];
                string key = $"參數{i + 1}";

                if (argument == null)
                {
                    argumentsDict.Add(key, null);
                }
                else
                {
                    Type type = argument.GetType();
                    key += $"-{type.Name}";

                    try
                    {
                        // 對於簡單類型直接使用
                        if (IsSimpleType(type))
                        {
                            argumentsDict.Add(key, argument);
                        }
                        else
                        {
                            // 對於複雜類型進行序列化，但限制深度和長度
                            var serialized = JsonConvert.SerializeObject(argument,
                                Formatting.None,
                                new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    MaxDepth = 2,
                                    NullValueHandling = NullValueHandling.Ignore
                                });

                            // 限制參數長度
                            if (serialized.Length > 500)
                            {
                                serialized = serialized.Substring(0, 497) + "...";
                            }

                            argumentsDict.Add(key, serialized);
                        }
                    }
                    catch (Exception ex)
                    {
                        argumentsDict.Add(key, $"序列化失敗: {ex.Message}");
                    }
                }
            }

            return argumentsDict;
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid) ||
                   type.IsEnum ||
                   (Nullable.GetUnderlyingType(type) != null && IsSimpleType(Nullable.GetUnderlyingType(type)));
        }
    }
}

