#version 460

uniform float iTime;
uniform vec2 iResolution;

in vec2 texCoords;

float rand(vec2 coords)
{
	return fract(sin(dot(coords, vec2(56.3456f,78.3456f)) * 5.0f) * 10000.0f);
}

float noise(vec2 coords)
{
	vec2 i = floor(coords);
	vec2 f = fract(coords);

	float a = rand(i);
	float b = rand(i + vec2(1.0f, 0.0f));
	float c = rand(i + vec2(0.0f, 1.0f));
	float d = rand(i + vec2(1.0f, 1.0f));

	vec2 cubic = f * f * (3.0f - 2.0f * f);

	return mix(a, b, cubic.x) + (c - a) * cubic.y * (1.0f - cubic.x) + (d - b) * cubic.x * cubic.y;
}

float fbm(vec2 coords)
{
	float value = 0.0f;
	float scale = 0.5f;

	for (int i = 0; i < 5; i++)
	{
		value += noise(coords) * scale;
		coords *= 4.0f;
		scale *= 0.5f;
	}

	return value;
}

float pixels = 256;

float value(vec2 fragCoord)
{
	vec2 uv = fragCoord;
    uv.x = floor(uv.x*pixels)/pixels;
    uv.y = floor(uv.y*pixels)/pixels;

    float final = 0.0f;
    
    for (int i =0;i < 3; i++)
    {
        vec2 motion = vec2(fbm(uv + vec2(iTime * 0.7f, -iTime * 0.05f) * 0.05f + vec2(i)));
        final += fbm(uv + motion + vec2(i * uv));
    }

	return final / 3.0f;
}


out vec4 fragColor;
void main()
{
	vec2 texC = texCoords;
	texC.x *= (iResolution.x / iResolution.y);
	fragColor = vec4(mix(vec3(74 / 255.0f, 255 / 255.0f, 222 / 255.0f) + vec3(0.3f), vec3(23 / 255.0f, 36 / 255.0f, 71 / 255.0f) + vec3(-0.3f), 1 - value(texC)), 1);
}