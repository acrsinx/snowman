shader_type spatial;
render_mode unshaded;
uniform sampler2D noise: filter_linear;
uniform sampler2D screen: hint_screen_texture, filter_nearest;
uniform sampler2D depth: hint_depth_texture, filter_nearest;
varying vec3 vertex_world;
void vertex() {
    vertex_world = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}
void fragment() {
    vec4 upos = INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, texture(depth, SCREEN_UV).r, 1.0);
    vec3 pixel_position = upos.xyz / upos.w;
    float passDistance = distance(pixel_position, VERTEX);
    vec3 view_world = normalize(CAMERA_POSITION_WORLD - vertex_world);
    float passLength = 1.0 / dot(view_world, vec3(0, 1, 0));
    passLength = min(passLength, passDistance);
    float value = 0.05 * exp(passLength) * texture(noise, UV + vec2(TIME * 0.01)).r;
    ALBEDO = texture(screen, SCREEN_UV).rgb * exp(-passDistance*0.3) + vec3(0.3843, 0.4549, 0.5490) * value;
}
