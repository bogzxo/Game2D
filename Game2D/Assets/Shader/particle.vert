#version 460
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aColor;

layout (location = 2) in vec2 aOffset;
layout (location = 3) in float age;
layout (location = 4) in float lifeTime;
layout (location = 5) in vec3 inColor;

out vec2 texCoords;
out vec3 color;
out flat int isAlive;
out float life;


uniform float timer;
uniform mat4 Matrix;

void main()
{
    life = age / lifeTime;
    color = inColor;
    texCoords = aColor;
    isAlive = int(age > lifeTime);
    gl_Position =  vec4(aPos + (aOffset), 0.0, 1.0) * Matrix;
}