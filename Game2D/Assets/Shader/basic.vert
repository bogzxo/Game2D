#version 460

layout (location = 0) in vec3 aPos; 
layout (location = 1) in vec2 aTexCoords; 

uniform mat4 mvp;

out vec2 fragPos;
out vec2 texCoords;

void main() {
	fragPos = (vec4(aPos, 1.0f)).xy;
	texCoords = aTexCoords;
	gl_Position = vec4(aPos, 1.0f) * mvp;
}