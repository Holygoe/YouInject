﻿using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class ServiceCollection : IServiceCollection
    {
        private readonly List<IRawServiceDescriptor> _descriptors;
        private bool _isBaked;
        
        internal IReadOnlyList<IRawServiceDescriptor> Descriptors
        {
            get
            {
                Assert.IsTrue(_isBaked, "Cannot get the records of the collection. It hasn't been baked yet.");
                return _descriptors;
            }
        }

        internal ServiceCollection()
        {
            _descriptors = new List<IRawServiceDescriptor>();
        }

        public void AddSingleton<TService, TDecision>()
        {
            var serviceType = typeof(TService);
            var decision = typeof(TDecision);
            AddService(serviceType, decision, ServiceLifetime.Singleton);
        }

        public void AddSingleton<TService>()
        {
            var serviceType = typeof(TService);
            AddService(serviceType, serviceType, ServiceLifetime.Singleton);
        }

        public void AddScoped<TService, TDecision>()
        {
            var serviceType = typeof(TService);
            var decision = typeof(TDecision);
            AddService(serviceType, decision, ServiceLifetime.Scoped);
        }

        public void AddScoped<TService>()
        {
            var serviceType = typeof(TService);
            AddService(serviceType, serviceType, ServiceLifetime.Scoped);
        }

        public void AddTransient<TService, TDecision>()
        {
            var serviceType = typeof(TService);
            var decision = typeof(TDecision);
            AddService(serviceType, decision, ServiceLifetime.Transient);
        }

        public void AddTransient<TService>()
        {
            var serviceType = typeof(TService);
            AddService(serviceType, serviceType, ServiceLifetime.Transient);
        }

        public IComponentDescriptorBuilder AddComponent<TService, TDecision>()
        {
            var serviceType = typeof(TService);
            var decision = typeof(TDecision);
            return AddComponent(serviceType, decision);
        }

        public IComponentDescriptorBuilder AddComponent<TService>()
        {
            var serviceType = typeof(TService);
            return AddComponent(serviceType, serviceType);
        }

        public void AddFactory<TFactory, TProduct>()
        {
            Assert.IsFalse(_isBaked, $"Cannot add the {typeof(TFactory).Name}, the host is already built.");
            var serviceType = typeof(TFactory);
            var productType = typeof(TProduct);
            var descriptor = new FactoryDescriptor(serviceType, productType);
            _descriptors.Add(descriptor);
        }

        internal BakedServiceCollection Bake()
        {
            Assert.IsFalse(_isBaked, $"Cannot bake the {nameof(ServiceCollection)}, it has already baked.");
            _isBaked = true;
            var bakedServices = new BakedServiceCollection(this);
            return bakedServices;
        }

        private IComponentDescriptorBuilder AddComponent(Type serviceType, Type decisionType)
        {
            Assert.IsFalse(_isBaked, $"Cannot add the '{serviceType.Name}' component, the host is already built.");
            var descriptor = new ComponentDescriptor(serviceType, decisionType);
            _descriptors.Add(descriptor);
            return descriptor.GetBuilder();
        }

        private void AddService(Type serviceType, Type decisionType, ServiceLifetime lifetime)
        {
            Assert.IsFalse(_isBaked, $"Cannot add the '{serviceType.Name}' service, the host is already built.");
            Assert.IsFalse(_isBaked);
            var descriptor = new ServiceDescriptor(serviceType, decisionType, lifetime);
            _descriptors.Add(descriptor);
        }
    }
}