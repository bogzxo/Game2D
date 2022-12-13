#version 460

in vec2 texCoords;

layout(binding = 0) uniform sampler2D inputTexture;


layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 NormalColor;
layout (location = 2) out vec4 FragPos;

float vignette(vec2 uv)
{
    uv *=  1.0 - uv.yx;
    float vig = uv.x*uv.y * 15.0;
    
    vig = pow(vig, 0.25);
    return vig;
}

void main()
{
    vec3 color = texture2D(inputTexture, texCoords).rgb;
    color.rgb *= vignette(texCoords);

	FragColor = vec4(color, 1.0f);
}