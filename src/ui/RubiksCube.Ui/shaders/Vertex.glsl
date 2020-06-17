#version 450

layout(set = 0, binding = 0) uniform ProjectionViewWorldBuffer
{
    mat4 Projection;
    mat4 View;
    mat4 World;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 TexCoords;
layout(location = 3) in float HighLighted;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 fsin_texCoords;

void main()
{
    vec4 worldPosition = World * vec4(Position, 1);
    gl_Position = Projection * View * worldPosition;

    fsin_Color = Color;
    fsin_texCoords = TexCoords;
}