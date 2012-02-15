float2 screenVector;

sampler2D input : register(s0); 
float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
    float4 toReturn;
	float4 leftColor;
	float4 midColor;
	float4 rightColor;

	// Double vision effect:
	midColor = tex2D(input , uv.xy);

	uv.xy = uv.xy - (0.015 * screenVector.xy);
	leftColor = tex2D(input , uv.xy);
	leftColor.r = 0.4;

	uv.xy = uv.xy + (0.03 * screenVector.xy);
	rightColor = tex2D(input , uv.xy);
	rightColor.b = 0.4;

	toReturn = lerp(midColor, lerp(leftColor, rightColor, 0.5), 0.5);
    return toReturn; 
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}
