shader_type spatial;
uniform sampler2D noise;
varying vec2 tex_position;
void vertex() {
    tex_position = VERTEX.xz / 2.0 + 0.5;
    float heightScale = -length(tex_position - vec2(-2.0, 0.0));
    VERTEX.y += texture(noise, tex_position).r * 2.0 + heightScale;
}
void fragment() {
    NORMAL += texture(noise, tex_position).rrr + vec3(-0.5);
    float up = dot(NORMAL, vec3(0, 1, 0));
    ALBEDO = vec3(up * 1.7);
}
