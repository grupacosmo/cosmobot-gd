shader_type spatial;

render_mode specular_toon;

uniform sampler2D noise;
uniform float rim = 0.2f;
uniform float metalic = 0.0f;
uniform float roughness = 0.01f;
uniform vec3 albedo: source_color = vec3(0.1, 0.3, 0.5);
uniform float amplitude = 0.3f; // it is not a real amplitude, though I cannot redefin 'height' so will  stay with this for now and replace in feature with better name


float wave(vec2 position){
  position += texture(noise, position / 10.0).x * 2.0 - 1.0;
  vec2 wv = 1.0 - abs(sin(position));
  return pow(1.0 - pow(wv.x * wv.y, 0.65), 4.0);
}

float height(vec2 position, float time) {
  float d = wave((position + time) * 0.4) * amplitude;
  d += wave((position - time) * 0.3) * amplitude;
  d += wave((position + time) * 0.5) * amplitude;
  d += wave((position - time) * 0.6) * amplitude;
  return d;
}

void vertex() {
	vec2 pos = VERTEX.xz / 2.0 + 0.5;
	float time = TIME * 0.3;
	VERTEX.y = height(pos, time);
	NORMAL = normalize(vec3(VERTEX.y - height(pos + vec2(0.1, 0.0), time), 0.1, VERTEX.y - height(pos + vec2(0.0, 0.1), time)));
}

void fragment() {
  float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
  RIM = rim;
  METALLIC = metalic;
  ROUGHNESS = roughness * (1.0 - fresnel);  
  ALBEDO = albedo * (0.2 * fresnel);
}