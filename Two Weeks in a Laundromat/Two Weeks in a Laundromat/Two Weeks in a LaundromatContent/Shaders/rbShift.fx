// These are here so I can set them. 
// They aren't used. The code is blind, like your mother.
float4x4 World;
float4x4 View;
float4x4 Projection;
float lightRadius;
float3 LightPos;
// End bullshit

float2 screenVector;

Texture UsedTexture;

sampler TextureSampler = sampler_state { 
	texture = <UsedTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

struct VertexToPixel
{
    float4 Position			: POSITION;    
    float2 TexCoords		: TEXCOORD0;
    float3 Normal			: TEXCOORD1;
    float3 WorldPosition    : TEXCOORD2;
};

VertexToPixel stupidVS(float3 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float3 inNormal : NORMAL) {
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

float4 uglyPixelShader(VertexToPixel PSIn) : COLOR 
{ 
	float4 leftColor;
	float4 midColor;
	float4 rightColor;
	float4 finalColor;

	float2 uv = PSIn.TexCoords;

	// ANAGLYPH MADNESS:
	midColor = tex2D(TextureSampler , uv.xy);

	uv.xy = uv.xy - (0.015 * screenVector.xy);
	leftColor = tex2D(TextureSampler , uv.xy);
	leftColor.r = 0.4;

	uv.xy = uv.xy + (0.03 * screenVector.xy);
	rightColor = tex2D(TextureSampler , uv.xy);
	rightColor.b = 0.4;

	finalColor = lerp(midColor, lerp(leftColor, rightColor, 0.5), 0.5);

	// POINT LIGHT STUFF:
	float lightDistance = distance(LightPos, PSIn.WorldPosition);
	float3 directionToLight = LightPos - PSIn.WorldPosition;
	float attenuation = saturate(1.0f - (lightDistance/lightRadius));
	directionToLight = normalize(directionToLight);	
	float diffuseIntensity = max(0,dot(PSIn.Normal,directionToLight));
	float4 diffuse = finalColor * diffuseIntensity * attenuation;
	//diffuse = lerp(defaultColor, (0,0,0,0), lightDistance/20);
    
	float4 color = diffuse;
	color.a = finalColor.a;
    return finalColor; 
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 stupidVS();
        PixelShader = compile ps_2_0 uglyPixelShader();
    }
}
