shader_type spatial;
render_mode unshaded;

uniform sampler2D screen_texture: hint_screen_texture, filter_linear_mipmap;
uniform sampler2D depth_texture: hint_depth_texture, filter_linear_mipmap;
uniform float range = 2.0;
uniform float threshold = 0.9;

vec4 getDepth(vec2 uv, mat4 matrix) {
    float depth = texture(depth_texture, uv).r;
    vec4 upos = matrix * vec4(uv * 2.0 - 1.0, depth, 1.0);
    vec3 pixel_position = upos.xyz / upos.w;
    return vec4(pixel_position, depth);
}

void fragment() {
    vec2 uv = SCREEN_UV;
    float realRange = range;
    vec4 depth = getDepth(uv, INV_PROJECTION_MATRIX);
    vec4 depthL = getDepth(uv-vec2(realRange, 0.0)/VIEWPORT_SIZE, INV_PROJECTION_MATRIX);
    vec4 depthR = getDepth(uv+vec2(realRange, 0.0)/VIEWPORT_SIZE, INV_PROJECTION_MATRIX);
    vec4 depthU = getDepth(uv-vec2(0.0, realRange)/VIEWPORT_SIZE, INV_PROJECTION_MATRIX);
    vec4 depthD = getDepth(uv+vec2(0.0, realRange)/VIEWPORT_SIZE, INV_PROJECTION_MATRIX);
    float diff1 = dot(normalize(depthR - depth), normalize(depth - depthL));
    float diff2 = dot(normalize(depthU - depth), normalize(depth - depthD));
    float a = (diff1 < threshold || diff2 < threshold)&&length(depth)<100.0?1.0:0.0;
    vec3 color = texture(screen_texture, uv).rgb;
    color = color - 0.1 * sin(2.0 * PI * color);
    a = 1.0 - a;
    ALBEDO = color.rgb * vec3(a);
}
