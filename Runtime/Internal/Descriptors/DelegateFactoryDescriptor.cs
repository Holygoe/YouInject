﻿using System;

namespace InjectReady.YouInject.Internal
{
    internal partial class DelegateFactoryDescriptor : IServiceDescriptor
    {
        private readonly Type _productInstanceType;
        private readonly Func<ContextualServiceProvider, object> _instanceFactory;
        
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
        
        internal DelegateFactoryDescriptor(Type delegateType, Type productInstanceType, ServiceLifetime lifetime)
        {
            if (delegateType.BaseType != typeof(MulticastDelegate))
            {
                throw new ArgumentException($"The '{delegateType.Name}' type is not a delegate.", nameof(delegateType));
            }

            _productInstanceType = productInstanceType;
            ServiceType = delegateType;
            Lifetime = lifetime;
            _instanceFactory = GetInstanceFactory();
        }

        public object ResolveService(ContextualServiceProvider serviceProvider)
        {
            var service = _instanceFactory.Invoke(serviceProvider);
            return service;
        }

        private Func<ContextualServiceProvider, object> GetInstanceFactory()
        {
            var factoryBuilder = new FactoryBuilder(this);
            
            return serviceProvider =>
            {
                var factoryDelegate = factoryBuilder.GetFactoryDelegate(serviceProvider);
                return factoryDelegate;
            };
        }
    }
}