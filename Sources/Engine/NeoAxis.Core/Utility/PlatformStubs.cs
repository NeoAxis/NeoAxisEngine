// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;

#if NETSTANDARD2_1
namespace System
{
	public sealed class SuppressGCTransitionAttribute : Attribute
    {
    }

    public sealed class UnmanagedCallersOnlyAttribute : Attribute
    {
        public UnmanagedCallersOnlyAttribute()
        {
        }

        public Type[] CallConvs;
        public string EntryPoint;
    }
}

#endif
