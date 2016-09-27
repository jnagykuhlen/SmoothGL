#version 330

uniform mat4 View;
uniform mat4 Projection;

layout(location = 0) in vec3 vPosition;

out vec3 fPosition;

void main()
{
	fPosition = vPosition;
	vec3 viewPosition = (View * vec4(vPosition, 0.0f)).xyz;
	gl_Position = Projection * vec4(viewPosition, 1.0f);
}