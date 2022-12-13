#version 460
out vec4 FragColor;

in vec2 texCoords;
in vec3 color;

in float life;
in flat int isAlive;

void main()
{
    FragColor = vec4(color, 1.0f);
}