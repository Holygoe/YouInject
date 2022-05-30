﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class SceneScopeBuilder : ISceneScopeBuilder
    {
        private readonly string _sceneId;
        private readonly Scope _parentScope;
        private readonly BakedServiceCollection _services;
        private readonly Dictionary<Type, Component> _components;

        public SceneScopeBuilder(string sceneId, IScope parentScope, BakedServiceCollection services)
        {
            _sceneId = sceneId;

            if (parentScope is not Scope scope)
            {
                throw new Exception($"Cannot create {nameof(SceneScopeBuilder)}. {nameof(parentScope)} must be '{nameof(ServiceScope)}' type.");
            }

            _parentScope = scope;
            _services = services;
            _components = new Dictionary<Type, Component>();
        }
        
        public SceneScope? Result { get; private set; }
        
        public  bool IsUsed { get; private set; }

        public event Action<SceneScope?>? Completed;

        public void AddComponents(Component[] components)
        {
            foreach (var component in components)
            {
                AddComponent(component);
            }
        }

        public SceneScope BuildScope()
        {
            Assert.IsFalse(IsUsed, "Failed to build a scope, the builder has already been used.");

            IsUsed = true;
            var scope = _parentScope.CreateDerivedSceneScope(_sceneId);
            scope.ComponentProvider.AddComponents(_components);
            Result = scope;
            Completed?.Invoke(scope);
            return scope;
        }

        public void Dispose()
        {
            if (IsUsed) return;

            IsUsed = true;
            Completed?.Invoke(null);
        }
        
        private void AddComponent(Component component)
        {
            Assert.IsFalse(IsUsed, $"Failed to inject a component of the '{component.GetType().Name}' " +
                                      "type, the builder has already been used.");

            if (!_services.TryGetServiceTypeByDecision(component, out var serviceType))
            {
                throw new Exception($"Failed a component injection because a service with a decision of the " +
                                    $"{component.GetType().Name} type is not registered with the host.");
            }

            _components.Add(serviceType, component);
        }
    }
}