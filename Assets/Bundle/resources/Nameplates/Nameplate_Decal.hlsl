#ifndef _NAMEPLATE_DECAL_HLSL_
#define _NAMEPLATE_DECAL_HLSL_

float remap(float value, float from1, float from2, float to1, float to2) {
	return (value - from1) / (from2 - from1) * (to2 - to1) + to1;
}

void NameplateDecal_float(Texture2D decal, SamplerState ss, float2 uv, float4 srcUV, float4 destUV, out float4 outColor) {
	uv = float2(remap(uv.x, destUV.x, destUV.z, srcUV.x, srcUV.z), remap(uv.y, destUV.y, destUV.w, srcUV.y, srcUV.w));
	if(uv.x < srcUV.x || uv.x > srcUV.z || uv.y < srcUV.y || uv.y > srcUV.w) { 
		outColor = float4(0, 0, 0, 0);
	} else {
		outColor = SAMPLE_TEXTURE2D(decal, ss, uv); 
	}
}

#endif