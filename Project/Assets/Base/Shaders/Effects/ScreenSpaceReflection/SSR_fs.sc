$input v_texCoord0

#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D(s_depthTexture, 0);
SAMPLER2D(s_sceneTexture, 1);
SAMPLER2D(s_normalTexture, 2);
SAMPLERCUBE(s_environmentTexture, 3);
SAMPLERCUBE(s_environmentTextureIBL, 4);
SAMPLER2D(s_brdfLUT, 5);
SAMPLER2D(s_gBuffer2Texture, 6);
SAMPLER2D(s_gBuffer0Texture, 7);

uniform mat4 invViewProj;

uniform vec4 edgeFactorPower;
uniform vec4 colorTextureSize;
uniform vec4 zNear;
uniform vec4 zFar;
uniform vec4 fov;
uniform vec4 aspectRatio;
uniform vec4 cameraPosition;

#include "../../PBRFilament/common_types.sh"
#include "../../PBRFilament/common_math.sh"
#include "../../PBRFilament/brdf.sh"
#include "../../PBRFilament/PBRFilament.sh"

float getDepth(vec2 coord)
{
	float zdepth = texture2D(s_depthTexture, coord).x;
	return getDepthValue(zdepth, zNear.x, zFar.x);
}

vec3 getViewPosition(vec2 coord)
{
	vec3 pos;
	pos =  vec3((coord.x * 2.0 - 1.0) / (90.0 / fov.x), (coord.y * 2.0 - 1.0) / aspectRatio.x / (90.0 / fov.x), 1.0);
	return (pos * getDepth(coord));
}

vec3 getViewNormal(vec2 coord)
{
	float pW = 1.0 / colorTextureSize.x;
	float pH = 1.0 / colorTextureSize.y;

	vec3 p1  = getViewPosition(coord + vec2(pW, 0.0)).xyz;
	vec3 p2  = getViewPosition(coord + vec2(0.0, pH)).xyz;
	vec3 p3  = getViewPosition(coord + vec2(-pW, 0.0)).xyz;
	vec3 p4  = getViewPosition(coord + vec2(0.0, -pH)).xyz;

	vec3 vP  = getViewPosition(coord);

	vec3 dx  = vP - p1;
	vec3 dy  = p2 - vP;
	vec3 dx2 = p3 - vP;
	vec3 dy2 = vP - p4;

	if (length(dx2) < length(dx) && coord.x - pW >= 0.0 || coord.x + pW > 1.0)
		dx = dx2;
	if (length(dy2) < length(dy) && coord.y - pH >= 0.0 || coord.y + pH > 1.0)
		dy = dy2;

	return normalize(cross(dx, dy));
}

vec2 getScreenCoord(vec3 pos)
{
	vec3 norm = pos / pos.z;
	vec2 view = vec2((norm.x * (90.0 / fov.x) + 1.0) / 2.0, (norm.y * (90.0 / fov.x) * aspectRatio.x + 1.0) / 2.0);
	return view;
}

vec2 snapToPixel(vec2 coord)
{
	coord.x = (floor(coord.x *  colorTextureSize.x) + 0.5) /  colorTextureSize.x;
	coord.y = (floor(coord.y * colorTextureSize.y) + 0.5) / colorTextureSize.y;
	return coord;
}

//#define stepSize   0.02 // step size for raymarching
//#define maxsteps   50   // maximum amount of steps for raymarching
//#define stepSize   0.005 // step size for raymarching
//#define maxsteps   200   // maximum amount of steps for raymarching
//!!!!
#define startScale 4.0   // initial scale of step size for raymarching
#define depth      0.5   // thickness of the world

vec3 raymarch(vec3 position, vec3 direction)
{
	float stepSize = 1.0f / (float)MAX_STEPS;	
	
	direction = direction * stepSize;//stepSize;
	float stepScale = startScale;

	for (int steps = 0; steps < MAX_STEPS/*maxsteps*/; steps++)
	{
		vec3 deltapos = direction * stepScale * position.z;
		position += deltapos;
		vec2 screencoord = getScreenCoord(position);

		bool OOB = false; // OUT OF BOUNDS
		OOB = OOB || (screencoord.x < 0.0) || (screencoord.x > 1.0); // X
		OOB = OOB || (screencoord.y < 0.0) || (screencoord.y > 1.0); // Y
		OOB = OOB || (position.z >  zFar.x ) || (position.z <  zNear.x); // Z
		if (OOB)
			return vec3_splat(0);

		screencoord = snapToPixel(screencoord);
		float penetration = length(position) - length(getViewPosition(screencoord));

		if (penetration > 0.0)
		{
			if (stepScale > 1.0)
			{
				position -= deltapos;
				stepScale *= 0.5;
			}
			else if (penetration < depth)
				return position;
		}
	}

	return vec3_splat(0);
}

