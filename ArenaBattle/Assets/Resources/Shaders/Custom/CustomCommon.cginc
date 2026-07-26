#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

#define AMBIENT_COLOR(albedo,color,value) (albedo * color * value)

#define RIM_DIRECT_BASIC(dir) (normalize(dir))
inline fixed3 RIM_DIRECT_VECTOR( fixed3 viewDir, fixed4 vec ) {
	fixed3 dir = normalize( viewDir );
	fixed3 right = cross( dir, fixed3(0,1,0) );
	fixed3 up = cross( dir, right );
	dir += (right * vec.x + up * vec.y);
	dir = normalize( dir );
	return dir;
 }

#define RIM_COLOR(color,power,dir,norm) (color * pow((1.0 - saturate( dot(dir, norm) )), power))

#define DISSOLVE_CLEAR(color,value,amount,size) (lerp( color, fixed4(0,0,0,0), max(0.0, int(value - (amount + size) + 0.99)) ))
#define DISSOLVE_COLOR(color,clear,value,amount) (lerp( color, clear, max(0.0, int(value - amount + 0.99)) ))
#define DISSOLVE_ALPHA(value,amount,size) (lerp( 1.0, 0.0, int(value - (amount + size) + 0.99) ))

fixed4 _SpecColor0;
fixed4 _SpecColor1;
fixed4 _SpecColor2;


inline fixed4 LightingMobileBlinnPhong( SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten ) {
	fixed diff = max( 0, dot(s.Normal, lightDir) ); // Diffuse Value

	half3 dir = normalize( lightDir + viewDir );

	float nh = max( 0, dot(s.Normal, dir) );
	float spec = pow( nh, (s.Specular * 128.0) ) * s.Gloss; // Specular Value (LightDir and ViewDir)
	
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor0.rgb * spec) * (atten * 2);
	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingMobileBlinnPhongEx( SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten ) {
	fixed diff = max( 0, dot(s.Normal, lightDir) ); // Diffuse Value

	float nh1 = max( 0, dot(s.Normal, lightDir) );
	float spec1 = pow( nh1, (s.Specular * 128.0) ) * s.Gloss; // Specular Value (LightDir)

	float nh2 = max( 0, dot(s.Normal, viewDir) );
	float spec2 = pow( nh2, (s.Specular * 128.0) ) * s.Gloss; // Specular Value (ViewDir)
			
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor0.rgb * (_SpecColor1.rgb * spec1 + _SpecColor2.rgb * spec2)) * (atten * 2);
	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingMobileBlinnPhongViewDir( SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten ) {
	fixed diff = max( 0, dot(s.Normal, lightDir) ); // Diffuse Value
			
	float nh = max( 0, dot(s.Normal, viewDir) );
	float spec = pow( nh, (s.Specular * 128.0) ) * s.Gloss; // Specular Value (ViewDir)
			
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor0.rgb * spec) * (atten * 2);
	c.a = s.Alpha;
	return c;
}

#endif