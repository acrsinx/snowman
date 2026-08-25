#define h(x) texture(height, x).r*0.2
shader_type spatial;
uniform sampler2D height: filter_linear;
uniform sampler2D noise: filter_linear;
void vertex() {
    ROUGHNESS = 0.99;
    float hm = h(UV);
    VERTEX.y += hm;
    NORMAL = normalize(vec3(hm - h(UV + vec2(0.01, 0.0)), 0.1, hm - h(UV + vec2(0.0, 0.01))));
}
void fragment() {
    ALBEDO = texture(noise, UV).rrr;
}
