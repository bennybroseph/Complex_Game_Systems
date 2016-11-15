#version 410

in vec4 vColor; // must match name in vertex shader

out vec4 fragColor; // first out variable is automatically written to the screen

void main()
{
	fragColor = vColor;
}