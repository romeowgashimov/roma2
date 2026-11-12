shader_type spatial;

// Настройки цветов
uniform vec4 color_top : source_color = vec4(0.1, 0.8, 0.9, 1.0);
uniform vec4 color_bottom : source_color = vec4(0.6, 0.1, 0.9, 1.0);
uniform vec4 color_fresnel : source_color = vec4(1.0, 0.3, 0.8, 1.0);

// Настройки волн и бликов
uniform float speed : hint_range(0.0, 5.0) = 1.2;
uniform float wave_scale : hint_range(0.1, 10.0) = 2.0;
uniform float fresnel_power : hint_range(0.5, 10.0) = 2.0;

// Настройки свечения (Glow)
uniform float glow_strength : hint_range(0.0, 10.0) = 2.5;
uniform float fresnel_glow_boost : hint_range(0.0, 15.0) = 5.0;
uniform float pulse_speed : hint_range(0.0, 10.0) = 2.0;

varying vec3 local_pos;

void vertex() {
	local_pos = VERTEX;
}

void fragment() {
	// Плавная волна градиента
	float wave = sin(local_pos.y * wave_scale + TIME * speed) * 0.5 + 0.5;
	wave += cos(local_pos.x * wave_scale * 0.7 - TIME * speed * 0.8) * 0.25;
	wave = clamp(wave, 0.0, 1.0);
	
	vec3 base_gradient = mix(color_bottom.rgb, color_top.rgb, wave);
	
	// Эффект Френеля (свечение по краям)
	float fresnel = pow(1.0 - clamp(dot(NORMAL, VIEW), 0.0, 1.0), fresnel_power);
	
	// Легкая пульсация яркости со временем
	float pulse = sin(TIME * pulse_speed) * 0.15 + 1.0;
	
	// Итоговый базовый цвет
	vec3 final_color = mix(base_gradient, color_fresnel.rgb, fresnel);
	ALBEDO = final_color;
	
	// Считаем свечение: базовый градиент + усиленное свечение по краям
	vec3 base_emission = base_gradient * glow_strength;
	vec3 edge_emission = color_fresnel.rgb * fresnel * fresnel_glow_boost;
	
	EMISSION = (base_emission + edge_emission) * pulse;
}