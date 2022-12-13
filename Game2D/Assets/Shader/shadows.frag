#version 460

in vec2 texCoords;
in vec2 fragPos;


layout(binding = 0) uniform sampler2D inputTexture;
layout(binding = 1) uniform sampler2D shadowTexture;

uniform vec2 iMouse;

layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 NormalColor;
layout (location = 2) out vec4 FragPos;


struct Light
{
    vec2 position;
    vec3 color;
    float intensity;
};

uniform Light[16] Lights;
uniform int lightCount;

vec3 lightScene()
{
    vec3 color;

    for (int i = 0; i < lightCount; i++)
    {
        Light light = Lights[i];
        //vec2 lightDir = normalize(light.position - fragPos);  

        color += light.color * (1 - distance(light.position, texCoords)) * light.intensity;
    }
    return color;
}

void main()
{
    vec4 baseColor = texture2D(inputTexture, texCoords);

    baseColor.rgb += lightScene();

    FragColor = baseColor;
}