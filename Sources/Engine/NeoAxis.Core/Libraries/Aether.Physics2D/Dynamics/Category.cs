/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

/*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace tainicom.Aether.Physics2D.Dynamics
{
    [Flags]
    public enum Category
    {
        None  = 0x00000000,        
        Category1  = 0x00000001,
        Category2  = 0x00000002,
        Category3  = 0x00000004,
        Category4  = 0x00000008,
        Category5  = 0x00000010,
        Category6  = 0x00000020,
        Category7  = 0x00000040,
        Category8  = 0x00000080,
        Category9  = 0x00000100,
        Category10 = 0x00000200,
        Category11 = 0x00000400,
        Category12 = 0x00000800,
        Category13 = 0x00001000,
        Category14 = 0x00002000,
        Category15 = 0x00004000,
        Category16 = 0x00008000,
        Category17 = 0x00010000,
        Category18 = 0x00020000,
        Category19 = 0x00040000,
        Category20 = 0x00080000,
        Category21 = 0x00100000,
        Category22 = 0x00200000,
        Category23 = 0x00400000,
        Category24 = 0x00800000,
        Category25 = 0x01000000,
        Category26 = 0x02000000,
        Category27 = 0x04000000,
        Category28 = 0x08000000,
        Category29 = 0x10000000,
        Category30 = 0x20000000,
        Category31 = 0x40000000,
        All = int.MaxValue,
    }
}
