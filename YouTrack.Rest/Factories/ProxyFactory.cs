#if !WINDOWS_PHONE
using Castle.DynamicProxy;
using YouTrack.Rest.Interception;
#endif

namespace YouTrack.Rest.Factories
{
    abstract class ProxyFactory
    {
        public TProxy CreateProxy<TProxy>(object target)
        {
#if WINDOWS_PHONE
            return (TProxy) target;
#else
            ProxyGenerator proxyGenerator = new ProxyGenerator();

            ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions(new PropertyGetterProxyGenerationHook<TProxy>());

            object issueProxy = proxyGenerator.CreateInterfaceProxyWithTargetInterface(typeof(TProxy), target, proxyGenerationOptions, new LoadableProxyInterceptor());

            return (TProxy)issueProxy;
#endif
        }
    }
}
