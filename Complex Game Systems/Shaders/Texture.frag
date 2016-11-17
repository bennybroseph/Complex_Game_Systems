#version 410

in vec4 vColor;
in vec2 vTextureUV;
in vec3 vNormal;
in vec3 vTangent;
in vec3 vBiTangent;

out vec4 fragColour;

uniform vec3 lightDirection;

uniform sampler2D textures[30];
uniform int textureCount;

uniform sampler2D normalMap;
uniform sampler2D diffuseMap;
uniform sampler2D specularMap;

void main()
{
	mat3 TBN = mat3(
		normalize(vTangent),
		normalize(vBiTangent),
		normalize(vNormal));

	vec3 N = texture(normalMap, vTextureUV).xyz * 2 - 1;
	float d = max(0, dot(normalize(TBN * N), normalize(lightDirection)));

	fragColour = texture(diffuseMap, vTextureUV);
	//fragColour.rgb = fragColour.rgb * d;
}