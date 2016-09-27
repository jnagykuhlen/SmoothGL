#version 330

uniform samplerCube TextureSkybox;

in vec3 fPosition;

out vec4 color;

void main()
{
	color = texture(TextureSkybox, fPosition);
}