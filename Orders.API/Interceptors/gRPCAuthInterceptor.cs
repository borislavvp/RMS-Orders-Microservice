using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace Orders.API.Interceptors
{
    public class gRPCAuthInterceptor : Interceptor
    {
        private IHttpContextAccessor _httpContextAccessor;
        public gRPCAuthInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var metadata = new Metadata();
            metadata.Add("Authorization", _httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
            var userIdentity = _httpContextAccessor.HttpContext.User.Identity;
            
            var callOption = context.Options.WithHeaders(metadata);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOption);

            return base.AsyncUnaryCall(request, context, continuation);
        }
    }
}
