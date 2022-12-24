#version 460

in vec2 texCoords;

layout(binding = 0) uniform sampler2D inputTexture;

out vec4 FragColor;

void main()
{
	FragColor = texture2D(inputTexture, texCoords);
}