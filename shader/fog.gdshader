shader_type spatial;
render_mode unshaded;
uniform sampler2D noise: filter_linear_mipmap;
varying vec3 vertex_world;
void vertex() {
    vertex_world = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}
void fragment() {
    vec3 view_world = normalize(CAMERA_POSITION_WORLD - vertex_world);
    float passLength = 1.0 / dot(view_world, vec3(0, 1, 0));
    float value = 0.1 * exp(passLength) * texture(noise, UV + vec2(TIME * 0.01)).r;
    ALBEDO = vec3(0.3843, 0.4549, 0.5490) * value;
}
