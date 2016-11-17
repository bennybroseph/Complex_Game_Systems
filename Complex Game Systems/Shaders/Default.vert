#version 410

// attributes of our vertex
layout(location=0) in vec3 inPosition;
layout(location=1) in vec4 inColor;

out vec4 vColor; // must match name in fragment shader

// a projection transformation to apply to the vertex' position
uniform mat4 projectionMatrix;

void main()
{
	vColor = inColor;

	// gl_Position is a special variable of OpenGL that must be set
	gl_Position = projectionMatrix * vec4(inPosition, 1.0);
}