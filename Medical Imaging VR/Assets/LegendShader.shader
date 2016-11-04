// shader that performs ray casting using a 3D texture
// adapted from a Cg example by Nvidia
// http://developer.download.nvidia.com/SDK/10/opengl/samples.html
// Gilles Ferrand, University of Manitoba 2016

Shader "Custom/Legend" {

	Properties{
		// the data cube, just so it can load different image data
		[NoScaleOffset] Cadaver_Data("Data Texture", 3D) = "" {}
		// data slicing and thresholding
		_SliceAxis1Min("Slice along axis 1: min", Range(0,1)) = 0
		_SliceAxis1Max("Slice along axis 1: max", Range(0,1)) = 1
		_SliceAxis2Min("Slice along axis 2: min", Range(0,1)) = 0
		_SliceAxis2Max("Slice along axis 2: max", Range(0,1)) = 1
		_SliceAxis3Min("Slice along axis 3: min", Range(0,1)) = 0
		_SliceAxis3Max("Slice along axis 3: max", Range(0,1)) = 1
		_DataMin("Data threshold: min", Range(0,1)) = 0
		_DataMax("Data threshold: max", Range(0,1)) = 1
		// normalization of data intensity (has to be adjusted for each data set, also depends on the number of steps)
		_Normalization("Intensity normalization", Float) = 1
	}

		SubShader{

		Pass{
		Cull Off
		ZTest Always
		ZWrite Off
		Fog{ Mode off }

		CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		sampler3D Legend_Data;
		sampler3D Cadaver_Data;
	float _SliceAxis1Min, _SliceAxis1Max;
	float _SliceAxis2Min, _SliceAxis2Max;
	float _SliceAxis3Min, _SliceAxis3Max;
	float _DataMin, _DataMax;
	float _Normalization;

	// calculates intersection between a ray and a box
	// http://www.siggraph.org/education/materials/HyperGraph/raytrace/rtinter3.htm
	bool IntersectBox(float3 ray_o, float3 ray_d, float3 boxMin, float3 boxMax, out float tNear, out float tFar)
	{
		// compute intersection of ray with all six bbox planes
		float3 invR = 1.0 / ray_d;
		float3 tBot = invR * (boxMin.xyz - ray_o);
		float3 tTop = invR * (boxMax.xyz - ray_o);
		// re-order intersections to find smallest and largest on each axis
		float3 tMin = min(tTop, tBot);
		float3 tMax = max(tTop, tBot);
		// find the largest tMin and the smallest tMax
		float2 t0 = max(tMin.xx, tMin.yz);
		float largest_tMin = max(t0.x, t0.y);
		t0 = min(tMax.xx, tMax.yz);
		float smallest_tMax = min(t0.x, t0.y);
		// check for hit
		bool hit = (largest_tMin <= smallest_tMax);
		tNear = largest_tMin;
		tFar = smallest_tMax;
		return hit;
	}

	struct vert_input {
		float4 pos : POSITION;
	};

	struct frag_input {
		float4 pos : SV_POSITION;
		float3 ray_o : TEXCOORD1; // ray origin
		float3 ray_d : TEXCOORD2; // ray direction
	};

	// vertex program
	frag_input vert(vert_input i)
	{
		frag_input o;

		// calculate eye ray in object space
		o.ray_d = -ObjSpaceViewDir(i.pos);
		o.ray_o = i.pos.xyz - o.ray_d;
		// calculate position on screen (unused)
		o.pos = mul(UNITY_MATRIX_MVP, i.pos);

		return o;
	}
	// tells if the particular position need to be drawn
	bool isBlack(float3 pos) {
		// sample texture (pos is normalized in [0,1])
		float3 pos_righthanded = float3(pos.x,pos.z,pos.y);
		float data = tex3D(Cadaver_Data, pos_righthanded).r; //just r value is fine, since we're dealing with black&white so its either 0 or 1
		data *= step(_SliceAxis1Min, pos.x);
		data *= step(_SliceAxis2Min, pos.y);
		data *= step(_SliceAxis3Min, pos.z);
		data *= step(pos.x, _SliceAxis1Max);
		data *= step(pos.y, _SliceAxis2Max);
		data *= step(pos.z, _SliceAxis3Max);

		//check if DataMin <= data <= DataMax
		data *= step(_DataMin, data);
		data *= step(data, _DataMax);

		//TODO: need another way to do this
		//Currently, it interprets alpha as grayscale
		return (bool)data;
	}

	float4 getCadaverData(float3 pos) {
		// sample texture (pos is normalized in [0,1])
		float3 pos_righthanded = float3(pos.x, pos.z, pos.y);
		float4 data = tex3D(Cadaver_Data, pos_righthanded); //just r value is fine, since we're dealing with black&white so its either 0 or 1
		data *= step(_SliceAxis1Min, pos.x);
		data *= step(_SliceAxis2Min, pos.y);
		data *= step(_SliceAxis3Min, pos.z);
		data *= step(pos.x, _SliceAxis1Max);
		data *= step(pos.y, _SliceAxis2Max);
		data *= step(pos.z, _SliceAxis3Max);

		//check if DataMin <= data <= DataMax
		data *= step(_DataMin, data);
		data *= step(data, _DataMax);

		return data;
	}


	float4 get_data(float3 pos) {
		float3 pos_righthanded = float3(pos.x, pos.z, pos.y);

		//return (isBlack(pos_righthanded)) ? tex3D(Cadaver_Data, pos_righthanded).rgba : float4(0,0,0,0);
		return getCadaverData(pos_righthanded);
		/*
		if (isBlack(pos_righthanded)){
			//get voxel from cadaver
			return tex3D(Cadaver_Data, pos_righthanded).rgba; 
		}
		else{
			//if it's not the region we want just return nothing
			return float4(0, 0, 0, 0);
		}
		*/
	}



#define FRONT_TO_BACK // ray integration order (BACK_TO_FRONT not working when being inside the cube)
#define STEP_CNT 191 // should ideally be at least as large as data resolution, but strongly affects frame rate

	// fragment program
	float4 frag(frag_input i) : COLOR
	{
		i.ray_d = normalize(i.ray_d);
	// calculate eye ray intersection with cube bounding box
	float3 boxMin = { -0.5, -0.5, -0.5 };
	float3 boxMax = { 0.5,  0.5,  0.5 };
	float tNear, tFar;
	bool hit = IntersectBox(i.ray_o, i.ray_d, boxMin, boxMax, tNear, tFar);
	if (!hit) discard;
	if (tNear < 0.0) tNear = 0.0;
	// calculate intersection points
	float3 pNear = i.ray_o + i.ray_d*tNear;
	float3 pFar = i.ray_o + i.ray_d*tFar;
	// convert to texture space
	pNear = pNear + 0.5;
	pFar = pFar + 0.5;

	// march along ray inside the cube, accumulating color
#ifdef FRONT_TO_BACK
	float3 ray_pos = pNear;
	float3 ray_dir = pFar - pNear;
#else
	float3 ray_pos = pFar;
	float3 ray_dir = pNear - pFar;
#endif
	float3 ray_step = normalize(ray_dir) * sqrt(3) / STEP_CNT;
	float4 ray_col = 0;

	[unroll(191)] for (int k = 0; k < STEP_CNT; k++)
	{
		
		float4 voxel_col = get_data(ray_pos);

#ifdef FRONT_TO_BACK

		ray_col.rgb = ray_col.rgb + (1 - ray_col.a) * voxel_col.a * voxel_col.rgb;
		ray_col.a = ray_col.a + (1 - ray_col.a) * voxel_col.a;

#else
		ray_col = (1 - voxel_col.a)*ray_col + voxel_col.a*voxel_col;
#endif
		ray_pos += ray_step;
		//keep it between 0 and 1 for color
		if (ray_pos.x < 0 || ray_pos.y < 0 || ray_pos.z < 0) break;
		if (ray_pos.x > 1 || ray_pos.y > 1 || ray_pos.z > 1) break;
	}
	return ray_col*_Normalization; //gray to white
	}

		ENDCG

	}

	}

		FallBack Off
}