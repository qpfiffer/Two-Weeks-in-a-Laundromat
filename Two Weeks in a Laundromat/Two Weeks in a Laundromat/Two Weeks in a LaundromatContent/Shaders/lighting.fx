float4x4 World;
float4x4 View;
float4x4 Projection;

// POINT LIGHT VARIABLES
float3 LightPos;
float LightPower;

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
    float4 Position			: POSITION;    
    float2 TexCoords		: TEXCOORD0;
    float3 Normal			: TEXCOORD1;
    float3 WorldPosition    : TEXCOORD2;
};
// ---------------------------------------------


// POINT LIGHT
// ---------------------------------------------
VertexToPixel PointLightVS(float3 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float3 inNormal : NORMAL) {
	VertexToPixel output;
	//generate the world-view-projection matrix
	float4x4 wvp = mul(mul(World, View), Projection);
     
	//transform the input position to the output
	output.Position = mul(float4(inPos, 1.0), wvp);

	output.Normal =  mul(inNormal, World);
	float4 worldPosition =  mul(float4(inPos, 1.0), World);
	output.WorldPosition = worldPosition / worldPosition.w;

	output.TexCoords = inTexCoords;
	//return the output structure
	return output;
}

float4 PointLightPS(VertexToPixel PSIn) : COLOR0
{   
	float lightRadius = 15;
	// WORKING:
	// Get the color of the texture
	float4 defaultColor = tex2D(TextureSampler, PSIn.TexCoords);
	// Distance to the light
	float lightDistance = distance(LightPos, PSIn.WorldPosition);
	// The vector to the light
	float3 directionToLight = LightPos - PSIn.WorldPosition;
	// Compute attention
	float attenuation = saturate(1.0f - (lightDistance/lightRadius));
	directionToLight = normalize(directionToLight);	
	float diffuseIntensity = max(0,dot(PSIn.Normal,directionToLight));
	float4 diffuse = defaultColor * diffuseIntensity * attenuation;
	//diffuse = lerp(defaultColor, (0,0,0,0), lightDistance/20);
    
	float4 color = diffuse;
	color.a = defaultColor.w;

    return color;
}
// ---------------------------------------------

// WORLD LIGHTING
// ---------------------------------------------
technique WorldLighting
{
	pass PointLight
    {
		VertexShader = compile vs_2_0 PointLightVS();
        PixelShader = compile ps_2_0 PointLightPS();
    }
}
// ---------------------------------------------
