#version 330

uniform sampler2D Texture;

in vec2 fTexCoord;
in vec3 fNormal;

out vec4 color;

void main()
{
	vec4 diffuseColor = texture(Texture, fTexCoord);
	color = normalize(fNormal).z * diffuseColor;
}
