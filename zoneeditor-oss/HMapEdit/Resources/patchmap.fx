
///////////////////////////////////////////////////////////////////////////////////////////////////////
// PatchMap Shader (c) 2009 TheKrokodil
// FORMAT = ANSI (Important, otherwise the shader compiler will fail!
///////////////////////////////////////////////////////////////////////////////////////////////////////

uniform Texture tex;
uniform Texture texR;
uniform Texture texG;
uniform Texture texB;

uniform sampler sam = sampler_state { texture = <tex>; minfilter = LINEAR; mipfilter = LINEAR; magfilter = LINEAR; AddressU = CLAMP; AddressV = CLAMP; AddressW = CLAMP; };
uniform sampler samR = sampler_state { texture = <texR>; minfilter = LINEAR; mipfilter = LINEAR; magfilter = LINEAR;};
uniform sampler samG = sampler_state { texture = <texG>; minfilter = LINEAR; mipfilter = LINEAR; magfilter = LINEAR;};
uniform sampler samB = sampler_state { texture = <texB>; minfilter = LINEAR; mipfilter = LINEAR; magfilter = LINEAR;};
  
uniform float numR;   
uniform float numG; 
uniform float numB;

uniform int layer;

void patch_r(in float2 gTC : TEXCOORD0, in float2 lTC : TEXCOORD1, out float4 color : COLOR0)
{
	float4 patch = tex2D(sam, lTC); 
	color = tex2D(samR, mul(gTC, numR)); 
	if (layer > 0) color.a = patch.x;
}
void patch_g(in float2 gTC : TEXCOORD0, in float2 lTC : TEXCOORD1, out float4 color : COLOR0)
{
	float4 patch = tex2D(sam, lTC); 
	color = tex2D(samG, mul(gTC, numG)); 
	color.a = patch.y;
}
void patch_b(in float2 gTC : TEXCOORD0, in float2 lTC : TEXCOORD1, out float4 color : COLOR0)
{
	float4 patch = tex2D(sam, lTC); 
	color = tex2D(samB, mul(gTC, numB)); 
	color.a = patch.z;
}

///////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////

uniform float2 cursor;
uniform float radius;
uniform int mode;
uniform bool border;

float4 effects(in float2 gTC : TEXCOORD0, in float2 lTC : TEXCOORD1) : COLOR0
{
	float4 res;
	res.rgb = 1;
	res.a = 0;

	float dist = distance(cursor, gTC);
	if (dist < radius)
	{
		float fac = 1.0;
		
		if (mode == 1) fac = 1 - (dist/radius); //Linear
		if (mode == 2) fac = 1 - pow(dist/radius, 2); //Cubic
		
		res.a = res.a + fac/2;
	}
	
	if (border)
	{
		if (lTC.x < 0.001 || lTC.y < 0.001)
			res.a = res.a + 0.25;
	}
	
	return res;
}


///////////////////////////////////////////////////////////////////////////////////////////////////////

technique PM
{
	pass P0
    {
         PixelShader  = compile ps_2_0 effects();
    }
    pass P1
    {
        PixelShader  = compile ps_2_0 patch_r();
    }
    pass P2
    {
        PixelShader  = compile ps_2_0 patch_g();
    }
    pass P3
    {
        PixelShader  = compile ps_2_0 patch_b();
    }
}

