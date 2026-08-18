shader_type canvas_item;
render_mode unshaded;
uniform sampler2D screen_texture: filter_linear;
vec3 deepenColor(vec3 c, float a) {
    float len = max(length(c), 0.0);
    float targetLen = max(len - (a < 0.5?0.0:1.0), 0.0);
    float scale = targetLen / len;
    return c * scale;
}
void fragment() {
    vec2 uv = SCREEN_UV;
    float dx = SCREEN_PIXEL_SIZE.x;
    float dy = SCREEN_PIXEL_SIZE.y;
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
    float a = abs(conv.r) + abs(conv.g) + abs(conv.b);
    COLOR = vec4(deepenColor(col4, a), 1);
}
