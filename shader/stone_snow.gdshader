shader_type spatial;
render_mode diffuse_toon, specular_toon;
uniform sampler2D noise: filter_linear_mipmap;
uniform sampler2D stone: filter_linear_mipmap;
uniform sampler2D snow: filter_linear_mipmap;
void fragment() {
    vec2 uv = UV;
    float factor = texture(noise, uv * 1.0).r;
    NORMAL = mix(texture(stone, uv).rgb, texture(snow, uv).rgb, factor);
    ALBEDO = mix(vec3(0.238), vec3(1), factor);
    ROUGHNESS = 1.0;
}
