#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

Texture2D tex;
sampler2D texture_sampler = sampler_state
{
	Texture = <tex>;
};

struct VertexShaderInput
{
	float4 pos : POSITION0;
	float4 color : COLOR0;
	float2 tex : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 pos : SV_POSITION;
	float4 color : COLOR0;
	float2 tex : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.pos = mul(input.pos, WorldViewProjection);
	output.color = input.color;
	output.tex = input.tex;
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 pos = (2*input.tex.xy - float2(1.,1.));

	float px = pos.x * pos.x;
	float py = pos.y * pos.y;
	
	float d = pow(px * px + py * py, .25) - .9;
	float s =  smoothstep((2./50.), .0, d);
	
	float3 cor = input.color.rgb;
	//if ((pos.x < -.9 || pos.x>.9) || (pos.y < -.9 || pos.y>.9)) s = 1.;
	return  float4(cor.r, cor.g, cor.b, s);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};