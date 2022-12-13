#version 460


in vec2 texCoords;

layout(binding = 0) uniform sampler2D inputTexture;
layout(binding = 1) uniform sampler2D shadowTexture;


layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 NormalColor;
layout (location = 2) out vec4 FragPos;

//
//uniform float transmit=0.99;// light transmition coeficient <0,1>
//uniform int txrsiz=512;     // max texture size [pixels]
//uniform vec2 t0 = vec2(10,10);            // texture start point (mouse position) <0,1>
//
////
void main()
{
	NormalColor = texture2D(shadowTexture, texCoords);
	FragColor = texture2D(inputTexture, texCoords);
}
//void main()
//    {
//    int i;
//    vec2 t,dt;
//    vec4 c0,c1;
//    dt=normalize(texCoords-t0)/float(txrsiz);
//    c0=vec4(1.0,1.0,1.0,1.0);   // light ray strength
//    t=t0;
//    if (dot(texCoords-t,dt)>0.0)
//     for (i=0;i<txrsiz;i++)
//        {
//        c1=texture2D(inputTexture,t);
//        c1.a = c1.r;
//        c0.rgb*=((c1.a)*(c1.rgb))+((1.0f-c1.a)*transmit);
//        if (dot(texCoords-t,dt)<=0.000f) break;
//        if (c0.r+c0.g+c0.b<=0.001f) break;
//        t+=dt;
//        }
//    fragColor=0.90*c0+0.10*texture2D(inputTexture,texCoords);  // render with ambient light
////  col=c0;                                 // render without ambient light
//    }