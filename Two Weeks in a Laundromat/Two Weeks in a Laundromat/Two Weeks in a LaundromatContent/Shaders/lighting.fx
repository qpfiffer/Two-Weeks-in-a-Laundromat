float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldViewProj;

// POINT LIGHT VARIABLES
float3 LightPos;
float LightPower;
float LightDistanceSquared;

// Texture that gets mapped onto surfaces:
Texture UsedTexture;

sampler TextureSampler = sampler_state { 
	texture = <UsedTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

// FUNCTIONS
// ---------------------------------------------
float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}
// ---------------------------------------------


// STRUCTURES
// ---------------------------------------------
struct VertexToPixel
{
    float4 Position     : POSITION;    
    float2 TexCoords    : TEXCOORD0;
    float3 Normal        : TEXCOORD1;
    float3 Position3D    : TEXCOORD2;
};
// ---------------------------------------------


// POINT LIGHT
// ---------------------------------------------
VertexToPixel PointLightVS(float4 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float3 inNormal : NORMAL) {
	VertexToPixel Output = (VertexToPixel)0;

	Output.Normal = normalize(mul(inNormal, (float3x3)World));    
	Output.Position3D = mul(inPos, World);
	Output.TexCoords = inTexCoords;
	Output.Position = mul(inPos, WorldViewProj);

	return Output;
}

float4 PointLightPS(VertexToPixel PSIn) : COLOR0
{   
	float diffuseLightingFactor = DotProduct(LightPos, PSIn.Position3D, PSIn.Normal);
	//diffuseLightingFactor = saturate(diffuseLightingFactor);
	
	// Introduce fall-off of light intensity
	//diffuseLightingFactor *= (LightDistanceSquared / dot(LightPos - PSIn.Position3D, LightPos - PSIn.Position3D));
	// Limit it a bit
	//diffuseLightingFactor = clamp(diffuseLightingFactor, 0, xLightPower);
	//diffuseLightingFactor = saturate(diffuseLightingFactor);

	float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
	float baseColorAlpha = baseColor.a;
	baseColor *= diffuseLightingFactor;
	//float greyscale   = dot(baseColor, float3(0.20, 0.49, 0.01));     
    //baseColor         = lerp(greyscale, baseColor, 0.05);
	baseColor.a = baseColorAlpha;
    return baseColor;
}
// ---------------------------------------------

// WORLD LIGHTING
// ---------------------------------------------
technique WorldLighting
{
	//pass AmbientLight
    //{
		//VertexShader = compile vs_2_0 AmbientLightVS();
        //PixelShader = compile ps_2_0 AmbientLightPS();
    //}
	pass PointLight
    {
		VertexShader = compile vs_2_0 PointLightVS();
        PixelShader = compile ps_2_0 PointLightPS();
    }
}
// ---------------------------------------------
