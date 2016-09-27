#version 330

uniform sampler2D TextureColor;
uniform sampler2D TextureDepth;
uniform float NearPlane;
uniform float FarPlane;

in vec2 fTexCoord;

out vec4 color;

float linearizeDepth(float depth)
{
	return(2.0f * NearPlane) / (FarPlane + NearPlane - depth * (FarPlane - NearPlane));
}

void main()
{
	vec3 colorSample  = texture(TextureColor, fTexCoord).xyz;
	float depthSample = texture(TextureDepth, fTexCoord).r;

	vec3 components[3] = vec3[]
	(
		colorSample,
		vec3((colorSample.x + colorSample.y + colorSample.z) / 3.0f),
		vec3(linearizeDepth(depthSample))
	);
	
	int index = clamp(int(3.0f * fTexCoord.x + 0.1f * (fTexCoord.y - 0.5f)), 0, 2);
	color = vec4(components[index], 1.0f);
}