vec3 glossyReflection(vec3 position, vec3 normal, vec3 view, vec3 specIBL)
{
	vec3 skyColor = saturate(specIBL);

	const float fresnel_multiplier = 2.8;
	const float fresnel_pow = 2.0;

	vec3 reflected   = normalize(reflect(view, normal));
	vec3 collision   = raymarch(position, reflected);
	vec2 screenCoord = getScreenCoord(collision);

	vec2 vCoordsEdgeFact = vec2(1.0, 1.0) - pow(saturate(abs(screenCoord.xy - vec2(0.5, 0.5)) * 2.0), edgeFactorPower.x);
	float fScreenEdgeFactor = saturate(min(vCoordsEdgeFact.x, vCoordsEdgeFact.y));

	float fresnel = saturate(fresnel_multiplier * pow(1.0 + dot(view, normal), fresnel_pow));

	float ssr_attenuation = saturate(reflected.z) * fresnel * fScreenEdgeFactor;

	vec3 reflection = mix(skyColor, saturate(texture2D(s_sceneTexture, screenCoord).rgb), ssr_attenuation);

	float skyAmount = 0.0;
	if (collision.z == 0.0)
		skyAmount = 1.0;

	return mix(reflection, skyColor, skyAmount);
}

vec3 specularIBL(vec2 texCoords)
{
	//!!!!
	vec3 baseColor = decodeRGBE8(texture2D(s_gBuffer0Texture, texCoords));
	//vec3 baseColor = texture2D(s_gBuffer0Texture, texCoords).rgb;
	//vec3 baseColor = texture2D(s_colorTexture, texCoords).rgb;
	vec4 gBufferData = texture2D(s_gBuffer2Texture, texCoords);

	vec4 normal_and_reflectance = texture2D(s_normalTexture, texCoords);

	vec3 normal = normalize(normal_and_reflectance.rgb * 2 - 1);

	float rawDepth = texture2D(s_depthTexture, texCoords).r;
	vec3 worldPosition = reconstructWorldPosition(invViewProj, texCoords, rawDepth);
	vec3 toCamera = normalize(cameraPosition.xyz - worldPosition);
	vec3 toLight = normal;

	MaterialInputs material;
	material.baseColor        = vec4(baseColor, 0.0);
	material.roughness        = gBufferData.y;
	material.metallic         = gBufferData.x;
	material.reflectance      = normal_and_reflectance.a;
	material.ambientOcclusion = gBufferData.z;

	material.anisotropy = 0.0f;
	material.anisotropyDirection = vec3_splat(0);

	material.clearCoat = 0;
	material.clearCoatRoughness = 0;
	material.clearCoatNormal = vec3_splat(0);

	setupPBRFilamentParams(material, vec3_splat(0), vec3_splat(0), normal, normal, toLight, toCamera, false);//gl_FrontFacing);

	PixelParams pixel;
	getPBRFilamentPixelParams(material, pixel);

	EnvironmentTextureData data;
	data.rotation = mat3(1,0,0,0,1,0,0,0,1);
	data.multiplier = vec3_splat(1);
	
	return getSpecularIrradiance(pixel, s_environmentTexture, data);
	//return iblDiffuse(material, pixel) + iblSpecular(material, pixel, vec3_splat(0), 0);
}

void main()
{
	vec3 specIBL = specularIBL(v_texCoord0);

	vec3 position = getViewPosition(v_texCoord0);
	vec3 normal = getViewNormal(v_texCoord0);
	vec3 view = normalize(position);

	vec3 reflection = glossyReflection(position, normal, view, specIBL);
	
	gl_FragColor = vec4(reflection.rgb, 0.0);
}