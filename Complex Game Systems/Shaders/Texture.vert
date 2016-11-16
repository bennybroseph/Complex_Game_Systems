#version 410

// attributes of our vertex
in vec3 inPosition;
in vec4 inColor;
in vec2 inTextureUV;
in vec3 inNormal;
in vec3 inTangent;

// must match name in fragment shader
out vec4 vPosition;
out vec4 vColor;
out vec2 vTextureUV;
out vec3 vNormal;
out vec3 vTangent;
out vec3 vBiTangent;

// a projection transformation to apply to the vertex' position
uniform mat4 ProjectionMatrix;

void main()
{
	vPosition = vec4(inPosition, 1.0f);
	vColor = inColor;
	vTextureUV = inTextureUV;
	vNormal = inNormal;
	vTangent = inTangent;
	vBiTangent = cross(vNormal, vTangent);

	// gl_Position is a special variable of OpenGL that must be set
	gl_Position = ProjectionMatrix * vec4(inPosition, 1.0f);
}