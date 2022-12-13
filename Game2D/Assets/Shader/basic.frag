#version 460

in vec2 texCoords;

layout(binding = 0) uniform sampler2D inputTexture;

uniform vec2 imageSize;
uniform vec2 texturePosition;
uniform bool flipped;

layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 NormalColor;
layout (location = 2) out vec4 FragPos;

void main()
{
	vec2 uv = texCoords;
	if (flipped)
		uv.x = 1 - uv.x;

	FragColor = texture2D(inputTexture, uv / imageSize + vec2((1.0f / imageSize.x) * texturePosition.x, (1.0f / imageSize.y) * texturePosition.y));
}