Shader "Custom/BoffaWaterCartoonShader"
{
    Properties
    {
        _DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725) //Shallow Water Color PreSet (0,1)
        _DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749) // Deep Water Color PreSet (0,1)

        _DepthMaxDistance("Depth Maximum Distance", Float) = 1 
        _SurfaceNoise("Surface Noise", 2D) = "white" {} // wave texture noise texture
        _SurfaceDistortion("Surface Distortion",2D) = "white"{}
        _SurfaceNoiseCutOff("Surface Noise Cut Off",Range(0,1)) = 0.777  // cutoff threshold to get a more binary look.
        _SurfaceDistortionAmmount("Surface Distortion Ammount", Range(0,1)) = 0.26

        _FoamColor("Foam Color", Color) = (1,1,1,1)
        _FoamMaxDistance("Foam Max Distance",Float) = 0.4 // to increase near the shoreline or where objects intersect the surface of the water, to create a foam effect.
        _FoamMinDistance("Foam Min Distance",Float) = 0.04 // to increase near the shoreline or where objects intersect the surface of the water, to create a foam effect.

      
 
   
            _WaveDirection1and2("Wave Direction 1 xy and 2 zw", Vector) = (0,0,0,0)
           _WaveDirection3and4("Wave Direction 3 xy and 4 zw", Vector) = (0,0,0,0)
           _WaveAmplitudeVector("Wave Amplitude Vector", Vector) = (1,1,1,1)
           _WaveTimeScales("Wave timescales", Vector) = (1,1,1,1)


           _WavePhase("WavePhase",Range(-3,3)) = 0.5
           _WaveGravity("WaveGravity",Range(-10,10)) = 0.5
           _WaveDepth("WaveDepth",Range(-10,10)) = 0.5

    }
    SubShader
    {
            Tags {     "Queue" = "Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off // only partially obscuring other object
			CGPROGRAM

            #define SMOOTHSTEP_AA 0.01

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define NEXT1 (0,0,0.1)
            #define NEXT2 (0.1,0,0)

            float4 _DepthGradientShallow; // collor for shallow water
            float4 _DepthGradientDeep; // collor for deep water

            float _DepthMaxDistance;// distance to determine considerations of deep, Shalllow Water < DeptMax < Deep Water 

            sampler2D _CameraDepthTexture;  // depth texture from camera, grayscaled based on distance

            sampler2D _SurfaceNoise; // Noise 2D Texture
            float4 _SurfaceNoise_ST; // Noise 2D Texture tiling and offest values, automatic inicialized via "_ST" end
            float _SurfaceNoiseCutOff; // Noise CutOff threshold

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;

            float _SurfaceDistortionAmmount;

            float4 _FoamColor;
            float _FoamMaxDistance; // Foam Distance threshold
            float _FoamMinDistance; // Foam Distance threshold

            float2 _SurfaceNoiseScroll; // Vector controlling waves animations 

            sampler2D _CameraNormalsTexture;

        

        
  
        
            float _WavePhase;
            float _WaveGravity;
            float _WaveDepth;
            
            float4 _WaveAmplitudeVector;
            float4 _WaveDirection1and2;
            float4 _WaveDirection3and4;

            float4 _WaveTimeScales;
          

            float4 alphaBlend(float4 top, float4 bottom)
            {
                float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
                float alpha = top.a + bottom.a * (1 - top.a);

                return float4(color, alpha);
            }

            float4 waveModifierVertex(float time, float waveHeight, float2 waveDisplacement, float2 waveDirection) {

                float4 vertexDisplacement;
                
                vertexDisplacement.y = waveHeight * sin(time * waveDisplacement.y);
                vertexDisplacement.x = waveHeight * cos(waveDisplacement.x - time * waveDisplacement.x);//d  +round(waveDisplacement.x);
                vertexDisplacement.z = 0;
             
                vertexDisplacement.w = 0;


                return  vertexDisplacement;

            }


            float WaveFrequency(float gravity, float depth, float2 direction) {
                float len = length(direction);
                float GK = gravity * len;
                float DK = tanh(depth*len);
                float sq = sqrt(GK * DK);
                return sq;
            }

            float Theta(float3 tposition, float tphase, float ttime, float tgravity, float2 tdirection, float tdepth, float tamplitude) {

                float TX = tdirection.x * tposition.x;
                float TZ = tdirection.y * tposition.y;

                float frequency = WaveFrequency(tgravity,tdepth,tdirection) * ttime;
                
                return (((TX + TZ) - frequency) - tphase);

            }

            float WaveXDisplacement(float2 dir, float theta, float3 pos, float depth, float amp) {
                float len = length(dir);
                float tx = (dir.x / len);
                float ty = (amp / tanh(len * depth));
                return -1 * (tx * ty) * cos(theta);

            }
            float WaveZDisplacement(float2 dir, float theta, float3 pos, float depth, float amp) {
                float len = length(dir);
                float tz = (dir.y / len);
                float ty = (amp / tanh(len * depth));
                return -1 * (tz * ty) * sin(theta);

            }

            float WaveYDisplacement(float theta, float amplitude) {
                return amplitude * cos(theta);
            }

            float _Theta;


            float3 WaveFormula(float2 direction, float amplitude, float3 position, float phase, float time, float gravity, float depth ) {
            
                
                float theta = Theta(position, phase, time, gravity, direction, depth, amplitude);
          
                float Xdisplacement = WaveXDisplacement(direction, theta, position, depth, amplitude);
                float Zdisplacement = WaveZDisplacement(direction, theta, position, depth, amplitude);
                float Ydisplacement = WaveYDisplacement(theta, amplitude);

                return float3(Xdisplacement, Ydisplacement, Zdisplacement);

            }

            float4 GersenerWaveFunction(float3 next, float4 dir1, float4 dir2, float4 dir3, float4 dir4, float amp1, float amp2, float amp3, float amp4,
                                        float3 position, float phase, float time, float gravity, float depth,float4 timescale) {

                float3 displacement1 = WaveFormula(dir1, amp1, position, phase, time * timescale.x, gravity, depth);
                float3 displacement2 = WaveFormula(dir2, amp2, position, phase, time * timescale.y, gravity, depth);
                float3 displacement3 = WaveFormula(dir3, amp3, position, phase, time * timescale.z, gravity, depth);
                float3 displacement4 = WaveFormula(dir4, amp4, position, phase, time * timescale.w, gravity, depth);
                
                float3 result = displacement1 + displacement2 + displacement3 + displacement4;


                return float4(result.xyz,0);
            }

            float4 GersenerWaveShortFunction(float3 next,float4 Dir12, float4 Dir34, float4 amps,
                float3 position, float phase, float time, float gravity, float depth, float4 timescale) {

                float3 displacement1 = WaveFormula(Dir12.xy, amps.x, position, phase, time * timescale.x, gravity, depth);
                float3 displacement2 = WaveFormula(Dir12.zw, amps.y, position, phase, time * timescale.y, gravity, depth);
                float3 displacement3 = WaveFormula(Dir34.xy, amps.z, position, phase, time * timescale.z, gravity, depth);
                float3 displacement4 = WaveFormula(Dir34.zw, amps.w, position, phase, time * timescale.w, gravity, depth);

                float3 result = displacement1 + displacement2 + displacement3 + displacement4;


                return float4(result.xyz, 0);
            }
            // next1 in z, next2 in x
            float3 NewNormal(float3 source, float3 next1, float3 next2) {

                float3 n1 = next1 - source;
                float3 n2 = next2 - source;
                return normalize(cross(normalize(n1), normalize(n2)));
            }


            struct appdata // vertice  
            {
                float4 vertex : POSITION; // vertice position
                float4 uv : TEXCOORD0; // vertice 2d texture coordinates 
                float3 normal : NORMAL;
            };

            struct v2f // vertice to fragment 
            {
                float4 vertex : SV_POSITION;
                float2 noiseUV : TEXCOORD0; // coordenate for noise texture
                float2 distortUV : TEXCOORD1; 
                float4 screenPosition : TEXCOORD2;
                float3 viewNormal: NORMAL;
            };

        

            v2f vert (appdata v)
            {
                v2f o;

              
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise); // modifies the UV input with provided texture's tiling and offset settings
                o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
           
              
               
                //_Theta = Theta(v.vertex, _WavePhase, _Time.y, _WaveGravity, _WaveDirection, _WaveDepth, _WaveAmplitude);
                o.vertex = UnityObjectToClipPos(v.vertex) - _WaveAmplitudeVector.x * waveModifierVertex(_Time.y, _WaveAmplitudeVector.y, _WaveDirection3and4, _WaveDirection1and2);
                o.vertex = UnityObjectToClipPos(v.vertex) - _WaveDepth * waveModifierVertex(_Time.y, _WaveGravity, _WaveDirection1and2.xy, _WaveDirection1and2.zw );

              //  o.vertex = UnityObjectToClipPos(v.vertex) - _WaveForce * waveModifierVertex(_Time.y, _WaveHeight/2,_WaveDisplacement, _WaveDirection1and2);
                //o.vertex = UnityObjectToClipPos(v.vertex) - WaveFormula(v.vertex, _WavePhase, _Time.y, _WaveGravity, _WaveDirection, _WaveDepth, _WaveAmplitude);
                //   float3 position, float phase, float time, float gravity, float depth,float4 timescale) {
                 
                
                
                /*GerstnerWaveShortFunction method
                float3 position = UnityObjectToClipPos(v.vertex);
                   

                float4 node1 = GersenerWaveShortFunction(float3(0, 0, 0), _WaveDirection1and2, _WaveDirection3and4, _WaveAmplitudeVector,
                    position, _WavePhase, _Time.y, _WaveGravity, _WaveDepth, _WaveTimeScales);
                float4 node2 = GersenerWaveShortFunction(float3(0, 0, 0), _WaveDirection1and2, _WaveDirection3and4, _WaveAmplitudeVector,
                    position, _WavePhase, _Time.y, _WaveGravity, _WaveDepth, _WaveTimeScales);
                float4 node3 = GersenerWaveShortFunction(float3(0, 0, 0), _WaveDirection1and2, _WaveDirection3and4, _WaveAmplitudeVector,
                    position, _WavePhase, _Time.y, _WaveGravity, _WaveDepth, _WaveTimeScales);
                
               
                float4 node1 = GersenerWaveFunction(float3(0, 0, 0), _WaveDirection1, _WaveDirection2, _WaveDirection3, _WaveDirection4, _WaveAmplitude1, _WaveAmplitude2, _WaveAmplitude3, _WaveAmplitude4,
                    position, _WavePhase, _Time.y, _WaveGravity, _WaveDepth, _WaveTimeScales);
                float4 node2 = GersenerWaveFunction(NEXT1, _WaveDirection1, _WaveDirection2, _WaveDirection3, _WaveDirection4, _WaveAmplitude1, _WaveAmplitude2, _WaveAmplitude3, _WaveAmplitude4,
                    position, _WavePhase, _Time.y, _WaveGravity, _WaveDepth, _WaveTimeScales);
                float4 node3 = GersenerWaveFunction(NEXT2, _WaveDirection1, _WaveDirection2, _WaveDirection3, _WaveDirection4, _WaveAmplitude1, _WaveAmplitude2, _WaveAmplitude3, _WaveAmplitude4,
                    position, _WavePhase, _Time.y, _WaveGravity, _WaveDepth, _WaveTimeScales);

              

                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex) + node1;
                o.viewNormal = NewNormal(node1, node2, node3);
                  */
                o.viewNormal = COMPUTE_VIEW_NORMAL;
                o.screenPosition = ComputeScreenPos(o.vertex); // get screen position of object to screen position
               
            

               
                return o;
            }

       


            float4 frag(v2f i) : SV_Target
            {
                float existingDepth01 = tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.screenPosition)).r; // samples the depth texture using tex2Dproj and our screen position.
                // Could also be done with
                //float existingDepth01 = tex2D(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition.xy / i.screenPosition.w)).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01); // converts the non-linear depth to be linear, in world units from the camera.

                float depthDifference = existingDepthLinear - i.screenPosition.w; // how deep this depth value is relative to our water surface,

                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);//  how deep it is compared to our maximum depth, percentage-wise, clamps the value between 0 and 1,
                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);// calculate the gradient and return our new water color.
                
                
               
                // get distortionsample from distortion surface texture based on distortion ammount
                float2 distortionSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 4 - 1) * _SurfaceDistortionAmmount;
                // animate UV map of noise texture based on time
                float2 waveDir= _WaveDirection1and2.xy;
                waveDir.y = _WaveDirection1and2;
               // float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x * waveDir.y)+ distortionSample.x, (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y* waveDir.x) + distortionSample.y);
                float2 noiseUV = float2((i.noiseUV.x + _Time.y *  waveDir.y * 0.2)+ distortionSample.x, (i.noiseUV.y + _Time.y * waveDir.x* 0.2) + distortionSample.y);
       
                

                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;
             

                float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));
                float3 normalDot = saturate(dot(existingNormal, i.viewNormal)); //saturate(dot(existingNormal, i.viewNormal));

                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
                float foamDepthDifference01 = saturate(depthDifference / foamDistance);//modulating the noise cutoff threshold based off the water depth.
               

                float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutOff; 
                

                // not Smooth without Anti Aliasing
                //float surfaceNoise = surfaceNoiseSample > surfaceNoiseCutoff ? 1 : 0;
                
                // smooth with AntiAliasing
                float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);

                float4 surfaceNoiseColor = _FoamColor;
                surfaceNoiseColor.a *= surfaceNoise;
                  return alphaBlend(surfaceNoiseColor, waterColor);
               
               //return alphaBlend(waterColor,surfaceNoiseColor);
              
                //return waterColor;
            }
            ENDCG
        }
    }
}