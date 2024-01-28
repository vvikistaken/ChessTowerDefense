shader_type canvas_item;

uniform bool invert = false;

// Called for every pixel the material is visible on.
void fragment() {
	vec4 color = texture(TEXTURE, UV);
	if(invert)
		COLOR = vec4(1.0 - color.rgb, color.a);
	else
		COLOR = vec4(color.rgba);
}
