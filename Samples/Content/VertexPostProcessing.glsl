#version 330

layout(location = 0) in vec4 vPosition;

out vec2 fTexCoord;

void main()
{
	fTexCoord = 0.5f * (vPosition.xy + vec2(1.0f, 1.0f));
	gl_Position = vec4(vPosition.xy, 0.0f, 1.0f);
}