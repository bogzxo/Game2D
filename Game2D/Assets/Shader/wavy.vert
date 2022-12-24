#version 460

layout (location = 0) in vec3 aPos; 
layout (location = 1) in vec2 aTexCoords; 

uniform mat4 mvp;
uniform float iTime;

out vec2 fragPos;
out vec2 texCoords;

const float scalex = 0.005f, scaley = 0.001f;
const float speed = 5;

void main() {
	fragPos = (vec4(aPos, 1.0f)).xy;

	 // Calculate the amount of wave based on the x position of the vertex and the current time
	float wave = sin(aPos.x + iTime);

	// Offset the y position of the vertex by the wave amount
	vec3 offset = vec3(0.0f, wave, 0.0f) / 10.0f;

	texCoords = aTexCoords;

    // Random stuff to make it wavy
    texCoords.x += sin ((texCoords.x + texCoords.y) / 2.0f  + iTime * speed + aPos.x) * scalex;
    texCoords.y += cos (texCoords.y + iTime * speed+ aPos.x) * scaley;
 

	gl_Position = vec4(aPos, 1.0f) * mvp;
}