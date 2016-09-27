#version 330

uniform mat4 View;
uniform mat4 Projection;
uniform mat4 Model;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 3) in mat4 vModel;

out vec2 fTexCoord;
out vec3 fNormal;

void main()
{
	vec4 worldPosition = vModel * vec4(vPosition, 1.0f);
	vec4 worldNormal = vModel * vec4(vNormal, 0.0f);
	fTexCoord = 0.5f * (vPosition.xy + vec2(1.0f, 1.0f));
	fNormal = worldNormal.xyz;
	gl_Position = Projection * View * worldPosition;
}