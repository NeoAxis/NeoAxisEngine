/*
* Copyright (c) 2012-2020 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

namespace Internal.Assimp.Unmanaged
{
    /// <summary>
    /// Static class containing material key constants. A fully qualified mat key
    /// name here means that it's a string that combines the mat key (base) name, its
    /// texture type semantic, and its texture index into a single string delimited by
    /// commas. For non-texture material properties, the texture type semantic and texture
    /// index are always zero.
    /// </summary>
    public static class AiMatKeys
    {
        /// <summary>
        /// Material name (String)
        /// </summary>
        public const string NAME_BASE = "?mat.name";

        /// <summary>
        /// Material name (String)
        /// </summary>
        public const string NAME = "?mat.name,0,0";

        /// <summary>
        /// Two sided property (boolean)
        /// </summary>
        public const string TWOSIDED_BASE = "$mat.twosided";

        /// <summary>
        /// Two sided property (boolean)
        /// </summary>
        public const string TWOSIDED = "$mat.twosided,0,0";

        /// <summary>
        /// Shading mode property (ShadingMode)
        /// </summary>
        public const string SHADING_MODEL_BASE = "$mat.shadingm";

        /// <summary>
        /// Shading mode property (ShadingMode)
        /// </summary>
        public const string SHADING_MODEL = "$mat.shadingm,0,0";

        /// <summary>
        /// Enable wireframe property (boolean)
        /// </summary>
        public const string ENABLE_WIREFRAME_BASE = "$mat.wireframe";

        /// <summary>
        /// Enable wireframe property (boolean)
        /// </summary>
        public const string ENABLE_WIREFRAME = "$mat.wireframe,0,0";

        /// <summary>
        /// Blending function (BlendMode)
        /// </summary>
        public const string BLEND_FUNC_BASE = "$mat.blend";

        /// <summary>
        /// Blending function (BlendMode)
        /// </summary>
        public const string BLEND_FUNC = "$mat.blend,0,0";

        /// <summary>
        /// Opacity (float)
        /// </summary>
        public const string OPACITY_BASE = "$mat.opacity";

        /// <summary>
        /// Opacity (float)
        /// </summary>
        public const string OPACITY = "$mat.opacity,0,0";

        /// <summary>
        /// Transparency Factor (float)
        /// </summary>
        public const string TRANSPARENCYFACTOR_BASE = "$mat.transparencyfactor";

        /// <summary>
        /// Transparency Factor (float)
        /// </summary>
        public const string TRANSPARENCYFACTOR = "$mat.transparencyfactor,0,0";

        /// <summary>
        /// Bumpscaling (float)
        /// </summary>
        public const string BUMPSCALING_BASE = "$mat.bumpscaling";

        /// <summary>
        /// Bumpscaling (float)
        /// </summary>
        public const string BUMPSCALING = "$mat.bumpscaling,0,0";

        /// <summary>
        /// Shininess (float)
        /// </summary>
        public const string SHININESS_BASE = "$mat.shininess";

        /// <summary>
        /// Shininess (float)
        /// </summary>
        public const string SHININESS = "$mat.shininess,0,0";

        /// <summary>
        /// Reflectivity (float)
        /// </summary>
        public const string REFLECTIVITY_BASE = "$mat.reflectivity";

        /// <summary>
        /// Reflectivity (float)
        /// </summary>
        public const string REFLECTIVITY = "$mat.reflectivity,0,0";

        /// <summary>
        /// Shininess strength (float)
        /// </summary>
        public const string SHININESS_STRENGTH_BASE = "$mat.shinpercent";

        /// <summary>
        /// Shininess strength (float)
        /// </summary>
        public const string SHININESS_STRENGTH = "$mat.shinpercent,0,0";

        /// <summary>
        /// Refracti (float)
        /// </summary>
        public const string REFRACTI_BASE = "$mat.refracti";

        /// <summary>
        /// Refracti (float)
        /// </summary>
        public const string REFRACTI = "$mat.refracti,0,0";

        /// <summary>
        /// Diffuse color (Vector4)
        /// </summary>
        public const string COLOR_DIFFUSE_BASE = "$clr.diffuse";

        /// <summary>
        /// Diffuse color (Vector4)
        /// </summary>
        public const string COLOR_DIFFUSE = "$clr.diffuse,0,0";

        /// <summary>
        /// Ambient color (Vector4)
        /// </summary>
        public const string COLOR_AMBIENT_BASE = "$clr.ambient";

        /// <summary>
        /// Ambient color (Vector4)
        /// </summary>
        public const string COLOR_AMBIENT = "$clr.ambient,0,0";

        /// <summary>
        /// Specular color (Vector4)
        /// </summary>
        public const string COLOR_SPECULAR_BASE = "$clr.specular";

        /// <summary>
        /// Specular color (Vector4)
        /// </summary>
        public const string COLOR_SPECULAR = "$clr.specular,0,0";

        /// <summary>
        /// Emissive color (Vector4)
        /// </summary>
        public const string COLOR_EMISSIVE_BASE = "$clr.emissive";

        /// <summary>
        /// Emissive color (Vector4)
        /// </summary>
        public const string COLOR_EMISSIVE = "$clr.emissive,0,0";

        /// <summary>
        /// Transparent color (Vector4)
        /// </summary>
        public const string COLOR_TRANSPARENT_BASE = "$clr.transparent";

        /// <summary>
        /// Transparent color (Vector4)
        /// </summary>
        public const string COLOR_TRANSPARENT = "$clr.transparent,0,0";

        /// <summary>
        /// Reflective color (Vector4)
        /// </summary>
        public const string COLOR_REFLECTIVE_BASE = "$clr.reflective";

        /// <summary>
        /// Reflective color (Vector4)
        /// </summary>
        public const string COLOR_REFLECTIVE = "$clr.reflective,0,0";

        /// <summary>
        /// Background image (String)
        /// </summary>
        public const string GLOBAL_BACKGROUND_IMAGE_BASE = "?bg.global";

        /// <summary>
        /// Background image (String)
        /// </summary>
        public const string GLOBAL_BACKGROUND_IMAGE = "?bg.global,0,0";

        /// <summary>
        /// Texture base name
        /// </summary>
        public const string TEXTURE_BASE = "$tex.file";

        /// <summary>
        /// UVWSRC base name
        /// </summary>
        public const string UVWSRC_BASE = "$tex.uvwsrc";

        /// <summary>
        /// Texture op base name
        /// </summary>
        public const string TEXOP_BASE = "$tex.op";

        /// <summary>
        /// Mapping base name
        /// </summary>
        public const string MAPPING_BASE = "$tex.mapping";

        /// <summary>
        /// Texture blend base name.
        /// </summary>
        public const string TEXBLEND_BASE = "$tex.blend";

        /// <summary>
        /// Mapping mode U base name
        /// </summary>
        public const string MAPPINGMODE_U_BASE = "$tex.mapmodeu";

        /// <summary>
        /// Mapping mode V base name
        /// </summary>
        public const string MAPPINGMODE_V_BASE = "$tex.mapmodev";

        /// <summary>
        /// Texture map axis base name
        /// </summary>
        public const string TEXMAP_AXIS_BASE = "$tex.mapaxis";

        /// <summary>
        /// UV transform base name
        /// </summary>
        public const string UVTRANSFORM_BASE = "$tex.uvtrafo";

        /// <summary>
        /// Texture flags base name
        /// </summary>
        public const string TEXFLAGS_BASE = "$tex.flags";

        /// <summary>
        /// Shader language type (string)
        /// </summary>
        public const string GLOBAL_SHADERLANG_BASE = "?sh.lang";

        /// <summary>
        /// Shader language type (string)
        /// </summary>
        public const string GLOBAL_SHADERLANG = "?sh.lang,0,0";

        /// <summary>
        /// Vertex shader source code (string)
        /// </summary>
        public const string SHADER_VERTEX_BASE = "?sh.vs";

        /// <summary>
        /// Vertex shader source code (string)
        /// </summary>
        public const string SHADER_VERTEX = "?sh.vs,0,0";

        /// <summary>
        /// Fragment/Pixel shader source code (string)
        /// </summary>
        public const string SHADER_FRAGMENT_BASE = "?sh.fs";

        /// <summary>
        /// Fragment/Pixel shader source code (string)
        /// </summary>
        public const string SHADER_FRAGMENT = "?sh.fs,0,0";

        /// <summary>
        /// Geometry shader source code (string)
        /// </summary>
        public const string SHADER_GEO_BASE = "?sh.gs";

        /// <summary>
        /// Geometry shader source code (string)
        /// </summary>
        public const string SHADER_GEO = "?sh.gs,0,0";

        /// <summary>
        /// Tesselation shader source code (string)
        /// </summary>
        public const string SHADER_TESSELATION_BASE = "?sh.ts";

        /// <summary>
        /// Tesselation shader source code (string)
        /// </summary>
        public const string SHADER_TESSELATION = "?sh.ts,0,0";

        /// <summary>
        /// Primitive/Domain shader source code (string)
        /// </summary>
        public const string SHADER_PRIMITIVE_BASE = "?sh.ps";

        /// <summary>
        /// Primitive/Domain shader source code (string)
        /// </summary>
        public const string SHADER_PRIMITIVE = "?sh.ps,0,0";

        /// <summary>
        /// Compute shader source code (string)
        /// </summary>
        public const string SHADER_COMPUTE_BASE = "?sh.cs";

        /// <summary>
        /// Compute shader source code (string)
        /// </summary>
        public const string SHADER_COMPUTE = "?sh.cs,0,0";

        /// <summary>
        /// Helper function to get the fully qualified name of a texture property type name. Takes
        /// in a base name constant, a texture type, and a texture index and outputs the name in the format:
        /// <para>"baseName,TextureType,texIndex"</para>
        /// </summary>
        /// <param name="baseName">Base name</param>
        /// <param name="texType">Texture type</param>
        /// <param name="texIndex">Texture index</param>
        /// <returns>Fully qualified texture name</returns>
        public static string GetFullTextureName(string baseName, TextureType texType, int texIndex)
        {
            return $"{baseName},{(int)texType},{texIndex}";
        }

        /// <summary>
        /// Helper function to get the base name from a fully qualified name of a material property type name. The format
        /// of such a string is:
        /// <para>"baseName,TextureType,texIndex"</para>
        /// </summary>
        /// <param name="fullyQualifiedName">Fully qualified material property name.</param>
        /// <returns>Base name of the property type.</returns>
        public static string GetBaseName(string fullyQualifiedName)
        {
            if(string.IsNullOrEmpty(fullyQualifiedName))
                return string.Empty;

            string[] substrings = fullyQualifiedName.Split(',');
            if(substrings != null && substrings.Length == 3)
                return substrings[0];

            return string.Empty;
        }
    }
}
