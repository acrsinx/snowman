shader_type spatial;
render_mode unshaded;
uniform sampler2D screen_texture: hint_screen_texture, filter_linear_mipmap;
uniform sampler2D depth_texture: hint_depth_texture, filter_linear_mipmap;
vec3 deepenColor(vec3 c) {
    float len = max(length(c), 0.0);
    float targetLen = max(len - 0.5, 0.0);
    float scale = targetLen / len;
    return c * scale;
}
float getDepth(vec2 uv, mat4 matrix) {
    float depth = texture(depth_texture, uv).r;
    vec4 upos = matrix * vec4(uv * 2.0 - 1.0, depth, 1.0);
    vec3 pixel_position = upos.xyz / upos.w;
    return length(pixel_position);
}
void fragment() {
    vec2 uv = SCREEN_UV;
    float dx = 3.0 / VIEWPORT_SIZE.x;
    float dy = 3.0 / VIEWPORT_SIZE.y;
    // [0, 1, 2]
    // [3, 4, 5]
    // [6, 7, 8]
    // 采样周围五个点的颜色
    vec3 col1 = texture(screen_texture, uv - vec2(0.0, dy)).rgb;
    vec3 col3 = texture(screen_texture, uv - vec2(dx, 0.0)).rgb;
    vec3 col4 = texture(screen_texture, uv).rgb;
    vec3 col5 = texture(screen_texture, uv + vec2(dx, 0.0)).rgb;
    vec3 col7 = texture(screen_texture, uv + vec2(0.0, dy)).rgb;
    vec3 conv = col1 + col3 + col5 + col7 - 4.0 * col4;
    float a = 0.3333 * (abs(conv.r) + abs(conv.g) + abs(conv.b));
    float depth4 = getDepth(uv, INV_PROJECTION_MATRIX);
    ALBEDO = (a > 0.2 && depth4 < 35.0)?deepenColor(col4):col4;
}
