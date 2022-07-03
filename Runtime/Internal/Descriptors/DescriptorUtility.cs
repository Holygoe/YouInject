﻿using System;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal static class DescriptorUtility
    {
        public static readonly Type MonoBehaviourType = typeof(MonoBehaviour);

        public static bool IsMonoBehavior(Type type)
        {
            return MonoBehaviourType.IsAssignableFrom(type);
        }
    }
}