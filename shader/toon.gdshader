shader_type spatial;
render_mode unshaded;
uniform sampler2D screen_texture: hint_screen_texture, filter_linear_mipmap;
uniform sampler2D depth_texture: hint_depth_texture, filter_linear_mipmap;
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
    float depth1 = getDepth(uv - vec2(0.0, dy), INV_PROJECTION_MATRIX);
    float depth3 = getDepth(uv - vec2(dx, 0.0), INV_PROJECTION_MATRIX);
    float depth4 = getDepth(uv, INV_PROJECTION_MATRIX);
    float depth5 = getDepth(uv + vec2(dx, 0.0), INV_PROJECTION_MATRIX);
    float depth7 = getDepth(uv + vec2(0.0, dy), INV_PROJECTION_MATRIX);
    // a
    // b
    // c
    vec3 depthB = vec3(depth3, depth4, depth5);
    // 卷积核
    // [0, 1, 0] d
    // [1, -4, 1] e
    // [0, 1, 0] f
    vec3 depthE = vec3(1, -4, 1);
    // 卷积
    float a = depth1 + dot(depthB, depthE) + depth7;
    a = abs(a);
    vec3 color = texture(screen_texture, uv).rgb;
    ALBEDO = a > 0.1 && depth4 < 100.0?vec3(0):color;
}
